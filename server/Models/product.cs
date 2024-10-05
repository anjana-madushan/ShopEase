using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace server.Models;

public class Product
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }

  [BsonElement("Name")]
  [Required(ErrorMessage = "Product name is required")]
  public string ProductName { get; set; } = null!;

  [Range(1.00, double.MaxValue, ErrorMessage = "Price must be greater than 1")]
  public decimal Price { get; set; }

  [Required(ErrorMessage = "Category is required")]
  public string Category { get; set; } = null!;

  public string Description { get; set; } = null!;
}