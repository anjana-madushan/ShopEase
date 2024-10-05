public class StockEntry
{
  public string StockEntryId { get; set; }
  public string ProductId { get; set; }
  public int Quantity { get; set; }
  public DateTime DateAdded { get; set; }
  public string Status { get; set; }
}