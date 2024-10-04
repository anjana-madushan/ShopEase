using System.Collections.Generic;
using api.Dtos.Account;

namespace server.DTOs
{
    public class AdminDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<AdminDTO>? AdminsCreated { get; set; } = new List<AdminDTO>();
        public List<CSRDTO>? CSRCreated { get; set; } = new List<CSRDTO>();
        public List<VendorDTO>? VendorCreated { get; set; } = new List<VendorDTO>();
    }

}
