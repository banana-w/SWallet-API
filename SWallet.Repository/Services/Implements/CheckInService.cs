using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Enums;
using SWallet.Repository.Interfaces;
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
            if (distance > 200)
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
                            predicate: x => x.ChallengeName.Contains("check-in"));

                var studentChallenge = await _unitOfWork.GetRepository<StudentChallenge>()
                    .AnyAsync(sc => sc.StudentId == studentId && sc.ChallengeId == challengeId);

                if (!studentChallenge)
                {
                    await _challengeService.AssignChallengeToStudent(challengeId, studentId);
                }

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
                    DateCreated = DateTime.UtcNow,
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
