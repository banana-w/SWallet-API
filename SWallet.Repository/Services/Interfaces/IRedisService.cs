using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IRedisService
    {
        Task SaveVerificationCodeAsync(string email, string code);
        Task<bool> VerifyCodeAsync(string email, string userInput);
    }
}
