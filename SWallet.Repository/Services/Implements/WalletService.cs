using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Wallet;
using SWallet.Repository.Payload.Response.Wallet;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SWallet.Repository.Services.Implements
{
    public class WalletService : BaseService<WalletService>, IWalletService
    {
        public WalletService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<WalletService> logger) : base(unitOfWork, logger)
        {
        }

        public async Task AddPointsToWallet(string campusId, int points)
        {
            try
            {
                // Tìm Wallet dựa trên CampusId
                var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                 predicate: b => b.CampusId == campusId);


                if (wallet != null)
                {
                    // Cộng điểm vào Wallet
                    wallet.Balance += points;

                    // Cập nhật Wallet trong cơ sở dữ liệu
                    _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy Wallet
                    // Bạn có thể tạo mới Wallet hoặc ném exception
                    // Ví dụ: tạo mới Wallet
                    var newWallet = new Wallet
                    {
                        Id = Guid.NewGuid().ToString(),
                        Balance = points,
                        CampusId = campusId,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Status = true
                    };
                    _unitOfWork.GetRepository<Wallet>().InsertAsync(wallet);
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: log lỗi)
                Console.WriteLine($"Lỗi khi cộng điểm vào Wallet: {ex.Message}");
                throw; // Ném lại exception để xử lý ở tầng trên
            }
        }

        public async Task AddPointsToBrandWallet(string brandId, int points)
        {
            try
            {
                // Tìm Wallet dựa trên CampusId
                var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                 predicate: b => b.BrandId == brandId);


                if (wallet != null)
                {
                    // Cộng điểm vào Wallet
                    wallet.Balance += points;

                    // Cập nhật Wallet trong cơ sở dữ liệu
                    _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy Wallet
                    // Bạn có thể tạo mới Wallet hoặc ném exception
                    // Ví dụ: tạo mới Wallet
                    var newWallet = new Wallet
                    {
                        Id = Guid.NewGuid().ToString(),
                        Balance = points,
                        BrandId = brandId,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Status = true
                    };
                    _unitOfWork.GetRepository<Wallet>().InsertAsync(wallet);
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: log lỗi)
                Console.WriteLine($"Lỗi khi cộng điểm vào Wallet: {ex.Message}");
                throw; // Ném lại exception để xử lý ở tầng trên
            }
        }


        public async Task<WalletResponse> AddWallet(WalletRequest walletRequest)
        {
            if (walletRequest.Balance < 0)
            {
                throw new ApiException("Balance cannot be negative", 400, "NEGATIVE_BALANCE");
            }
            var wallet = new Wallet
            {
                Id = Ulid.NewUlid().ToString(),
                CampusId = walletRequest.CampusId,
                CampaignId = walletRequest.CampaignId,
                StudentId = walletRequest.StudentId,
                BrandId = walletRequest.BrandId,
                LecturerId = walletRequest.LecturerId,
                Type = walletRequest.Type,
                Balance = walletRequest.Balance,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Description = walletRequest.Description,
                Status = true
            };
            await _unitOfWork.GetRepository<Wallet>().InsertAsync(wallet);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return new WalletResponse
                {
                    Id = wallet.Id,
                    CampaignId = wallet.CampaignId,
                    StudentId = wallet.StudentId,
                    CampusId = wallet.CampusId,
                    BrandId = wallet.BrandId,
                    LecturerId = wallet.LecturerId,
                    Type = wallet.Type,
                    Balance = wallet.Balance,
                    DateCreated = wallet.DateCreated,
                    DateUpdated = wallet.DateUpdated,
                    Description = wallet.Description,
                    Status = wallet.Status
                };
            }
            throw new ApiException("Create Wallet Failed", 500, "WALLET_CREATION_FAILED");
        }

        public async Task<WalletResponse> GetWalletByBrandId(string id, int type)
        {
            var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(predicate: x => x.BrandId == id && x.Type == type);
            if (wallet == null)
            {
                throw new ApiException("Wallet not found", 404, "WALLET_NOT_FOUND");
            }
            return new WalletResponse
            {
                Id = wallet.Id,
                CampaignId = wallet.CampaignId,
                StudentId = wallet.StudentId,
                BrandId = wallet.BrandId,
                Type = wallet.Type,
                Balance = wallet.Balance,
                DateCreated = wallet.DateCreated,
                DateUpdated = wallet.DateUpdated,
                Description = wallet.Description,
                Status = wallet.Status
            };
        }

        public async Task<WalletResponse> GetWalletByCampusId(string id, int type)
        {
            var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(predicate: x => x.CampusId == id && x.Type == type);
            if (wallet == null)
            {
                throw new ApiException("Wallet not found", 404, "WALLET_NOT_FOUND");
            }
            return new WalletResponse
            {
                Id = wallet.Id,
                CampaignId = wallet.CampaignId,
                StudentId = wallet.StudentId,
                BrandId = wallet.BrandId,
                CampusId = wallet.CampusId,
                Type = wallet.Type,
                Balance = wallet.Balance,
                DateCreated = wallet.DateCreated,
                DateUpdated = wallet.DateUpdated,
                Description = wallet.Description,
                Status = wallet.Status
            };
        }

        public async Task<WalletResponse> GetWalletByLecturerId(string id, int type)
        {
            var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(predicate: x => x.LecturerId == id && x.Type == type);
            if (wallet == null)
            {
                throw new ApiException("Wallet not found", 404, "WALLET_NOT_FOUND");
            }
            return new WalletResponse
            {
                Id = wallet.Id,
                CampaignId = wallet.CampaignId,
                StudentId = wallet.StudentId,
                BrandId = wallet.BrandId,
                Type = wallet.Type,
                Balance = wallet.Balance,
                DateCreated = wallet.DateCreated,
                DateUpdated = wallet.DateUpdated,
                Description = wallet.Description,
                Status = wallet.Status
            };
        }

        public async Task<WalletResponse> GetWalletByStudentId(string id, int type)
        {
            var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(predicate: x => x.StudentId == id && x.Type == type);
            if (wallet == null)
            {
                throw new ApiException("Wallet not found", 404, "WALLET_NOT_FOUND");
            }
            return new WalletResponse
            {
                Id = wallet.Id,
                CampaignId = wallet.CampaignId,
                StudentId = wallet.StudentId,
                BrandId = wallet.BrandId,
                Type = wallet.Type,
                Balance = wallet.Balance,
                DateCreated = wallet.DateCreated,
                DateUpdated = wallet.DateUpdated,
                Description = wallet.Description,
                Status = wallet.Status
            };
        }

        public async Task<WalletResponse> UpdateWallet(string id, decimal balance)
        {
            var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (wallet == null)
            {
                throw new ApiException("Wallet not found", 404, "WALLET_NOT_FOUND");
            }
            if (balance < 0)
            {
                throw new ApiException("Balance cannot be negative", 400, "NEGATIVE_BALANCE");
            }

            wallet.Balance =+ balance ;
            wallet.DateUpdated = DateTime.Now;

             _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
            var result = await _unitOfWork.CommitAsync();
            if (result > 0)
            {
                return new WalletResponse
                {
                    Id = wallet.Id,
                    CampaignId = wallet.CampaignId,
                    StudentId = wallet.StudentId,
                    BrandId = wallet.BrandId,
                    Type = wallet.Type,
                    Balance = wallet.Balance,
                    DateCreated = wallet.DateCreated,
                    DateUpdated = wallet.DateUpdated,
                    Description = wallet.Description,
                    Status = wallet.Status
                };
            }
            throw new ApiException("Update Wallet Failed", 500, "WALLET_UPDATE_FAILED");
        }

        public async Task<WalletResponse> UpdateWalletForRedeem(string id, decimal balanceChange)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var wallet = await _unitOfWork.GetRepository<Wallet>()
                    .SingleOrDefaultAsync(predicate: x => x.Id == id);

                if (wallet == null)
                {
                    throw new ApiException("Wallet not found for this student", 404, "WALLET_NOT_FOUND");
                }

                // Kiểm tra số dư sau khi thay đổi
                decimal newBalance = (decimal)(wallet.Balance + balanceChange);
                if (newBalance < 0)
                {
                    throw new ApiException("Insufficient balance", 400, "INSUFFICIENT_BALANCE");
                }

                // Cập nhật wallet
                wallet.Balance = newBalance;
                wallet.DateUpdated = DateTime.UtcNow;

                _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
                var result = await _unitOfWork.CommitAsync();
                if (result <= 0)
                {
                    throw new ApiException("Failed to update wallet", 500, "WALLET_UPDATE_FAILED");
                }

                await _unitOfWork.CommitTransactionAsync();

                // Trả về response
                return new WalletResponse
                {
                    Id = wallet.Id,
                    CampaignId = wallet.CampaignId,
                    StudentId = wallet.StudentId,
                    BrandId = wallet.BrandId,
                    Type = wallet.Type,
                    Balance = wallet.Balance,
                    DateCreated = wallet.DateCreated,
                    DateUpdated = wallet.DateUpdated,
                    Description = wallet.Description,
                    Status = wallet.Status
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task AddPointsToStudentWallet(string studentId, int points)
        {
            try
            {

                var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                 predicate: b => b.StudentId == studentId);


                if (wallet != null)
                {
                    // Cộng điểm vào Wallet
                    wallet.Balance += points;

                    // Cập nhật Wallet trong cơ sở dữ liệu
                    _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy Wallet
                    // Bạn có thể tạo mới Wallet hoặc ném exception
                    // Ví dụ: tạo mới Wallet
                    var newWallet = new Wallet
                    {
                        Id = Guid.NewGuid().ToString(),
                        Balance = points,
                        StudentId = studentId,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Status = true
                    };
                    _unitOfWork.GetRepository<Wallet>().InsertAsync(wallet);
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: log lỗi)
                Console.WriteLine($"Lỗi khi cộng điểm vào Wallet: {ex.Message}");
                throw; // Ném lại exception để xử lý ở tầng trên
            }
        }
    }
}
