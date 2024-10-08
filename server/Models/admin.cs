using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace server.Models
{
    public class Admin
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

        [BsonElement("adminsCreated")]
        public List<Admin> AdminsCreated { get; set; } = new List<Admin>();

        [BsonElement("CSRsCreated")]
        public List<CSR> CSRCreated { get; set; } = new List<CSR>();

        [BsonElement("CustomersApproved")]
        public List<Users> CustomerApproved { get; set; } = new List<Users>();

        [BsonElement("VendorCreated")]
        public List<Vendor> VendorCreated { get; set; } = new List<Vendor>();  
    }
}
