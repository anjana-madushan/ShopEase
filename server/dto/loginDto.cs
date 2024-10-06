using System.Collections.Generic;

namespace server.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

}
