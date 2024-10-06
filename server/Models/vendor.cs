using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace server.Models;

public class Vendor
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }

  [BsonElement("username")]
  public string Username { get; set; }

  [BsonElement("password")]
  public string Password { get; set; }

  [BsonElement("email")]
  public string Email { get; set; }
}