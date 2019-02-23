using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCA.Services.RepositoryNook.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using CCA.Services.RepositoryNook.Config;
using CCA.Services.RepositoryNook.Exceptions;
using MongoDB.Bson.Serialization;
using System.Net;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CCA.Services.RepositoryNook.Services
{
    public class RepositoryService : IRepositoryService
    {
        private IJsonConfiguration _config;
        private readonly string GENERIC_DB_NAME = "repository-nook-db";
        private readonly string GENERIC_COLLECTION_NAME = "repository";

        public RepositoryService(IJsonConfiguration config)     // ctor
        {
            _config = config;
        }

        public async Task<Repository> Create(string repository, string collection, Repository repoObject)
        {
            if( repoObject.validate )
            {
                ValidateInnerDataAgainstSchema(repoObject.schemaUri, repoObject.data);
            }

            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            if (repoObject._id == null)                         // user can send in a unique identifier, else we generate a mongo ObjectId (mongo unique id)
            {
                repoObject._id = ObjectId.GenerateNewId();
            }
            if (repoObject.createdDate == DateTime.MinValue)   // user can send in a creation date, else we insert now()
            {
                repoObject.createdDate = DateTime.Now;
            }

            CreateRepositoryTextIndices(repositoryCollection);

            await repositoryCollection.InsertOneAsync(repoObject);
            return repoObject;
        }

        public async Task<Repository> Read(string _id, string repository, string collection)
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var filter = Builders<Repository>.Filter.Eq("_id", new ObjectId(_id));
            var fluentFindInterface = repositoryCollection.Find(filter);

            Repository foundObject = await fluentFindInterface.SingleOrDefaultAsync().ConfigureAwait(false);

            if (foundObject is null)
            {
                throw new RepoSvcDocumentNotFoundException($"DocumentId: {_id}");
            }
            return foundObject;
        }
        public List<Repository> QueryByKey(string repository, string collection, string keyName, string keyValue)
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            //var keyNameFilter = Builders<Repository>.Filter.Eq(d => d.keyName, keyName);
            //var keyValueFilter = Builders<Repository>.Filter.Eq(d => d.keyValue, keyValue);

            var foundObject = repositoryCollection.Find(r => r.keyName == keyName && r.keyValue == keyValue).ToList(); // linq complex query

            if (foundObject is null)
            {
                throw new RepoSvcDocumentNotFoundException($"keyName: {keyName}, keyValue: {keyValue}");
            }
            return foundObject;
        }
        public async Task Update(string _id, string repository, string collection, Repository repoObject)
        {

            if (repoObject.validate)
            {
                ValidateInnerDataAgainstSchema(repoObject.schemaUri, repoObject.data);
            }

            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var filter = Builders<Repository>.Filter.Eq("_id", new ObjectId(_id));
            repoObject._id = new ObjectId(_id);   // object-ize the GUID string

            if (repoObject.modifiedDate is null)
            {
                repoObject.modifiedDate = DateTime.Now;
            }
            var replaceOneResult = await repositoryCollection.ReplaceOneAsync(filter, repoObject, new UpdateOptions { IsUpsert = true });

            if (replaceOneResult.ModifiedCount == 0)
            {
                throw new RepoSvcDocumentNotFoundException($"DocumentId: {_id}");
            }

        }

        public async Task Delete(string _id, string repository, string collection)
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var filter = Builders<Repository>.Filter.Eq("_id", ObjectId.Parse(_id));
            var result = await repositoryCollection.DeleteOneAsync(filter);

            if (result.DeletedCount != 1)
            {
                throw new RepoSvcDocumentNotFoundException($"DocumentId: {_id}");
            }
        }

        //
        // private routines
        //
        private void ValidateInnerDataAgainstSchema(string schemaUri, string data)
        {
            JObject jobject = null;
            JSchema schema = null;
            try
            {
                schema = JSchema.Parse(ReadInStringFromWebUri(schemaUri));
                jobject = JObject.Parse(data);
            }
            catch (Exception exc)
            {
                throw new RepoSvcValidationError("Error parsing schema or data JSON, please check schema URI and file, and data for valid JSON, and retry.");
            }
            if (!jobject.IsValid(schema))
            {
                throw new RepoSvcValidationError("Invalid Error; validating data against schema failed. Check data and schema and retry.");
            };
        }

        private IMongoCollection<Repository> ConnectToCollection(string repository, string collection)
        {
            var client = new MongoClient(_config.AtlasMongoConnection);

            IMongoDatabase database = ConnectToDatabase(client, repository);

            if (!CheckIfCollectionExists(database, collection))
            {
                throw new RepoSvcDatabaseOrCollectionNotFound($"Database or Collection Not Found. Check request and retry. Repository: {repository}, Collection: {collection}");
            }

            if (collection == null)
            {
                collection = GENERIC_COLLECTION_NAME;
            }

            return database.GetCollection<Repository>(collection);
        }

        private IMongoDatabase ConnectToDatabase(MongoClient client, string repository)
        {
            if (repository == null)
            {
                repository = GENERIC_DB_NAME;
            }

            var database = client.GetDatabase(repository);

            return database;
        }

        private void CreateRepositoryTextIndices(IMongoCollection<Repository> collection)   // indempotent; a no-op if index already exists.
        {
            // text search field     .Text()   is the keyValue
            var textKey = Builders<Repository>.IndexKeys.Text(t => t.keyValue);             // the key value, is collections text search field, and is highly queryable
            var options = new CreateIndexOptions() { Name = "IX_keyValue}" };
            collection.Indexes.CreateOne(textKey, options);

            // another indexed field   is the  keyName
            var indexKey = Builders<Repository>.IndexKeys.Ascending(i => i.keyName);        // the key name, is text and is indexed for speedier queries
            var ix_options = new CreateIndexOptions() { Name = "IX_keyName}" };
            collection.Indexes.CreateOne(indexKey, ix_options);

            // finally the tags are madesearchable
            var tagsKey = Builders<Repository>.IndexKeys.Ascending(t => t.tags);            // the tags array
            var tags_ix_options = new CreateIndexOptions() { Name = "IX_tags}" };
            collection.Indexes.CreateOne(tagsKey, tags_ix_options);
        }         

        private bool CheckIfCollectionExists(IMongoDatabase database, string collectionName)
        {
            var nameFilter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = nameFilter };
            return database.ListCollectionNames(options).Any();
        }

        private string ReadInStringFromWebUri(string schemaUri)
        {
            try
            {
                WebRequest request = WebRequest.Create(schemaUri);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string schemaBody = String.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    schemaBody = sr.ReadToEnd();
                }
                return schemaBody;
            }
            catch (Exception exc)
            {
                throw new RepoSvcValidationError("error: reading in string from Uri. Check URI string and/or file existence, and retry.");
            }
        }
    }
}