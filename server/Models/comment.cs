using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
namespace server.Models;

public class Comments
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }
  public required string CustomerId { get; set; }

  public required string VendorId { get; set; }
  public string? Comment { get; set; } = null!;

  [Range(0, double.MaxValue, ErrorMessage = "Rating must be greater than 0")]
  public int? Rating { get; set; } = 0;
}
