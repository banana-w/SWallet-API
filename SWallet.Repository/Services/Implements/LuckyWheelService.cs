using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{

    public interface ILuckyWheelService
    {
        Task<int> GetSpinCountAsync(string studentId, DateTime date);
        Task IncrementSpinCountAsync(string studentId, DateTime date);
        Task<int> GetBonusSpinsAsync(string studentId, DateTime date);
        Task IncrementBonusSpinsAsync(string studentId, DateTime date);
    }
    public class LuckyWheelService : BaseService<LuckyWheelService>, ILuckyWheelService
    {
        public LuckyWheelService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<LuckyWheelService> logger) : base(unitOfWork, logger)
        {
        }

        public async Task<int> GetSpinCountAsync(string studentId, DateTime date)
        {
            try
            {
                var dateOnly = DateOnly.FromDateTime(date);

                var spinHistory = await _unitOfWork.GetRepository<SpinHistory>()
                    .SingleOrDefaultAsync(predicate: x => x.StudentId == studentId && x.Date == dateOnly);

                if (spinHistory == null)
                {
                    return 0;
                }

                return (int)spinHistory.SpinCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting spin count for studentId: {studentId}, date: {date}");
                throw new ApiException("Failed to get spin count", 500, "INTERNAL_SERVER_ERROR");
            }
        }

        public async Task IncrementSpinCountAsync(string studentId, DateTime date)
        {
            try
            {
                var dateOnly = DateOnly.FromDateTime(date);

                var spinHistory = await _unitOfWork.GetRepository<SpinHistory>()
                    .SingleOrDefaultAsync(predicate: x => x.StudentId == studentId && x.Date == dateOnly);

                if (spinHistory == null)
                {
                    spinHistory = new SpinHistory
                    {
                        StudentId = studentId,
                        Date = dateOnly,
                        SpinCount = 1,
                        BonusSpins = 0 // Khởi tạo BonusSpins
                    };
                    await _unitOfWork.GetRepository<SpinHistory>().InsertAsync(spinHistory);
                }
                else
                {
                    spinHistory.SpinCount += 1;
                    _unitOfWork.GetRepository<SpinHistory>().UpdateAsync(spinHistory);
                }

                var isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccess)
                {
                    throw new ApiException("Failed to increment spin count", 400, "BAD_REQUEST");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error incrementing spin count for studentId: {studentId}, date: {date}");
                throw new ApiException("Failed to increment spin count", 500, "INTERNAL_SERVER_ERROR");
            }
        }

        public async Task<int> GetBonusSpinsAsync(string studentId, DateTime date)
        {
            try
            {
                var dateOnly = DateOnly.FromDateTime(date);

                var spinHistory = await _unitOfWork.GetRepository<SpinHistory>()
                    .SingleOrDefaultAsync(predicate: x => x.StudentId == studentId && x.Date == dateOnly);

                if (spinHistory == null)
                {
                    return 0; // Nếu không có bản ghi, trả về 0
                }

                return (int)spinHistory.BonusSpins;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bonus spins for studentId: {studentId}, date: {date}");
                throw new ApiException("Failed to get bonus spins", 500, "INTERNAL_SERVER_ERROR");
            }
        }

        public async Task IncrementBonusSpinsAsync(string studentId, DateTime date)
        {
            try
            {
                var dateOnly = DateOnly.FromDateTime(date);

                var spinHistory = await _unitOfWork.GetRepository<SpinHistory>()
                    .SingleOrDefaultAsync(predicate: x => x.StudentId == studentId && x.Date == dateOnly);

                if (spinHistory == null)
                {
                    // Nếu không có bản ghi, tạo mới
                    spinHistory = new SpinHistory
                    {
                        StudentId = studentId,
                        Date = dateOnly,
                        SpinCount = 0, // Khởi tạo SpinCount
                        BonusSpins = 1 // Tăng BonusSpins
                    };
                    await _unitOfWork.GetRepository<SpinHistory>().InsertAsync(spinHistory);
                }
                else
                {
                    // Nếu đã có bản ghi, tăng BonusSpins
                    spinHistory.BonusSpins += 1;
                    _unitOfWork.GetRepository<SpinHistory>().UpdateAsync(spinHistory);
                }

                var isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccess)
                {
                    throw new ApiException("Failed to increment bonus spins", 400, "BAD_REQUEST");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error incrementing bonus spins for studentId: {studentId}, date: {date}");
                throw new ApiException("Failed to increment bonus spins", 500, "INTERNAL_SERVER_ERROR");
            }
        }
    }
}
