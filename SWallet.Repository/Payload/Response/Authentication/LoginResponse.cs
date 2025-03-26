using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Authentication
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? AccountId { get; set; }
        public bool? IsVerify { get; set; }
        public string? Email { get; set; }
    }
}
