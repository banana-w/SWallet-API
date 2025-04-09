using Microsoft.EntityFrameworkCore;
using SWallet.Domain.Models;
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

    public class CheckInService : ICheckInService
    {
        private readonly SwalletDbContext _context;
        private readonly IWalletService _walletService;

        public CheckInService(SwalletDbContext context, IWalletService walletService)
        {
            _context = context;
            _walletService = walletService;
        }

        public async Task<(bool Success, string Message, int PointsAwarded)> CheckInWithQR(string studentId, string qrCode, double userLat, double userLong)
        {
            // Tìm địa điểm từ qrCode
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Qrcode == qrCode);
            if (location == null)
            {
                return (false, "Mã QR không hợp lệ hoặc địa điểm không tồn tại", 0);
            }

            // Tính khoảng cách và kiểm tra bán kính
            double distance = CalculateDistance(userLat, userLong, (double)location.Latitue, (double)location.Longtitude);
            if (distance > 50) // Bán kính 50m
            {
                return (false, "Bạn không ở gần địa điểm này để check-in", 0);
            }

            // Ghi lại check-in
            return await RecordCheckIn(studentId, location.Id);
        }

        private async Task<(bool Success, string Message, int PointsAwarded)> RecordCheckIn(string studentId, string locationId)
        {
            var today = DateTime.Today;
            var challengeId = "CHECKIN_DAILY";
            const int pointsAwarded = 10;

            try
            {
                // Kiểm tra hoặc tạo StudentChallenge
                var studentChallenge = await _context.StudentChallenges
                    .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.ChallengeId == challengeId);

                if (studentChallenge == null)
                {
                    studentChallenge = new StudentChallenge
                    {
                        ChallengeId = challengeId,
                        StudentId = studentId,
                        Amount = 0,
                        Current = 0,
                        Condition = 1,
                        IsCompleted = false,
                        DateCreated = DateTime.Now,
                        Status = true
                    };
                    _context.StudentChallenges.Add(studentChallenge);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra check-in trùng lặp trong ngày
                var existingCheckIn = await _context.ChallengeTransactions
                    .FirstOrDefaultAsync(t => t.StudentId == studentId && t.ChallengeId == challengeId &&
                                              t.DateCreated >= today && t.DateCreated < today.AddDays(1) &&
                                              t.Description.Contains(locationId));

                if (existingCheckIn != null)
                {
                    return (false, "Bạn đã check-in tại địa điểm này hôm nay", 0);
                }

                // Lấy ví của sinh viên
                var wallet = await _context.Wallets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(w => w.StudentId == studentId);

                if (wallet == null)
                {
                    return (false, $"Không tìm thấy ví cho sinh viên {studentId}", 0);
                }

                // Tạo giao dịch check-in
                var transaction = new ChallengeTransaction
                {
                    Id = Ulid.NewUlid().ToString(),
                    StudentId = studentId,
                    WalletId = wallet.Id,
                    ChallengeId = challengeId,
                    Amount = pointsAwarded,
                    DateCreated = DateTime.Now,
                    Description = $"Check-in tại {locationId}",
                    State = true
                };

                _context.ChallengeTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                // Cộng điểm vào ví
                await _walletService.AddPointsToStudentWallet(studentId, (int)transaction.Amount);

                // Cập nhật StudentChallenge
                studentChallenge.Current = (studentChallenge.Current ?? 0) + 1;
                if (studentChallenge.Current >= studentChallenge.Condition)
                {
                    studentChallenge.IsCompleted = true;
                    studentChallenge.DateCompleted = DateTime.Now;
                }
                _context.StudentChallenges.Update(studentChallenge);
                await _context.SaveChangesAsync();

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
