using Microsoft.AspNetCore.Http;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Store;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Response.Account;


namespace SWallet.Repository.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateStudentAccount(AccountRequest accountRequest, StudentRequest studentRequest);
        Task<AccountResponse> GetAccountById(string id);
        Task<AccountResponse> CreateBrandAccount(AccountRequest accountRequest, CreateBrandByAccountId brandRequest);
        Task<AccountResponse> CreateStoreAccount(AccountRequest accountRequest, CreateStoreModel storeRequest);
        Task<AccountResponse> CreateCampusAccount(AccountRequest accountRequest, string campusId);
        Task<AccountResponse> CreateLecturerAccount(AccountRequest accountRequest, CreateLecturerModel lecturerReq, string campusId);
        Task<AccountResponse> UpdateAccount(string id, string phone, string email, string oldPassword, string newPassword);
        Task<AccountResponse> UpdateAccountAvatar(string id, IFormFile avatar);
        Task<bool> ValidUsername(string username);
        Task<bool> ValidEmail(string email);
    }
}
