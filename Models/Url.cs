using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Models
{
    public class Url
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userid")]
        public string UserId { get; set; }

        [BsonElement("link")]
        public string Link{ get; set; }

        [BsonElement("actress")]
        public string? Actress { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("tags")]
        public string[]? Tags { get; set; }
    }

    public class Url1
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Link { get; set; }
        public string Actress { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
    }

    public class UrlSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public string[] Tags { get; set; }
    }

    public class UrlIndexViewModel
    {
        public List<Url> ListOfUrls { get; set; }
        public List<string> Tags { get; set; }
        public UrlSearch UrlSearch { get; set; }

        public int TotalPages { get; set; }
    }
}
