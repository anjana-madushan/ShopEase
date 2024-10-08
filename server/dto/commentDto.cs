
namespace server.DTOs;

public class CommentDto
{
  public string? Id { get; set; }
  // public string? CustomerId { get; set; }
  // public string? VendorId { get; set; }
  public required string Comment { get; set; } = null!;
  public int? Rating { get; set; } = 0;
}
