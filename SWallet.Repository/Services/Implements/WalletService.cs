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

namespace SWallet.Repository.Services.Implements
{
    public class WalletService : BaseService<WalletService>, IWalletService
    {
        public WalletService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<WalletService> logger) : base(unitOfWork, logger)
        {
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
                CampaignId = walletRequest.CampaignId,
                StudentId = walletRequest.StudentId,
                BrandId = walletRequest.BrandId,
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
                    BrandId = wallet.BrandId,
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

        public Task<WalletResponse> GetWalletByBrandId(string id)
        {
            throw new NotImplementedException();
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

            wallet.Balance = balance;
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
    }
}
