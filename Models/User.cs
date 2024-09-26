using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 8 and 15 characters and contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character.")]
        public string PasswordHash { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("roles")]
        public List<string > Roles { get; set; }
    }
}
