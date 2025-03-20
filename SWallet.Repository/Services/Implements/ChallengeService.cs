using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request;
using SWallet.Repository.Payload.Response.Challenge;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class ChallengeService : BaseService<ChallengeService>, IChallengeService
    {
        private readonly ICloudinaryService _cloudinaryService;
        public ChallengeService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<ChallengeService> logger, ICloudinaryService cloudinaryService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<bool> CreateChallenge(ChallengeRequest request)
        {

            var challenge = new Challenge
            {
                Id = Ulid.NewUlid().ToString(),
                ChallengeName = request.ChallengeName,
                Description = request.Description,
                Type = request.Type,
                Image = "NUll",
                FileName = "NUll",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Status = true
            };
            // Add challenge to the database
            await _unitOfWork.GetRepository<Challenge>().InsertAsync(challenge);

            if (request.Image != null && request.Image.Length > 0)
            {
                var image = await _cloudinaryService.UploadImageAsync(request.Image);
                if (image != null)
                {
                    challenge.Image = image.SecureUrl.AbsoluteUri;
                    challenge.FileName = image.PublicId;
                }
            }

            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Create challenge fail", 400, "CHALLENGE_FAIL");
        }

        //public async Task<bool> CompleteChallengeAsync(string challengeId, string studentId)
        //{
        //    var challenge = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(predicate: x => x.Id == challengeId);
        //    if (challenge == null) return false;

        //    var studentChallenge = await _unitOfWork.GetRepository<StudentChallenge>().SingleOrDefaultAsync(
        //        predicate: sc => sc.ChallengeId == challengeId && sc.StudentId == studentId);

        //    // Nếu chưa có, tạo mới
        //    if (studentChallenge == null)
        //    {
        //        studentChallenge = new StudentChallenge
        //        {
        //            ChallengeId = challengeId,
        //            StudentId = studentId,
        //            IsCompleted = false,
        //            DateCreated = DateTime.Now,
        //            DateUpdated = DateTime.Now // last reset
        //        };
        //        await _unitOfWork.GetRepository<StudentChallenge>().InsertAsync(studentChallenge);
        //    }

        //    // Xử lý hoàn thành Daily Challenge
        //    if (challenge.Type == (int)ChallengeType.Daily) // Daily Challenge
        //    {
        //        if ((bool)!studentChallenge.IsCompleted)
        //        {
        //            studentChallenge.IsCompleted = true;
        //            studentChallenge.DateUpdated = DateTime.Now;
        //            //studentChallenge.LastReset = DateTime.Now;
        //            //await GrantRewardAsync(studentId, challenge.Reward);
        //        }
        //    }
        //    // Xử lý hoàn thành Achievement Challenge
        //    else if (challenge.Type == (int)ChallengeType.Achievement) // Achievement Challenge
        //    {
        //        if (studentChallenge.Current < challenge.MaxMilestone)
        //        {
        //            studentChallenge.CurrentMilestone++;
        //            studentChallenge.DateUpdated = DateTime.Now;

        //            // Kiểm tra hoàn thành milestone cuối
        //            if (studentChallenge.CurrentMilestone == challenge.MaxMilestone)
        //            {
        //                studentChallenge.IsCompleted = true;
        //                await IncreaseUserLevelAsync(studentId);
        //            }

        //            await GrantRewardAsync(studentId, challenge.Reward);
        //        }
        //    }

        //    _context.StudentChallenges.Update(studentChallenge);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public Task<ChallengeResponse> GetChallenge(string id)
        {
            var challenge = _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(
                selector: x => new ChallengeResponse
                {
                    Id = x.Id,
                    ChallengeName = x.ChallengeName,
                    Description = x.Description,
                    Type = x.Type,
                    Image = x.Image,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Status = x.Status
                },
                predicate: x => x.Id.Equals(id));
            if (challenge == null)
            {
                throw new ApiException("Challenge not found", 404, "CHALLENGE_NOT_FOUND");
            }
            return challenge;
        }

        public Task<IPaginate<ChallengeResponse>> GetChallenges(string? search, IEnumerable<ChallengeType> types, int page, int size)
        {
            Expression<Func<Challenge, bool>> filter = x => x.Status == true &&
                 (string.IsNullOrEmpty(search) || x.ChallengeName.Contains(search)) &&
                 (!types.Any() || types.Contains((ChallengeType)x.Type));

            var challenges = _unitOfWork.GetRepository<Challenge>().GetPagingListAsync(
                selector: x => new ChallengeResponse
                {
                    Id = x.Id,
                    ChallengeName = x.ChallengeName,
                    Description = x.Description,
                    Type = x.Type,
                    Image = x.Image,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Status = x.Status
                },
                predicate: filter,
                page: page,
                size: size);
            return challenges;
        }

        public async Task<bool> UpdateChallenge(string id, ChallengeRequest request)
        {
            var challenge = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(predicate: x => x.Equals(id));
            if (challenge == null)
            {
                throw new ApiException("Challenge not found", 404, "CHALLENGE_NOT_FOUND");
            }

            challenge.ChallengeName = request.ChallengeName;
            challenge.Description = request.Description;
            challenge.Type = request.Type;
            challenge.DateUpdated = DateTime.Now;

            if (request.Image != null && request.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(challenge.FileName))
                {
                    await _cloudinaryService.RemoveImageAsync(challenge.FileName);
                }

                var image = await _cloudinaryService.UploadImageAsync(request.Image);
                if (image != null)
                {
                    challenge.Image = image.SecureUrl.AbsoluteUri;
                    challenge.FileName = image.PublicId;
                }
            }

            _unitOfWork.GetRepository<Challenge>().UpdateAsync(challenge);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Update challenge fail", 400, "CHALLENGE_UPDATE_FAIL");
        }
    }
}
