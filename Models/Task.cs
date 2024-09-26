using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Task
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("dueDate")]
        public DateTime DueDate { get; set; }

        [BsonElement("priority")]
        public Priority Priority { get; set; }

        [BsonElement("status")]
        public Status Status { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [NotMapped]
        public bool CanRepeat { get; set; }
        [NotMapped]
        public RepeatType RepeatType { get; set; }
    }

    public class Task1
    {        
        public string Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public Priority Priority { get; set; }

        public Status Status { get; set; }

        public string UserId { get; set; }

       
        public bool CanRepeat { get; set; }
       
        public RepeatType RepeatType { get; set; }
    }

    public enum RepeatType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public enum Priority
    {
        High=0,
        Medium=1,
        Low=2
    }

    public enum Status
    {
        None = -1,
        Pending =0,
        InProgress=1,
        Completed=2
    }
}
