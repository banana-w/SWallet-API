using SWallet.Repository.Payload.Request.Login;
using SWallet.Repository.Payload.Response.Login;


namespace SWallet.Repository.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
    }
}
