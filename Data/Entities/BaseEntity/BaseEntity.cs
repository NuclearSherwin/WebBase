using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.BaseEntity
{
    [Serializable]
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreateAt = DateTime.Now;
        }
        
        [BsonId] 
        public string Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string CreatedBy { get; set; }
        [BsonIgnoreIfNull]
        public DateTime? UpdateAt { get; set; }
        [BsonIgnoreIfNull]
        public string UpdateBy { get; set; }

        public bool IsDeleted { get; set; }
        [BsonIgnoreIfNull]
        public DateTime? DeleteAt { get; set; }
        [BsonIgnoreIfNull]
        public string DeleteBy { get; set; }
        [BsonExtraElements]
        [JsonIgnore]
        public Dictionary<string, object> ExtraElements { get; set; }
        
    }
}