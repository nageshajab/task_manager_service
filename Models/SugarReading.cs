using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class SugarReading
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        [BsonElement("userid")]
        public string UserId { get; set; }

        [BsonElement("fasting")]
        public string Fasting { get; set; }

        [BsonElement("pp")]
        public string PP { get; set; }

        [BsonElement("date")]
        public string Date { get; set; }

        [BsonElement("weight")]
        public int Weight { get; set; }

        [BsonElement("medicines")]
        public string[] Medicines { get; set; }
    }

    public class SugarReading1
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Fasting { get; set; }
        public string PP { get; set; }
        public string Date { get; set; }
        public int Weight { get; set; }
        public string[] Medicines { get; set; }
    }

    public class SugarReadingSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class SugarReadingIndexViewModel
    {
        public List<SugarReading> ListOfSugarReadings { get; set; }
        public SugarReadingSearch SugarReadingSearch{ get; set; }
        public int TotalPages { get; set; }
    }
}
