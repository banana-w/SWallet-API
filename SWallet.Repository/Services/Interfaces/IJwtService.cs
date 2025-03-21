using SWallet.Repository.Payload.Response.Account;


namespace SWallet.Repository.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(AccountResponse account, Tuple<string, string> guidClaim);
    }
}
