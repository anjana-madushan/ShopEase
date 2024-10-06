using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace server.Models;

public class Users
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

    [BsonElement("approvalStatus")]
    public bool ApprovalStatus { get; set; }

    [BsonElement("approvedBy")]
    public string ApprovedBy { get; set; }

    [BsonElement("Deactivated")]
    public bool Deactivated { get; set; }

    [BsonElement("deactivatedBy")]
    public string DeactivatedBy { get; set; }

      [BsonElement("reactivatedBy")]
    public string ReactivatedBy { get; set; }


}