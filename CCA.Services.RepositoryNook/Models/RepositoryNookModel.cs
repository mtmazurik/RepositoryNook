using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CCA.Services.RepositoryNook.Models
{

    public class RepositoryNookModel
    {
        [BsonRequired]
        [BsonElement("_id")]
        public object _id { get; set; }
        [BsonRequired]
        [BsonElement("primaryIndex")]
        public string primaryIndex { get; set; }
        [BsonElement("secondaryIndex")]
        public string secondaryIndex { get; set; }
        [BsonElement("tertiaryIndex")]
        public string tertiaryIndex { get; set; }
        [BsonElement("tags")]
        public List<string> tags { get; set; }
        [BsonElement("createdDate")]
        public DateTime createdDate { get; set; }
        [BsonElement("createdBy")]
        public string createdBy { get; set; }
        [BsonElement("modifiedDate")]
        public object modifiedDate { get; set; }
        [BsonElement("modifiedBy")]
        public string modifiedBy { get; set; }
        [BsonElement("app")]
        public string app { get; set; }
        [BsonElement("schemaName")]
        public string schemaName { get; set; }
        [BsonElement("schemaUri")]
        public string schemUri { get; set; }
        [BsonElement("objectType")]
        public string objectType { get; set; }
        [BsonElement("objectData")]
        public string objectData { get; set; }
    }
}
