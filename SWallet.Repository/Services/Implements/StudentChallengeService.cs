using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.StudentChallenge;
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

        public async Task<bool> CompleteChallenge(string challengeId, string studentId)
        {
            var challenge = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(predicate: x => x.Id == challengeId);
            if (challenge == null) return false;

            var studentChallenge = await _unitOfWork.GetRepository<StudentChallenge>().SingleOrDefaultAsync(
                predicate: sc => sc.ChallengeId == challengeId && sc.StudentId == studentId);

            if (studentChallenge == null)
            {
                studentChallenge = new StudentChallenge
                {
                    ChallengeId = challengeId,
                    StudentId = studentId,
                    IsCompleted = false,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now // last reset
                };
                await _unitOfWork.GetRepository<StudentChallenge>().InsertAsync(studentChallenge);
            }

            // Xử lý hoàn thành Daily Challenge
            if (challenge.Type == (int)ChallengeType.Daily) // Daily Challenge
            {
                if ((bool)!studentChallenge.IsCompleted)
                {
                    studentChallenge.IsCompleted = true;
                    studentChallenge.DateUpdated = DateTime.Now;
                    //studentChallenge.LastReset = DateTime.Now;
                    //await GrantRewardAsync(studentId, challenge.Reward);
                }
            }
            // Xử lý hoàn thành Achievement Challenge
            else if (challenge.Type == (int)ChallengeType.Achievement) // Achievement Challenge
            {
                if (studentChallenge.Current < challenge.Condition)
                {
                    studentChallenge.Current++;
                    studentChallenge.DateUpdated = DateTime.Now;

                    // Kiểm tra hoàn thành milestone cuối
                    if (studentChallenge.Current >= challenge.Condition)
                    {
                        studentChallenge.IsCompleted = true;
                        //await IncreaseUserLevelAsync(studentId);
                    }

                    //await GrantRewardAsync(studentId, challenge.Reward);
                }
            }

            _unitOfWork.GetRepository<StudentChallenge>().UpdateAsync(studentChallenge);

            var result = await _unitOfWork.CommitAsync() > 0;
            return result;
        }

        public async Task<bool> CreateStudentChallenge(StudentChallengeRequest studentChallengeRequest)
        {
            var studentChallenge = new StudentChallenge
            {
                ChallengeId = studentChallengeRequest.ChallengeId,
                StudentId = GetStudentIdFromJwt(),
                IsCompleted = false,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Current = 0,
                DateCompleted = null,
                Condition = studentChallengeRequest.Condition,
                Description = studentChallengeRequest.Description,
                Amount = studentChallengeRequest.Amount,
                Status = true,
            };

            if (studentChallenge.StudentId.IsNullOrEmpty())
            {
                throw new ApiException("StudentId null", 400, "CREATE_STUDENT_CHALLENGE_FAILED");
            }

            await _unitOfWork.GetRepository<StudentChallenge>().InsertAsync(studentChallenge);
            var result = await _unitOfWork.CommitAsync() > 0;
            if(result)
            {
                return true;
            }
            throw new ApiException("Create student challenge failed", 400, "CREATE_STUDENT_CHALLENGE_FAILED");
        }

        public Task<bool> GrantReward(string challengeId, string studentId)
        {
            throw new NotImplementedException();
        }
    }
}
