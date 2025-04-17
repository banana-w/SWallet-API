using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Payload.Request;
using SWallet.Repository.Payload.Response.Challenge;


namespace SWallet.Repository.Services.Interfaces
{
    public interface IChallengeService
    {
        Task<bool> CreateChallenge(ChallengeRequest request);
        Task<IPaginate<ChallengeResponse>> GetChallenges(string? search, IEnumerable<ChallengeType> types, int page, int size);
        Task<IPaginate<ChallengeResponseExtra>> GetStudentChallenges(string studentId,
                string? search,
                IEnumerable<ChallengeType> types,
                int page,
                int size);
        Task<ChallengeResponse> GetChallenge(string id);
        Task<bool> UpdateChallenge(string id, ChallengeRequest request);
        Task<bool> AssignChallengeToStudent(string challengeId, string studentId);
        Task<bool> AssignAllChallengesToStudent(string studentId);
        Task<bool> UpdateAchievementProgress(string studentId, IEnumerable<Challenge> challenges, decimal amount);
        Task<bool> RecordDailyTaskAction(string studentId, string challengeId, decimal amount);
        Task<bool> RewardStudent(string studentId, string challengeId, int type);
        Task<bool> AddChallengeTransaction(ChallengeTransaction transaction, int type);
        Task<bool> CheckProgress(string studentId, string challengeId, decimal newAmount);
        //Task<bool> DeleteChallenge(string id);
    }
}
