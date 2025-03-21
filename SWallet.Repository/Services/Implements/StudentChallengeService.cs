using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class StudentChallengeService : BaseService<StudentChallengeService>, IStudentChallengeService
    {
        public StudentChallengeService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<StudentChallengeService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
        {
        }

        public Task<bool> CompleteChallenge(string challengeId, string studentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateStudentChallenge(string studentId, string challengeId)
        {
            throw new NotImplementedException();
        }
    }
}
