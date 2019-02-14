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
            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection, repoObject);

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
        public async Task<Repository> Read(string repository, string collection, Repository repoObject)
        {
            var repoObjectId = repoObject._id.ToString();

            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection, repoObject);

            var filter = Builders<Repository>.Filter.Eq("_id", new ObjectId(repoObjectId));
            var fluentFindInterface = repositoryCollection.Find(filter);

            Repository foundObject = await fluentFindInterface.SingleOrDefaultAsync().ConfigureAwait(false);

            if (foundObject is null)
            {
                throw new RepoSvcDocumentNotFoundException(repoObjectId);
            }
            return foundObject;
        }

        public async Task Update(string repository, string collection, Repository repoObject)
        {
            var repoObjectId = repoObject._id.ToString();

            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection, repoObject);

            var filter = Builders<Repository>.Filter.Eq("_id", new ObjectId(repoObjectId));
            repoObject._id = new ObjectId(repoObjectId);                                                // objectify the GUID string
            if (repoObject.modifiedDate is null)
            {
                repoObject.modifiedDate = DateTime.Now;
            }
            var replaceOneResult = await repositoryCollection.ReplaceOneAsync(filter, repoObject, new UpdateOptions { IsUpsert = true });

            if (replaceOneResult.ModifiedCount == 0)
            {
                throw new RepoSvcDocumentNotFoundException(repoObjectId);
            }

        }

        public async Task Delete(string repository, string collection, Repository repoObject)
        {
            var repoObjectId = repoObject._id.ToString();

            IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection, repoObject);

            var filter = Builders<Repository>.Filter.Eq("_id", new ObjectId(repoObjectId));
            var result = await repositoryCollection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new RepoSvcDocumentNotFoundException(repoObjectId);
            }
        }

        private IMongoCollection<Repository> ConnectToCollection(string repository, string collection, Repository repoObject)
        {
            var client = new MongoClient(_config.AtlasMongoConnection);

            IMongoDatabase database = ConnectToDatabase(repository, repoObject, client);

            if (!CheckIfCollectionExists(database, collection))
            {
                throw new RepoSvcDatabaseOrCollectionNotFound($"Database or Collection Not Found. Check request and retry. Repository: {repository}, Collection: {collection}");
            }

            if (collection == null)
            {
                collection = GENERIC_COLLECTION_NAME;
            }
            repoObject.collection = collection;

            return database.GetCollection<Repository>(collection);
        }

        private IMongoDatabase ConnectToDatabase(string repository, Repository repoObject, MongoClient client)
        {
            if (repository == null)
            {
                repository = GENERIC_DB_NAME;
            }
            repoObject.repository = repository; // stuff into repoObject
            var database = client.GetDatabase(repository);
            return database;
        }

        // Indempotent: is a no-op if already exists.
        private void CreateRepositoryTextIndices(IMongoCollection<Repository> collection)
        {
            // text search field     .Text()   is the keyValue
            var textKey = Builders<Repository>.IndexKeys.Text(t => t.keyValue);       // the key value, is collections text search field, and is highly queryable
            var options = new CreateIndexOptions() { Name = "IX_keyValue}" };
            collection.Indexes.CreateOne(textKey, options);

            // another indexed field   is the  keyName
            var indexKey = Builders<Repository>.IndexKeys.Ascending(i => i.keyName);   // the key name, is text and is indexed for speedier queries
            var ix_options = new CreateIndexOptions() { Name = "IX_keyName}" };
            collection.Indexes.CreateOne(indexKey, ix_options);

            // finally the tags are madesearchable
            var tagsKey = Builders<Repository>.IndexKeys.Ascending(t => t.tags);        // the tags array
            var tags_ix_options = new CreateIndexOptions() { Name = "IX_tags}" };
            collection.Indexes.CreateOne(tagsKey, tags_ix_options);
        }

        private bool CheckIfCollectionExists(IMongoDatabase database, string collectionName)
        {
            var nameFilter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = nameFilter };
            return database.ListCollectionNames(options).Any();
        }
    }
}