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
        //private readonly IMongoCollection<RepositoryNookModel> _repository;

        public RepositoryNookService(IApplicationLifetime applicationLifetime)     // ctor
        {
            _applicationLifetime = applicationLifetime;

        } 
        public string kill()
        {
            _applicationLifetime.StopApplication();
            return "RepositoryNook service stopped.";
        }
        public void Create(RepositoryNookModel nookObject)
        {
            var client = new MongoClient("mongodb+srv://nook-service:Passw0rd!@mongo-db-cluster-sdzbh.mongodb.net/");
            var database = client.GetDatabase("repository-nook-db");
            IMongoCollection<RepositoryNookModel> repository = database.GetCollection<RepositoryNookModel>("repository");

            if (nookObject._id == null)                         // user can send in a unique identifier, else we generate a guid
            { 
                nookObject._id = ObjectId.GenerateNewId();
            }
            if (nookObject.createdDate == DateTime.MinValue)   // user can send in a creation date, else we insert now()
            { 
                nookObject.createdDate = DateTime.Now;
            }

            CreateIndices(repository, nookObject);

            repository.InsertOne(nookObject);
        }
        void CreateIndices(IMongoCollection<RepositoryNookModel> repository, RepositoryNookModel nookObject)
        {
            // TO-DO: implement check for number of indices, although they are indempotent , reduce the call overhead,   if indices.count >=  3  we've created them, so exit rtn

            var repositoryNookModelBuilder = Builders<RepositoryNookModel>.IndexKeys;

                                                                // primary index must be supplied, not necessarily unique
            var primaryIndexModel = new CreateIndexModel<RepositoryNookModel>(repositoryNookModelBuilder.Ascending(i => i.primaryIndex)
                        , new CreateIndexOptions() { Name = "primaryIndex" });
            repository.Indexes.CreateOne(primaryIndexModel);

            if (nookObject.secondaryIndex != null)  // because optional:   as soon as once is provided we create index for it
            {
                var indexModel = new CreateIndexModel<RepositoryNookModel>(repositoryNookModelBuilder.Ascending(i => i.secondaryIndex)
                                        , new CreateIndexOptions() { Name = "secondaryIndex" });
                repository.Indexes.CreateOne(indexModel);
            }
            if (nookObject.tertiaryIndex != null)  // because optional:   create index, if supplied
            {
                var indexModel = new CreateIndexModel<RepositoryNookModel>(repositoryNookModelBuilder.Ascending(i => i.tertiaryIndex)
                                        , new CreateIndexOptions() { Name = "tertiaryIndex" });
                repository.Indexes.CreateOne(indexModel);
            }

        }

    }
}
