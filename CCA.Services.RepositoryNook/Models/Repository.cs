using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCA.Services.RepositoryNook.Models
{
    [BsonIgnoreExtraElements]
    public class Repository
    {
        [BsonRequired]
        [BsonElement("_id")]
        public object _id { get; set; }
        [BsonRequired]
        [BsonElement("key")]
        public string key { get; set; }
        [BsonElement("tags")]
        public IEnumerable<string> tags { get; set; }
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
        [BsonElement("validate")]
        public bool validate { get; set; }
        [BsonElement("schemaUri")]
        public string schemaUri { get; set; }
        [BsonElement("data")]
        public string data { get; set; }
    }
}
