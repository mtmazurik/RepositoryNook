using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCA.Services.RepositoryNook.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using CCA.Services.RepositoryNook.Config;

namespace CCA.Services.RepositoryNook.Services
{
    public class RepositoryService : IRepositoryService
    {
        private IJsonConfiguration _config;
        private readonly string DATABASE_NAME = "repository-nook-db";
        private readonly string REPOSITORY_COLLECTION = "repository";

        public RepositoryService(IJsonConfiguration config)     // ctor
        {
            _config = config;
        } 

        public Repository Create(Repository repoObject)
        {
            var client = new MongoClient(_config.AtlasMongoConnection);
            var database = client.GetDatabase(DATABASE_NAME);
            IMongoCollection<Repository> repositoryCollection = database.GetCollection<Repository>(REPOSITORY_COLLECTION);

            if (repoObject._id == null)                         // user can send in a unique identifier, else we generate a mongo ObjectId (mongo unique id)
            { 
                repoObject._id = ObjectId.GenerateNewId();
            }
            if (repoObject.createdDate == DateTime.MinValue)   // user can send in a creation date, else we insert now()
            { 
                repoObject.createdDate = DateTime.Now;
            }

            CreateRepositoryIndices(repositoryCollection);

            repositoryCollection.InsertOne(repoObject);
            return repoObject;
        }
        // create indices, iff not existing, based on passing in Primary, Secondary or Tertiary index value.  
        // Indempotent: is a no-op if already exists.
        private void CreateRepositoryIndices(IMongoCollection<Repository> collection)
        {
            var RepositoryModelBuilder = Builders<Repository>.IndexKeys;

            var primaryIndexModel = new CreateIndexModel<Repository>(RepositoryModelBuilder.Ascending(i => i.primaryIndex), new CreateIndexOptions() { Name = "primaryIndex" });
            collection.Indexes.CreateOne(primaryIndexModel);

            var indexModel2 = new CreateIndexModel<Repository>(RepositoryModelBuilder.Ascending(i => i.secondaryIndex), new CreateIndexOptions() { Name = "secondaryIndex" });
            collection.Indexes.CreateOne(indexModel2);

            var indexModel3 = new CreateIndexModel<Repository>(RepositoryModelBuilder.Ascending(i => i.tertiaryIndex), new CreateIndexOptions() { Name = "tertiaryIndex" });
            collection.Indexes.CreateOne(indexModel3);
        }

    }
}
