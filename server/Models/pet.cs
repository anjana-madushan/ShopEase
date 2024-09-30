// using MongoDB.Bson;
// using MongoDB.Bson.Serialization.Attributes;

// namespace server.Models;

// public class Pet
// {
//   [BsonId]
//   [BsonRepresentation(BsonType.ObjectId)]
//   public string? Id { get; set; }

//   [BsonElement("Name")]
//   public string PetName { get; set; } = null!;

//   public string Type { get; set; } = null!; // e.g., Dog, Cat, etc.
//   public decimal Age { get; set; } // Age in years
//   public string Owner { get; set; } = null!; // Owner's name
// }
