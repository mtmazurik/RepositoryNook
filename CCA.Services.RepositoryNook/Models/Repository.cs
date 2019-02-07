using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CCA.Services.RepositoryNook.Models
{

    public class Repository
    {
        [BsonRequired]
        [BsonElement("_id")]
        public object _id { get; set; }
        [BsonRequired]
        [BsonElement("keyName")]
        public string keyName { get; set; }
        [BsonElement("keyValue")]
        public string keyValue { get; set; }
        [BsonElement("tags")]
        public IEnumerable<NameValuePair> tags { get; set; }
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
        [BsonElement("repository")]
        public string repository { get; set; }
        [BsonElement("collection")]
        public string collection { get; set; }
        [BsonElement("schemaName")]
        public string schemaName { get; set; }
        [BsonElement("schemaUri")]
        public string schemaUri { get; set; }
        [BsonElement("body")]
        public string body { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class NameValuePair
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("value")]
        public string Value { get; set; }
    }
}
