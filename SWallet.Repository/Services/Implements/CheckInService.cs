using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Response.DailyCheckIn;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public interface ICheckInService
    {
        Task<(bool Success, string Message, int PointsAwarded)> CheckInWithQR(string studentId, string qrCode, double userLat, double userLong);
        Task<CheckInResponse> GetCheckInDataAsync(string studentId);
        Task<CheckInResponse> CheckInAsync(string studentId);
    }

    public class CheckInService : BaseService<CheckInService>, ICheckInService
    {
        private readonly IWalletService _walletService;
        private readonly IChallengeService _challengeService;

        public CheckInService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<CheckInService> logger, IHttpContextAccessor httpContextAccessor, 
            IWalletService walletService, IChallengeService challengeService) : base(unitOfWork, logger, httpContextAccessor)
        {
            _walletService = walletService;
            _challengeService = challengeService;
        }

        public async Task<CheckInResponse> GetCheckInDataAsync(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                throw new ApiException("StudentId cannot be empty", 400, "BAD_REQUEST");
            }

            try
            {
                // Lấy lịch sử điểm danh của student
                var checkInHistories = await _unitOfWork.GetRepository<DailyGiftHistory>()
                    .GetListAsync(predicate: x => x.StudentId == studentId);

                // Tính streak và points
                int streak = 0;
                int points = 0;
                DateTime today = DateTime.Today;
                bool canCheckInToday = true;
                bool[] checkInHistory = new bool[7]; // Lịch sử 7 ngày trong chuỗi
                int currentDayIndex = 0;

                if (checkInHistories.Any())
                {
                    // Sắp xếp theo ngày điểm danh
                    var orderedHistories = checkInHistories.OrderBy(x => x.CheckInDate).ToList();

                    // Tìm bản ghi gần nhất
                    var lastCheckIn = orderedHistories.Last();
                    streak = (int)lastCheckIn.Streak;
                    points = (int)lastCheckIn.Points;

                    // Kiểm tra xem có thể điểm danh hôm nay không
                    canCheckInToday = lastCheckIn.CheckInDate < today;

                    // Nếu đã bỏ lỡ một ngày, reset streak
                    var yesterday = today.AddDays(-1);
                    if (!orderedHistories.Any(x => x.CheckInDate == yesterday) &&
                        lastCheckIn.CheckInDate < yesterday)
                    {
                        streak = 0;
                    }

                    // Tính currentDayIndex
                    currentDayIndex = streak == 0 ? 0 : (streak >= 7 ? 6 : (streak - 1) % 7);

                    // Tính checkInHistory dựa trên streak
                    for (int i = 0; i < 7; i++)
                    {
                        checkInHistory[i] = i < streak && i < 7; // Đánh dấu true cho các ngày đã điểm danh
                    }
                }

                return new CheckInResponse
                {
                    CheckInHistory = checkInHistory,
                    Streak = streak,
                    Points = points,
                    CanCheckInToday = canCheckInToday,
                    CurrentDayIndex = currentDayIndex,
                    RewardPoints = 0 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting check-in data for studentId: {studentId}");
                throw new ApiException("Failed to get check-in data", 500, "INTERNAL_SERVER_ERROR");
            }
        }

        public async Task<CheckInResponse> CheckInAsync(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                throw new ApiException("StudentId cannot be empty", 400, "BAD_REQUEST");
            }

            try
            {
                // Lấy dữ liệu hiện tại
                var currentData = await GetCheckInDataAsync(studentId);

                if (!currentData.CanCheckInToday)
                {
                    throw new ApiException("You have already checked in today", 400, "BAD_REQUEST");
                }

                // Tính streak mới
                int newStreak = currentData.Streak + 1;

                // Tính phần thưởng dựa trên ngày trong chuỗi
                int rewardPoints;
                if (newStreak >= 7)
                {
                    rewardPoints = 70; // Ngày 7 trở đi: 70 điểm
                }
                else
                {
                    rewardPoints = newStreak * 10; // Ngày 1: 10, Ngày 2: 20, ..., Ngày 6: 60
                }

                // Tính tổng điểm tích lũy
                int newPoints = currentData.Points + rewardPoints;

                // Tạo bản ghi mới
                var newCheckIn = new DailyGiftHistory
                {
                    StudentId = studentId,
                    CheckInDate = DateTime.Today,
                    Streak = newStreak,
                    Points = newPoints
                };

                        // Lưu bản ghi điểm danh
                        await _unitOfWork.GetRepository<DailyGiftHistory>().InsertAsync(newCheckIn);
                        var isSuccess = await _unitOfWork.CommitAsync() > 0;

                        if (!isSuccess)
                        {
                            throw new ApiException("Failed to check in", 400, "BAD_REQUEST");
                        }

                        // Cập nhật balance trong Wallet
                        await _walletService.AddPointsToStudentWallet(studentId, rewardPoints);

          

                // Tính lại currentDayIndex
                int currentDayIndex = newStreak >= 7 ? 6 : (newStreak - 1) % 7;

                // Tính lại checkInHistory dựa trên newStreak
                bool[] checkInHistory = new bool[7];
                for (int i = 0; i < 7; i++)
                {
                    checkInHistory[i] = i < newStreak && i < 7; // Đánh dấu true cho các ngày đã điểm danh
                }

                return new CheckInResponse
                {
                    CheckInHistory = checkInHistory,
                    Streak = newStreak,
                    Points = newPoints,
                    CanCheckInToday = false,
                    CurrentDayIndex = currentDayIndex,
                    RewardPoints = rewardPoints // Trả về phần thưởng nhận được hôm nay
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking in for studentId: {studentId}");
                throw new ApiException("Failed to check in", 500, "INTERNAL_SERVER_ERROR");
            }
        }

        public async Task<(bool Success, string Message, int PointsAwarded)> CheckInWithQR(string studentId, string qrCode, double userLat, double userLong)
        {
            // Tìm địa điểm từ qrCode
            var location = await _unitOfWork.GetRepository<Location>().SingleOrDefaultAsync(predicate: l => l.Qrcode == qrCode);
            if (location == null)
            {
                return (false, "Mã QR không hợp lệ hoặc địa điểm không tồn tại", 0);
            }

            // Tính khoảng cách và kiểm tra bán kính
            double distance = CalculateDistance(userLat, userLong, (double)location.Latitue, (double)location.Longtitude);
            if (distance > 100)
            {
                return (false, "Bạn không ở gần địa điểm này để check-in", 0);
            }

            // Ghi lại check-in
            return await RecordCheckIn(studentId, location.Id);
        }

        private async Task<(bool Success, string Message, int PointsAwarded)> RecordCheckIn(string studentId, string locationId)
        {
            var today = DateTime.Today;
            const int pointsAwarded = 10;

            try
            {
                var challengeId = await _unitOfWork.GetRepository<Challenge>().SingleOrDefaultAsync(
                            selector: x => x.Id,
                            predicate: x => x.Category!.Contains("Check-in"));

                // Kiểm tra check-in trùng lặp trong ngày
                var existingCheckIn = await _unitOfWork.GetRepository<ChallengeTransaction>()
                    .AnyAsync(predicate: t => t.StudentId == studentId && t.ChallengeId == challengeId &&
                                              t.DateCreated >= today && t.DateCreated < today.AddDays(1) &&
                                              t.Description.Contains(locationId));

                if (existingCheckIn)
                {
                    return (false, "Bạn đã check-in tại địa điểm này hôm nay", 0);
                }

                var wallet = await _unitOfWork.GetRepository<Wallet>()
                    .SingleOrDefaultAsync(predicate: w => w.StudentId == studentId);

                if (wallet == null)
                {
                    return (false, $"Không tìm thấy ví cho sinh viên {studentId}", 0);
                }

                var transaction = new ChallengeTransaction
                {
                    Id = Ulid.NewUlid().ToString(),
                    StudentId = studentId,
                    WalletId = wallet.Id,
                    ChallengeId = challengeId,
                    Amount = pointsAwarded,
                    DateCreated = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.TimeUtils.GetVietnamTimeZone()),
                    Type = 0,
                    Description = $"Check-in tại {locationId}",
                };

                await _challengeService.AddChallengeTransaction(transaction, (int)ChallengeType.Daily);

                //await _walletService.AddPointsToStudentWallet(studentId, (int)transaction.Amount);

                return (true, "Check-in thành công", pointsAwarded);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi xử lý check-in: {ex.Message}", 0);
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Bán kính Trái Đất (mét)
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Khoảng cách chính xác hơn bằng công thức Haversine
        }

        private double ToRadians(double degree)
        {
            return degree * Math.PI / 180;
        }
    }
}
