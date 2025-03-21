using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IStudentChallengeService
    {
        Task<bool> CreateStudentChallenge(string studentId, string challengeId);
        Task<bool> CompleteChallenge(string challengeId, string studentId);


    }
}
