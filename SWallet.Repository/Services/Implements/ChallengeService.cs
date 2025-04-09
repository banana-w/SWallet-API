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
using System.Diagnostics;
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
                Amount = request.Amount,
                Condition = request.Condition,
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

        public async Task<bool> AssignChallengeToStudent(string challengeId, string studentId)
        {
            var studentChallenge = new StudentChallenge
            {
                ChallengeId = challengeId,
                StudentId = studentId,
                Current = 0, // Tiến trình ban đầu
                IsCompleted = false, // Chưa hoàn thành
                DateCreated = DateTime.Now,
                Status = true
            };

            await _unitOfWork.GetRepository<StudentChallenge>().InsertAsync(studentChallenge);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Assign challenge to student fail", 400, "ASSIGN_CHALLENGE_FAIL");
        }

        public async Task<bool> AssignAllChallengesToStudent(string studentId)
        {
            // Lấy tất cả các challenge hiện có từ repository
            var challenges = await _unitOfWork.GetRepository<Challenge>().GetListAsync();

            if (challenges.Count == 0)
            {
                throw new ApiException("No challenges available to assign", 400, "NO_CHALLENGES_FOUND");
            }

            // Tạo danh sách StudentChallenge cho từng challenge
            var studentChallenges = challenges.Select(challenge => new StudentChallenge
            {
                ChallengeId = challenge.Id,
                StudentId = studentId,
                Current = 0, // Tiến trình ban đầu
                IsCompleted = false, // Chưa hoàn thành
                DateCreated = DateTime.Now,
                Status = true
            }).ToList();

            // Thêm tất cả StudentChallenge vào repository
            await _unitOfWork.GetRepository<StudentChallenge>().InsertRangeAsync(studentChallenges);

            var result = await _unitOfWork.CommitAsync();

            if (result > 0)
            {
                return true;
            }

            throw new ApiException("Failed to assign challenges to student", 400, "ASSIGN_ALL_CHALLENGES_FAIL");
        }

        public async Task<bool> UpdateAchievementProgress(string studentId, string challengeId, decimal amount)
        {
            var studentChallenge = await _unitOfWork.GetRepository<StudentChallenge>()
                .SingleOrDefaultAsync(predicate: sc => sc.StudentId == studentId && sc.ChallengeId == challengeId);

            if (studentChallenge != null && (bool)!studentChallenge.IsCompleted)
            {
                studentChallenge.Current = (studentChallenge.Current ?? 0) + amount;
                var challenge = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(predicate: c => c.Id == challengeId);

                if (studentChallenge.Current >= challenge.Condition)
                {
                    studentChallenge.IsCompleted = true;
                    studentChallenge.DateCompleted = DateTime.Now;
                }
                _unitOfWork.GetRepository<StudentChallenge>().UpdateAsync(studentChallenge);
                var result = await _unitOfWork.CommitAsync();
                if (result > 0)
                {
                    return true;
                }
            }
            throw new ApiException("Update achievement progress fail", 400, "UPDATE_ACHIEVEMENT_FAIL");
        }

        public async Task<bool> RecordDailyTaskAction(string studentId, string challengeId, decimal amount)
        {
            var transaction = new ChallengeTransaction
            {
                Id = Ulid.NewUlid().ToString(),
                ChallengeId = challengeId,
                StudentId = studentId,
                Amount = amount, // Giá trị của hành động (ví dụ: 1 bài tập = 1 đơn vị)
                DateCreated = DateTime.Now,
                Description = "Daily task action",
                State = true
            };
            await _unitOfWork.GetRepository<ChallengeTransaction>().InsertAsync(transaction);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Record daily task action fail", 400, "RECORD_DAILY_TASK_FAIL");
        }

        //public bool IsAchievementCompleted(string studentId, string challengeId)
        //{
        //    var studentChallenge = _context.StudentChallenges
        //        .FirstOrDefault(sc => sc.StudentId == studentId && sc.ChallengeId == challengeId);
        //    return studentChallenge?.IsCompleted ?? false;
        //}
        //public bool IsDailyTaskCompletedToday(string studentId, string challengeId)
        //{
        //    var today = DateTime.Today;
        //    var transactions = _context.ChallengeTransactions
        //        .Where(t => t.ChallengeId == challengeId && t.StudentId == studentId &&
        //                    t.DateCreated >= today && t.DateCreated < today.AddDays(1))
        //        .ToList();

        //    var totalAmount = transactions.Sum(t => t.Amount ?? 0);
        //    var challenge = _context.Challenges.FirstOrDefault(c => c.Id == challengeId);
        //    return totalAmount >= challenge.Condition;
        //}
        //public void RewardStudent(string studentId, string challengeId)
        //{
        //    var challenge = _context.Challenges.FirstOrDefault(c => c.Id == challengeId);
        //    if (challenge == null) return;

        //    var wallet = _context.Wallets.FirstOrDefault(w => w.StudentId == studentId);
        //    if (wallet != null)
        //    {
        //        wallet.Balance += challenge.Amount; // Cộng phần thưởng vào ví
        //        _context.SaveChanges();
        //    }
        //}

        public async Task<bool> CheckProgress(string studentId, string challengeId, decimal newAmount)
        {
            var challenge = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(predicate: c => c.Id == challengeId);
            var studentChallenge = await _unitOfWork.GetRepository<StudentChallenge>()
                .SingleOrDefaultAsync(predicate: sc => sc.StudentId == studentId && sc.ChallengeId == challengeId);

            if (challenge == null || studentChallenge == null) return false;

            if (challenge.Type == 1) // Thành tựu
            {
                if (studentChallenge.IsCompleted == true) return false; // Đã hoàn thành trước đó

                studentChallenge.Current = (studentChallenge.Current ?? 0) + newAmount;
                if (studentChallenge.Current >= challenge.Condition)
                {
                    studentChallenge.IsCompleted = true;
                    studentChallenge.DateCompleted = DateTime.Now;
                }
                _unitOfWork.GetRepository<StudentChallenge>().UpdateAsync(studentChallenge);
                var result = await _unitOfWork.CommitAsync() > 0;
                if (result)
                    return studentChallenge.IsCompleted.Value;
            }
            else if (challenge.Type == 2) // Nhiệm vụ hàng ngày
            {
                var today = DateTime.Today;
                var transactions = await _unitOfWork.GetRepository<ChallengeTransaction>().GetListAsync(
                    predicate: t => t.ChallengeId == challengeId && t.StudentId == studentId &&
                                t.DateCreated >= today && t.DateCreated < today.AddDays(1));

                var totalAmount = transactions.Sum(t => t.Amount ?? 0) + newAmount;

                return totalAmount >= challenge.Condition;
            }

            throw new ApiException("Check progress fail", 400, "CHECK_PROGRESS_FAIL");
        }

        public async Task<ChallengeResponse> GetChallenge(string id)
        {
            var challenge = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(
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
                    Amount = x.Amount,
                    Condition = x.Condition,
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

        public Task<IPaginate<ChallengeResponseExtra>> GetStudentChallenges(
                string studentId,
                string? search,
                IEnumerable<ChallengeType> types,
                int page,
                int size)
        {
            Expression<Func<StudentChallenge, bool>> filter = sc =>
                sc.StudentId == studentId && // Lọc theo studentId
                sc.Status == true && // Chỉ lấy các challenge đang hoạt động
                (string.IsNullOrEmpty(search) || sc.Challenge.ChallengeName.Contains(search)) && // Tìm kiếm theo tên challenge
                (!types.Any() || types.Contains((ChallengeType)sc.Challenge.Type)); // Lọc theo loại challenge

            var studentChallenges = _unitOfWork.GetRepository<StudentChallenge>().GetPagingListAsync(
                selector: sc => new ChallengeResponseExtra
                {
                    id = sc.ChallengeId, 
                    challengeId = sc.ChallengeId,
                    challengeType = ((ChallengeType)sc.Challenge.Type).ToString(),
                    challengeTypeName = sc.Challenge.Type == 1 ? "Hằng ngày" : "Thành Tựu", 
                    challengeName = sc.Challenge.ChallengeName,
                    challengeImage = sc.Challenge.Image,
                    studentId = sc.StudentId,
                    studentName = sc.Student.FullName,
                    amount = sc.Challenge.Amount ?? 0,
                    current = sc.Current ?? 0,
                    condition = sc.Challenge.Condition ?? 0,
                    isCompleted = sc.IsCompleted ?? false,
                    isClaimed = sc.DateCompleted.HasValue && sc.IsCompleted == true,
                    dateCreated = sc.DateCreated ?? DateTime.MinValue,
                    dateUpdated = sc.DateUpdated,
                    description = sc.Challenge.Description,
                    status = sc.Status ?? false
                },
                predicate: filter,
                include: source => source.Include(sc => sc.Challenge)
                                        .Include(sc => sc.Student), 
                page: page,
                size: size);

            return studentChallenges;
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
