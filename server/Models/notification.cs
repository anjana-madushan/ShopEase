using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace server.Models;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("message")]
    public string Message{ get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("read")]
    public bool Read { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; }

}