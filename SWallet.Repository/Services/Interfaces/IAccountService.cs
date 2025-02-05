using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Login;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Payload.Response.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateStudentAccount(CreateStudentAccount accountCreation);
        Task<AccountResponse> GetAccountById(string id);
    }
}
