namespace server.Models;

public class MongoDBConfig
{

  public string MongoURI { get; set; } = null!;
  public string DbName { get; set; } = null!;
  public string MongoProductCollection { get; set; } = null!;
  public string MongoCommentCollection { get; set; } = null!;
  public string MongoAdminCollection { get; set; } = null!;
  public string MongoOrderCollection { get; set; } = null!;
  public string MongoNotificationCollection { get; set; } = null!;
  public string MongoCSRCollection { get; set; } = null!;
  public string MongoCustomerCollection { get; set; } = null!;
  public string MongoVendorCollection { get; set; } = null!;
}