using SWallet.Repository.Payload.Response.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(AccountResponse account);
    }
}
