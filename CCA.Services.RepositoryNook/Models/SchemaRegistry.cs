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
        [BsonElement("schemaOrg")]
        public string schemaOrg { get; set; }
        [BsonElement("schemaUri")]
        public string schemaUri { get; set; }
        [BsonElement("schemaName")]
        public string schemaName{ get; set; }
        [BsonElement("description")]
        public string description { get; set; }
        [BsonElement("type")]
        public string type { get; set; }
        [BsonElement("createdDate")]
        public DateTime createdDate { get; set; }
        [BsonElement("createdBy")]
        public string createdBy { get; set; }
        [BsonElement("modifiedDate")]
        public DateTime modifiedDate { get; set; }
        [BsonElement("modifiedBy")]
        public string modifiedBy { get; set; }
        [BsonElement("app")]
        public string app { get; set; }
        [BsonElement("schemaBody")]
        public string schemaBody { get; set; }
    }
}
