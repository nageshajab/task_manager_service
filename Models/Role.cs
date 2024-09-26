using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.Models
{
    public class Role
    {
        [BsonId]
        public ObjectId Id { get;set; }
        [BsonElement("name")]
        public string Name{ get; set; }
    }

    public class RoleSearch
    {
        public string Name { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public string SortBy { get; set; }
    }
}
