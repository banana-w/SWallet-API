using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Payload.Request;
using SWallet.Repository.Payload.Response.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IChallengeService
    {
        Task<bool> CreateChallenge(ChallengeRequest request);
        Task<IPaginate<ChallengeResponse>> GetChallenges(string? search, IEnumerable<ChallengeType> types, int page, int size);
        Task<ChallengeResponse> GetChallenge(string id);
        Task<bool> UpdateChallenge(string id, ChallengeRequest request);
        //Task<bool> DeleteChallenge(string id);
    }
}
