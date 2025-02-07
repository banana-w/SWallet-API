using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Response.Account;


namespace SWallet.Repository.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateStudentAccount(CreateStudentAccount accountCreation);
        Task<AccountResponse> GetAccountById(string id);
    }
}
