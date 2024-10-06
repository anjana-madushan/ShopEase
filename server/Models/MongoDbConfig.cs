namespace server.Models;

public class MongoDBConfig
{

  public string MongoURI { get; set; } = null!;
  public string DbName { get; set; } = null!;
  public string MongoProductCollection { get; set; } = null!;
  public string MongoCommentCollection { get; set; } = null!;
}