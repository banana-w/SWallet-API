using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Response.Login
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? AccountId { get; set; }
    }
}
