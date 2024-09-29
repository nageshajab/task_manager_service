using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Rent
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userid")]
        public string UserId { get; set; }  

        [BsonElement("tenant")]
        public string Tenant { get; set; }

        [BsonElement("amount")]
        public string Amount{ get; set; }

        [BsonElement("date")]
        public DateTime Date{ get; set; }

    }

    public class Rent1
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Tenant { get; set; }
        public string Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class RentSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class RentIndexViewModel
    {
        public List<Rent> Rents{ get; set; }
        public RentSearch  RentSearch{ get; set; }
        public int TotalPages { get; set; }
    }
}
