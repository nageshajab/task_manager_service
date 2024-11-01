using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models
{
    public class Patient
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement]
        public string City { get; set; }=string.Empty;

        [BsonElement]
        public string Address { get; set; }=string.Empty;

        [BsonElement]
        public required string[] InsuranceCoverage { get; set; }

        [BsonElement]
        public int therapyVisitsRemaining { get; set; }

        [BsonElement]
        public int totalTherapyVisits { get; set; }

    }
}
