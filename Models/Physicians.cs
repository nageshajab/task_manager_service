using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models
{
    public class Physicians
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement]
        public string Specialty { get; set; }=string.Empty;

        [BsonElement]
        public int Distance { get; set; }

        [BsonElement]
        public string City { get; set; }=string.Empty;

        [BsonElement]
        public string Address { get; set; }=string.Empty;

    }
}
