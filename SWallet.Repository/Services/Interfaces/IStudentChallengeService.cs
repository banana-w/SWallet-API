using SWallet.Repository.Payload.Request.StudentChallenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IStudentChallengeService
    {
        Task<bool> CreateStudentChallenge(StudentChallengeRequest studentChallengeRequest);
        Task<bool> CompleteChallenge(string challengeId, string studentId);
        Task<bool> GrantReward(string challengeId, string studentId);


    }
}
