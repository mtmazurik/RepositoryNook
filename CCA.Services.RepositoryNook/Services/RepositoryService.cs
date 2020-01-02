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
        public async Task<List<string>> GetDatabases()
        {
            var client = new MongoClient(_config.AtlasMongoConnection);
            List<string> databases = new List<string>();

            using (var cursor = await client.ListDatabasesAsync())
            {
                await cursor.ForEachAsync(d => databases.Add(d.ToString()));
            }
            return databases;
        }
        public async Task<List<string>> GetCollections(string database)
        {
            var client = new MongoClient(_config.AtlasMongoConnection);
            List<string> collections = new List<string>();

            using (var cursor = await client.GetDatabase(database).ListCollectionsAsync())
            {
                await cursor.ForEachAsync(d => collections.Add( new JObject(new JProperty("name",d["name"].ToString())).ToString()));
            }
            return collections;
        }
        public async Task<Repository> Create(string repository, string collection, Repository repoObject)
        {
            if( repoObject.validate )
            {
                ValidateInnerDataAgainstSchema(repoObject.schemaUri, repoObject.data);
            }

            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            if (repoObject.createdDate == DateTime.MinValue)   // user can send in a creation date, else we insert now()
            {
                repoObject.createdDate = DateTime.Now;
            }

            CreateRepositoryTextIndices(repositoryCollection);   //enable auto-indexing feature after re-doing the separation of the key/value pairs for "key" and "tags"

            if (repoObject._id == null)                         // user can send in a unique identifier, else we generate a mongo ObjectId (mongo unique id)
            {
                repoObject._id = ObjectId.GenerateNewId().ToString();
            }
            if (repoObject.createdDate == DateTime.MinValue)   // user can send in a creation date, else we insert now()
            {
                repoObject.createdDate = DateTime.Now;
            }

            await repositoryCollection.InsertOneAsync(repoObject);
            return repoObject;
        }

        public async Task<Repository> Read(string _id, string repository, string collection)
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var filter = Builders<Repository>.Filter.Eq("_id", _id);      // FIND with filter  filter("_id" = ObjectId(_id) ) - IS AN ASYNC CALL 
            var fluentFindInterface = repositoryCollection.Find(filter);

            Repository foundObject = await fluentFindInterface.SingleOrDefaultAsync().ConfigureAwait(false);

            if (foundObject is null)
            {
                throw new RepoSvcDocumentNotFoundException($"DocumentId: {_id}");
            }
            return foundObject;
        }
        public List<Repository> ReadAll(string repository, string collection)           // READ ALL repository object (mongo documents) - NOT AN ASYNC CALL
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var found = repositoryCollection.AsQueryable().ToList();

            if (found is null)
            {
                throw new RepoSvcDocumentNotFoundException("RepositoryService.ReadAll() error.");
            }
            return found;
        }
        public List<Repository> QueryByKey(string repository, string collection, string key)
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var found = repositoryCollection.Find(r => r.key == key).ToList();          // FIND keyName (req) and keyValue (req) - NOT AN ASYNC CALL

            if (found is null)
            {
                throw new RepoSvcDocumentNotFoundException($"key:{key}");
            }
            return found;
        }
        public List<Repository> QueryByTag(string repository, string collection, string tag)
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var builder = Builders<Repository>.Filter.AnyEq("tags", tag);

            var found = repositoryCollection.Find(builder).ToList();

            if (found is null)
            {
                throw new RepoSvcDocumentNotFoundException($"tag: {tag}");
            }
            return found;
        }
        public async Task Update(string repository, string collection, Repository repoObject)
        {

            if (repoObject.validate)
            {
                ValidateInnerDataAgainstSchema(repoObject.schemaUri, repoObject.data);
            }

            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            if (repoObject.modifiedDate is null) // if not user supplied, we put in the modified date
            {
                repoObject.modifiedDate = DateTime.Now;
            }


            var filter = Builders<Repository>.Filter.Eq("_id", repoObject._id);

            try
            {
                await repositoryCollection.ReplaceOneAsync(filter, repoObject, new UpdateOptions { IsUpsert = true });
            }
            catch
            {
                throw new RepoSvcDocumentNotFoundException($"DocumentId: {repoObject._id}");
            }

        }

        public async Task Delete(string repository, string collection, string _id)
        {
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

            var filter = Builders<Repository>.Filter.Eq("_id", _id);
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

        [Obsolete]
        private void CreateRepositoryTextIndices(IMongoCollection<Repository> collection)   // indempotent; a no-op if index already exists.
        {
            // index: key (primary) 
            var textKey = Builders<Repository>.IndexKeys.Text(t => t.key);             // the key value, is collections text search field, and is highly queryable
            var options = new CreateIndexOptions() { Name = "IX_key}" };               // (name:value pairs string), NOT unique
            collection.Indexes.CreateOne(textKey, options);                            // does not create it, if exists


            // index: for tags
            var tagIndices = Builders<Repository>.IndexKeys.Ascending(t => t.tags);    // the tags array (name:value pairs strings)
            var tags_ix_options = new CreateIndexOptions() { Name = "IX_tags}" };
            collection.Indexes.CreateOne(tagIndices, tags_ix_options);
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