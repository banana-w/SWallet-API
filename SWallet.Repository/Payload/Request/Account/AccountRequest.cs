using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Payload.Request.Account
{
    public class AccountRequest
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

    }
}
