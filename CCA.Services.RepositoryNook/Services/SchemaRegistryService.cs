using CCA.Services.RepositoryNook.Config;
using CCA.Services.RepositoryNook.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Services
{
    public class SchemaRegistryService : ISchemaRegistryService
    {
        private IJsonConfiguration _config;
        private readonly string DATABASE_NAME = "repository-nook-db";
        private readonly string SCHEMA_REGISTRY_COLLECTION = "schema-registry";

        public SchemaRegistryService(IJsonConfiguration config)     // ctor
        {
            _config = config;
        }

        public SchemaRegistry Create(SchemaRegistry schemaRegistry)
        {
            var client = new MongoClient(_config.AtlasMongoConnection);
            var database = client.GetDatabase(DATABASE_NAME);
            IMongoCollection<SchemaRegistry> srCollection = database.GetCollection<SchemaRegistry>(SCHEMA_REGISTRY_COLLECTION);

            if (schemaRegistry._id == null)                         // user can send in a unique identifier, else we generate a mongo ObjectId (mongo unique id)
            {
                schemaRegistry._id = ObjectId.GenerateNewId();
            }
            if (schemaRegistry.createdDate == DateTime.MinValue)   // user can send in a creation date, else we insert now()
            {
                schemaRegistry.createdDate = DateTime.Now;
            }

            CreateSchemaNameIndex(srCollection);                  // primary index is schema name, must be unique

            srCollection.InsertOne(schemaRegistry);

            return schemaRegistry;
        }
        private void CreateSchemaNameIndex(IMongoCollection<SchemaRegistry> collection)
        {
                var modelBuilder = Builders<SchemaRegistry>.IndexKeys;
                var indexModel = new CreateIndexModel<SchemaRegistry>(modelBuilder.Ascending(i => i.schemaName), new CreateIndexOptions() { Name = "PK_schemaName", Unique = true });

                collection.Indexes.CreateOne(indexModel);       //indempotent
            }
    }
}
