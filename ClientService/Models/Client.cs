using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientService.Models
{
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }      // ✅ required

        public required string FullName { get; set; } // ✅ required

        public required string Email { get; set; }    // ✅ required

        public required string Phone { get; set; }    // ✅ required
    }
}