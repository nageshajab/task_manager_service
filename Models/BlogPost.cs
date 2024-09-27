using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class BlogPost
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userid")]
        public string UserId { get; set; }  

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("repositoryurls")]
        public string[] RepositoryUrls { get; set; }

        [BsonElement("blogposturl")]
        public string BlogPostUrl{ get; set; }

        [BsonElement("tags")]
        public string[] Tags { get; set; }
    }

    public class BlogPost1
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] RepositoryUrls { get; set; }
        public string BlogPostUrl { get; set; }
        public string[] Tags { get; set; }
    }

    public class BlogPostSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class BlogPostIndexViewModel
    {
        public List<BlogPost> ListOfBlogPosts{ get; set; }
        public BlogPostSearch BlogPostSearch { get; set; }
        public int TotalPages { get; set; }
    }
}
