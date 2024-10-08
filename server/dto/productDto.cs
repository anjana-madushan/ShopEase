namespace server.DTOs
{
  public class ProductDTO
  {
    public string? VenderId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string? Image { get; set; }
    public bool IsCategoryActive { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int StockLevel { get; set; }
    public int MinStockLevel { get; set; }
  }
}
