using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Movie
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("genre")]
        public Genre Genre { get; set; }

        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("language")]
        public Langauge Language { get; set; } 

        [BsonElement("actors")]
        public List<string> Actors { get; set; }

        [BsonElement("userId")]
        public required string UserId { get; set; }
    }
    public class Movie1
    {
        public string Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Genre Genre { get; set; }

        public double Rating { get; set; }

        public string Description { get; set; } = string.Empty;

        public Langauge Language { get; set; } 

        public string Actors { get; set; } 

        public string UserId { get; set; }
    }
 
    public enum Langauge
    {
        Hindi=0,
        English = 1,
        Tamil = 2,
        Telugu = 3,
        Malayalam = 4,
        Kannada = 5,
        Marathi = 6,

    }

    public enum Genre
    {
        Suspense = 0,
        Action = 1,
        Comedy = 2,
        Romance = 3,
        Drama = 4,
        Horror = 5,
        SciFi = 6,
        Documentary = 7,
        Animation = 8,
    }

    public class MovieSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class MovieIndexViewModel
    {
        public List<TaskManager.Models.Movie>? ListOfMovies { get; set; }
        public MovieSearch? MovieSearch { get; set; }
        public int TotalPages { get; set; }
    }
}
