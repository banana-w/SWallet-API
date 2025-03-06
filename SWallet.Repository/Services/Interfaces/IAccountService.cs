using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Response.Account;


namespace SWallet.Repository.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateStudentAccount(AccountRequest accountRequest, StudentRequest studentRequest);
        Task<AccountResponse> GetAccountById(string id);
        Task<AccountResponse> CreateBrandAccount(AccountRequest accountRequest, CreateBrandByAccountId brandRequest);
    }
}
