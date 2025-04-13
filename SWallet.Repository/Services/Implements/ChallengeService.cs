using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request;
using SWallet.Repository.Payload.Response.Challenge;
using SWallet.Repository.Services.Interfaces;
using SWallet.Repository.Utils;
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
        private readonly IWalletService _walletService;
        public ChallengeService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<ChallengeService> logger, ICloudinaryService cloudinaryService, IWalletService walletService) : base(unitOfWork, logger)
        {
            _cloudinaryService = cloudinaryService;
            _walletService = walletService;
        }

        public async Task<bool> CreateChallenge(ChallengeRequest request)
        {
            // Tạo challenge mới
            var challenge = new Challenge
            {
                Id = Ulid.NewUlid().ToString(),
                ChallengeName = request.ChallengeName,
                Description = request.Description,
                Amount = request.Amount,
                Condition = request.Condition,
                Type = request.Type,
                Category = request.Category,
                Image = "NULL",
                FileName = "NULL",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Status = true
            };

            // Thêm challenge vào database
            await _unitOfWork.GetRepository<Challenge>().InsertAsync(challenge);

            // Xử lý hình ảnh
            if (request.Image != null && request.Image.Length > 0)
            {
                var image = await _cloudinaryService.UploadImageAsync(request.Image);
                if (image != null)
                {
                    challenge.Image = image.SecureUrl.AbsoluteUri;
                    challenge.FileName = image.PublicId;
                }
            }

            // Gán StudentChallenge cho tất cả sinh viên
            var students = await _unitOfWork.GetRepository<Student>().GetListAsync();
            if (students.Count != 0)
            {
                var studentChallenges = new List<StudentChallenge>();

                foreach (var student in students)
                {
                    decimal initialProgress = 0;

                    // Nếu là Achievement, tính tổng Current từ các StudentChallenge cùng Category
                    if (request.Type == (int)ChallengeType.Achievement)
                    {
                        var existingStudentChallenges = await _unitOfWork.GetRepository<StudentChallenge>()
                            .GetListAsync(predicate: sc => sc.StudentId == student.Id &&
                                              sc.Challenge.Category == request.Category &&
                                              sc.Challenge.Type == 1 &&
                                              sc.Status == true);

                        initialProgress = existingStudentChallenges.Sum(sc => sc.Current ?? 0);
                    }

                    // Tạo StudentChallenge mới
                    var studentChallenge = new StudentChallenge
                    {
                        ChallengeId = challenge.Id,
                        StudentId = student.Id,
                        Current = initialProgress, // Gán tiến trình ban đầu
                        IsCompleted = initialProgress >= challenge.Condition, // Kiểm tra ngay nếu đã hoàn thành
                        DateCreated = DateTime.Now,
                        Status = true,
                        DateCompleted = initialProgress >= challenge.Condition ? DateTime.Now : null
                    };

                    studentChallenges.Add(studentChallenge);
                }

                // Thêm tất cả StudentChallenge vào repository
                await _unitOfWork.GetRepository<StudentChallenge>().InsertRangeAsync(studentChallenges);
            }

            // Commit thay đổi
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
            var challengeIds = await _unitOfWork.GetRepository<Challenge>().GetListAsync(selector: x => x.Id);

            if (challengeIds.Count == 0)
            {
                throw new ApiException("No challenges available to assign", 400, "NO_CHALLENGES_FOUND");
            }

            // Tạo danh sách StudentChallenge cho từng challenge
            var studentChallenges = challengeIds.Select(challengeId => new StudentChallenge
            {
                ChallengeId = challengeId,
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
                Type = (int)ChallengeType.Daily
            };
            await _unitOfWork.GetRepository<ChallengeTransaction>().InsertAsync(transaction);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Record daily task action fail", 400, "RECORD_DAILY_TASK_FAIL");
        }

        public async Task<(bool IsCompleted, decimal Amount)> IsAchievementCompleted(string studentId, string challengeId)
        {
            var studentChallenge = await _unitOfWork.GetRepository<StudentChallenge>()
                .SingleOrDefaultAsync(predicate: sc => sc.StudentId == studentId && sc.ChallengeId == challengeId);

            bool isCompleted = studentChallenge?.IsCompleted ?? false && studentChallenge.DateCreated.HasValue;

            decimal challengeAmount = 0;
            if (studentChallenge != null)
            {
                challengeAmount = await _unitOfWork.GetRepository<Challenge>()
                   .SingleOrDefaultAsync(
                   selector: c => c.Amount,
                   predicate: c => c.Id == challengeId) ?? 0;
            }

            return (isCompleted, challengeAmount);
        }
        public async Task<(bool IsCompleted, decimal Amount)> IsDailyTaskCompletedToday(string studentId, string challengeId)
        {
            var today = DateTime.Today;
            var transactions = await _unitOfWork.GetRepository<ChallengeTransaction>().GetListAsync(
               predicate: t => t.ChallengeId == challengeId && t.StudentId == studentId &&
                            t.DateCreated >= today && t.DateCreated < today.AddDays(1));

            var totalAmount = transactions.Count;
            var challenge = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(predicate: c => c.Id == challengeId);

            var isCompletedSC = await _unitOfWork.GetRepository<StudentChallenge>().SingleOrDefaultAsync(
                selector: sc => sc.IsCompleted,
                predicate: sc => sc.StudentId == studentId && sc.ChallengeId == challengeId);
            if ((bool)isCompletedSC)
            {
                decimal amount1 = challenge.Amount ?? 0;
                return (true, amount1);
            }

            bool isCompleted = totalAmount >= challenge.Condition;
            decimal amount = challenge.Amount ?? 0;

            return (isCompleted, amount);
        }
        public async Task<bool> RewardStudent(string studentId, string challengeId, int type)
        {
            (bool isCompleted, decimal amount) result = (false, 0);

            if (type == (int)ChallengeType.Daily)
            {
                result = await IsDailyTaskCompletedToday(studentId, challengeId);

            }
            else if (type == (int)ChallengeType.Achievement)
            {
                result = await IsAchievementCompleted(studentId, challengeId);
            }

            if (result.isCompleted)
            {
                var walletId = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                    selector: x => x.Id,
                    predicate: w => w.StudentId == studentId);
                if (!string.IsNullOrEmpty(walletId))
                {
                    await _walletService.UpdateWalletForRedeem(walletId, result.amount);
                    var transaction = new ChallengeTransaction
                    {
                        Id = Ulid.NewUlid().ToString(),
                        ChallengeId = challengeId,
                        StudentId = studentId,
                        WalletId = walletId,
                        Amount = result.amount,
                        DateCreated = DateTime.UtcNow,
                        Description = type == (int)ChallengeType.Daily ? "Daily" : "Achievement",
                        Type = type
                    };

                    var check = await AddChallengeTransaction(transaction, type);

                    return check;
                }
            }
            throw new ApiException("Reward student fail", 400, "REWARD_STUDENT_FAIL");
        }
        public async Task<bool> AddChallengeTransaction(ChallengeTransaction transaction, int type)
        {

            await _unitOfWork.GetRepository<ChallengeTransaction>().InsertAsync(transaction);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return true;
            }
            throw new ApiException("Add challenge transaction fail", 400, "ADD_CHALLENGE_TRANSACTION_FAIL");
        }

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

        public async Task<IPaginate<ChallengeResponseExtra>> GetStudentChallenges(
            string studentId,
            string? search,
            IEnumerable<ChallengeType> types,
            int page,
            int size)
        {
            // Bộ lọc cho StudentChallenge
            Expression<Func<StudentChallenge, bool>> filter = sc =>
                sc.StudentId == studentId &&
                sc.Status == true &&
                (string.IsNullOrEmpty(search) || sc.Challenge.ChallengeName.Contains(search)) &&
                (!types.Any() || types.Contains((ChallengeType)sc.Challenge.Type));

            // Truy vấn StudentChallenge với phân trang
            var studentChallenges = await _unitOfWork.GetRepository<StudentChallenge>().GetPagingListAsync(
                selector: sc => new ChallengeResponseExtra
                {
                    id = sc.ChallengeId,
                    challengeId = sc.ChallengeId,
                    challengeType = ((ChallengeType)sc.Challenge.Type).ToString(),
                    challengeTypeName = sc.Challenge.Type == 1 ? "Thành Tựu" : "Hằng ngày",
                    challengeName = sc.Challenge.ChallengeName,
                    challengeImage = sc.Challenge.Image,
                    studentId = sc.StudentId,
                    studentName = sc.Student.FullName,
                    amount = sc.Challenge.Amount ?? 0,
                    current = sc.Current ?? 0, // Sẽ cập nhật sau
                    condition = sc.Challenge.Condition ?? 0,
                    isCompleted = sc.IsCompleted ?? false,
                    isClaimed = false, // Sẽ cập nhật sau
                    dateCreated = sc.DateCreated ?? DateTime.MinValue,
                    dateUpdated = sc.DateUpdated,
                    description = sc.Challenge.Description,
                    status = sc.Status ?? false,
                    category = sc.Challenge.Category // Để nhóm
                },
                predicate: filter,
                include: source => source.Include(sc => sc.Challenge)
                                        .Include(sc => sc.Student),
                page: page,
                size: size);

            // Xử lý tiến trình nếu có dữ liệu
            if (studentChallenges.Items.Any())
            {
                // Lấy tất cả category duy nhất
                var categories = studentChallenges.Items.Select(c => c.category).Distinct().ToList();

                // Xử lý Daily challenges
                if (types.Contains(ChallengeType.Daily) || !types.Any())
                {
                    var today = TimeUtils.GetVietnamToday();

                    // Lấy tất cả giao dịch trong ngày cho sinh viên
                    var allTransactions = await _unitOfWork.GetRepository<ChallengeTransaction>()
                        .GetListAsync(
                            predicate: t => t.StudentId == studentId &&
                                            t.DateCreated >= today &&
                                            t.DateCreated < today.AddDays(1),
                            include: source => source.Include(t => t.StudentChallenge)
                                                   .ThenInclude(sc => sc.Challenge) // Kết nối với Challenge
                        );

                    // Nhóm giao dịch theo Category
                    var progressByCategory = allTransactions
                        .Where(t => t.Type == 0) // Giao dịch tiến trình
                        .GroupBy(t => t.StudentChallenge.Challenge.Category!)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Count()
                        );

                    var claimByCategory = allTransactions
                        .Where(t => t.Type == 1) // Giao dịch nhận thưởng
                        .GroupBy(t => t.StudentChallenge.Challenge.Category!)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(t => t.ChallengeId).Distinct().ToList()
                        );

                    // Cập nhật current và isCompleted cho Daily challenges
                    foreach (var challenge in studentChallenges.Items.Where(c => c.challengeType == "Daily"))
                    {
                        var totalProgress = progressByCategory.TryGetValue(challenge.category, out var count) ? count : 0;
                        challenge.current = totalProgress;
                        challenge.isCompleted = totalProgress >= challenge.condition;
                        challenge.isClaimed = claimByCategory.TryGetValue(challenge.category, out var claimedIds)
                                                && claimedIds.Contains(challenge.challengeId);
                    }
                }

                // Xử lý Achievement challenges
                if (types.Contains(ChallengeType.Achievement) || !types.Any())
                {
                    // Lấy tất cả StudentChallenge của sinh viên
                    var allStudentChallenges = await _unitOfWork.GetRepository<StudentChallenge>()
                        .GetListAsync(
                            predicate: sc => sc.StudentId == studentId &&
                                            sc.Status == true &&
                                            sc.Challenge.Type == 1,
                            include: source => source.Include(sc => sc.Challenge)
                        );

                    // Nhóm tiến trình theo Category
                    var progressByCategory = allStudentChallenges
                        .GroupBy(sc => sc.Challenge.Category!)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Sum(sc => sc.Current ?? 0)
                        );

                    var claimByCategory = allStudentChallenges
                        .Where(sc => sc.DateCompleted.HasValue)
                        .GroupBy(sc => sc.Challenge.Category!)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(sc => sc.ChallengeId).Distinct().ToList()
                        );

                    // Cập nhật current và isCompleted cho Achievement challenges
                    foreach (var challenge in studentChallenges.Items.Where(c => c.challengeType == "Achievement"))
                    {
                        var totalProgress = progressByCategory.TryGetValue(challenge.category, out var sum) ? sum : 0;
                        challenge.current = totalProgress;
                        challenge.isCompleted = totalProgress >= challenge.condition;
                        challenge.isClaimed = claimByCategory.TryGetValue(challenge.category, out var claimedIds)
                                                && claimedIds.Contains(challenge.challengeId);
                    }
                }
            }

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
