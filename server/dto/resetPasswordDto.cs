using System.Collections.Generic;
using api.Dtos.Account;

namespace server.DTOs
{
    public class ResetPasswordDto
    {
        public string Password { get; set; }

        public string Code { get; set; }

    }

}
