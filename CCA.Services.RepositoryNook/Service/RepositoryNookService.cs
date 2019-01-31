using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCA.Services.RepositoryNook.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CCA.Services.RepositoryNook.Service
{
    public class RepositoryNookService : IRepositoryNookService
    {
        private IApplicationLifetime _applicationLifetime;
        //private readonly IMongoCollection<RepositoryModel> _repository;

        public RepositoryNookService(IApplicationLifetime applicationLifetime)     // ctor
        {
            _applicationLifetime = applicationLifetime;

        } 
        public string kill()
        {
            _applicationLifetime.StopApplication();
            return "RepositoryNook service stopped.";
        }
        public Repository Create(Repository repoObject)
        {
            var client = new MongoClient("mongodb+srv://nook-service:Passw0rd!@mongo-db-cluster-sdzbh.mongodb.net/");
            var database = client.GetDatabase("repository-nook-db");
            IMongoCollection<Repository> collection = database.GetCollection<Repository>("repository");

            if (repoObject._id == null)                         // user can send in a unique identifier, else we generate a guid
            { 
                repoObject._id = ObjectId.GenerateNewId();
            }
            if (repoObject.createdDate == DateTime.MinValue)   // user can send in a creation date, else we insert now()
            { 
                repoObject.createdDate = DateTime.Now;
            }

            CreateIndices(collection, repoObject);

            collection.InsertOne(repoObject);
            return repoObject;
        }
        void CreateIndices(IMongoCollection<Repository> collection, Repository repoObject)
        {
            // TO-DO: implement check for number of indices, although they are indempotent , reduce the call overhead,   if indices.count >=  3  we've created them, so exit rtn

            var RepositoryModelBuilder = Builders<Repository>.IndexKeys;

                                                                // primary index must be supplied, not necessarily unique
            var primaryIndexModel = new CreateIndexModel<Repository>(RepositoryModelBuilder.Ascending(i => i.primaryIndex)
                        , new CreateIndexOptions() { Name = "primaryIndex" });
            collection.Indexes.CreateOne(primaryIndexModel);

            if (repoObject.secondaryIndex != null)  // because optional:   as soon as once is provided we create index for it
            {
                var indexModel = new CreateIndexModel<Repository>(RepositoryModelBuilder.Ascending(i => i.secondaryIndex)
                                        , new CreateIndexOptions() { Name = "secondaryIndex" });
                collection.Indexes.CreateOne(indexModel);
            }
            if (repoObject.tertiaryIndex != null)  // because optional:   create index, if supplied
            {
                var indexModel = new CreateIndexModel<Repository>(RepositoryModelBuilder.Ascending(i => i.tertiaryIndex)
                                        , new CreateIndexOptions() { Name = "tertiaryIndex" });
                collection.Indexes.CreateOne(indexModel);
            }

        }

    }
}
