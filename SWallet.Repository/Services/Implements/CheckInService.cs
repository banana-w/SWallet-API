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
        Task<bool> CheckInWithGPS(string studentId, string locationId, double userLat, double userLong);
        Task<bool> CheckInWithQR(string studentId, string qrCode);
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

        public async Task<bool> CheckInWithGPS(string studentId, string locationId, double userLat, double userLong)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
            if (location == null) return false;

            double distance = CalculateDistance(userLat, userLong, (double)location.Latitue, (double)location.Longtitude);
            if (distance > 50) // Bán kính 50m
                return false;

            await RecordCheckIn(studentId, locationId);
            return true;
        }

        public async Task<bool> CheckInWithQR(string studentId, string qrCode)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Qrcode == qrCode);
            if (location == null) return false;

            await RecordCheckIn(studentId, location.Id);
            return true;
        }

        private async Task RecordCheckIn(string studentId, string locationId)
        {
            var today = DateTime.Today;
            var challengeId = "CHECKIN_DAILY";

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

            var existingCheckIn = await _context.ChallengeTransactions
                .FirstOrDefaultAsync(t => t.StudentId == studentId && t.ChallengeId == challengeId &&
                                          t.DateCreated >= today && t.DateCreated < today.AddDays(1) &&
                                          t.Description.Contains(locationId));

            if (existingCheckIn == null)
            {
                var wallet = await _context.Wallets
                    .AsNoTracking() // Không theo dõi thực thể Wallet
                    .FirstOrDefaultAsync(w => w.StudentId == studentId);

                if (wallet == null)
                {
                    throw new Exception($"Không tìm thấy ví cho sinh viên {studentId}");
                }

                var transaction = new ChallengeTransaction
                {
                    Id = Ulid.NewUlid().ToString(),
                    StudentId = studentId,
                    WalletId = wallet.Id,
                    ChallengeId = challengeId,
                    Amount = 10,
                    DateCreated = DateTime.Now,
                    Description = $"Check-in tại {locationId}",
                    State = true
                };

                _context.ChallengeTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                await _walletService.AddPointsToStudentWallet(studentId, (int)transaction.Amount);

                studentChallenge.Current = (studentChallenge.Current ?? 0) + 1;
                if (studentChallenge.Current >= studentChallenge.Condition)
                {
                    studentChallenge.IsCompleted = true;
                    studentChallenge.DateCompleted = DateTime.Now;
                }
                _context.StudentChallenges.Update(studentChallenge);
                await _context.SaveChangesAsync();
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            return Math.Sqrt(Math.Pow(lat2 - lat1, 2) + Math.Pow(lon2 - lon1, 2)) * 111000; // Ước lượng mét
        }
    }
}
