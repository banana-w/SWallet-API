using SWallet.Repository.Payload.Request.Authentication;
using SWallet.Repository.Payload.Response.Authentication;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task<bool> VerifyEmail(string email, string userInput);
    }
}
