using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CCA.Services.RepositoryNook.Models
{

    public class SchemaRegistry
    {
        [BsonRequired]
        [BsonElement("_id")]
        public object _id { get; set; }
        [BsonRequired]
        [BsonElement("$schema")]
        public string schema { get; set; }
        [BsonElement("$id")]
        public string id { get; set; }
        [BsonElement("title")]
        public string title{ get; set; }
        [BsonElement("description")]
        public string description { get; set; }
        [BsonElement("type")]
        public string type { get; set; }
        [BsonElement("createdDate")]
        public DateTime createdDate { get; set; }
        [BsonElement("createdBy")]
        public string createdBy { get; set; }
        [BsonElement("modifiedDate")]
        public object modifiedDate { get; set; }
        [BsonElement("modifiedBy")]
        public string modifiedBy { get; set; }
        [BsonElement("app")]
        public string objectType { get; set; }
        [BsonElement("data")]
        public string data { get; set; }
    }
}
