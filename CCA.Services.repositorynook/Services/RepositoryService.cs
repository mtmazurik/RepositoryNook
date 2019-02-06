﻿using Microsoft.AspNetCore.Hosting;
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

        public async Task<Repository> Create(Repository repoObject)
        {
            var client = new MongoClient(_config.AtlasMongoConnection);
            var database = client.GetDatabase(repoObject.repository);
            IMongoCollection<Repository> repositoryCollection = database.GetCollection<Repository>(repoObject.collection);

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

        // Indempotent: is a no-op if already exists.
        private void CreateRepositoryTextIndices(IMongoCollection<Repository> collection)
        {
            // text search field     .Text()   is the keyValue
            var textKey = Builders<Repository>.IndexKeys.Text( t => t.keyValue);       // the key value, is collections text search field, and is highly queryable
            var options = new CreateIndexOptions() { Name = "IX_keyValue}" }; 
            collection.Indexes.CreateOne( textKey, options );

            // another indexed field   is the  keyName
            var indexKey = Builders<Repository>.IndexKeys.Ascending( i => i.keyName);   // the key name, is text and is indexed for speedier queries
            var ix_options = new CreateIndexOptions() { Name = "IX_keyName}" };
            collection.Indexes.CreateOne(indexKey, ix_options);

            // finally the tags are madesearchable
            var tagsKey = Builders<Repository>.IndexKeys.Ascending(t => t.tags);        // the tags array
            var tags_ix_options = new CreateIndexOptions() { Name = "IX_tags}" };
            collection.Indexes.CreateOne(tagsKey, tags_ix_options);
        }

    }
}
