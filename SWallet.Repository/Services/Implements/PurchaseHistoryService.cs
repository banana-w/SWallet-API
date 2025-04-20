using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Response.Product;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class PurchaseHistoryService : BaseService<PurchaseHistoryService>, IPurchaseHistory
    {
        public PurchaseHistoryService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<PurchaseHistoryService> logger) : base(unitOfWork, logger)
        {
        }

        public async Task<PointPurchaseHistory> GetPurchaseHistoryByPaymentIdAsync(string paymentId)
        {
            var history = await _unitOfWork.GetRepository<PointPurchaseHistory>().SingleOrDefaultAsync(
              selector: x => new PointPurchaseHistory
              {
                  Id = x.Id,
                  PointPackageId = x.PointPackageId,
                  Points = x.Points,
                  Amount = x.Amount,
                  PaymentId = x.PaymentId,
                  PaymentStatus = x.PaymentStatus,
                  CreatedDate = x.CreatedDate,
                  UpdatedDate = x.UpdatedDate,
                  EntityId = x.EntityId,
                  EntityType = x.EntityType,
                 
              },
              predicate: x => x.Id == paymentId);
            return history;
        }

        public async Task<PointPurchaseHistory> SavePurchaseHistoryAsync(PointPurchaseHistory purchaseHistory)
        {
            var newHistory = new PointPurchaseHistory
            {
                Id = Ulid.NewUlid().ToString(),
                EntityId = purchaseHistory.EntityId,
                EntityType = purchaseHistory.EntityType,
                PointPackageId = purchaseHistory.PointPackageId,
                Points = purchaseHistory.Points,
                Amount = purchaseHistory.Amount,
                PaymentId = purchaseHistory.PaymentId,
                PaymentStatus = purchaseHistory.PaymentStatus,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now

            };

            await _unitOfWork.GetRepository<PointPurchaseHistory>().InsertAsync(newHistory);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                throw new ApiException("Create History Fail", 400, "BAD_REQUEST");
            }

            var result = await _unitOfWork.GetRepository<PointPurchaseHistory>().SingleOrDefaultAsync(
                selector: x => new PointPurchaseHistory
                {
                    Id = x.Id,
                    PointPackageId = x.PointPackageId,
                    Points = x.Points,
                    Amount = x.Amount,
                    PaymentId = x.PaymentId,
                    PaymentStatus = x.PaymentStatus,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    EntityId = x.EntityId,
                    EntityType = x.EntityType,
                    },
                predicate: x => x.Id == newHistory.Id);

            return result;
        }

        public async Task<PointPurchaseHistory> UpdatePurchaseHistoryAsync(PointPurchaseHistory purchaseHistory)
        {
            var updateHistory = await _unitOfWork.GetRepository<PointPurchaseHistory>().SingleOrDefaultAsync(predicate: x => x.Id == purchaseHistory.Id
               );

            if (updateHistory == null)
            {
                throw new ApiException("History not found", 404, "NOT_FOUND");
            }
            //updateHistory.Id = purchaseHistory.Id;
            updateHistory.EntityId = purchaseHistory.EntityId;
            updateHistory.EntityType = purchaseHistory.EntityType;
            updateHistory.PointPackageId = purchaseHistory.PointPackageId;
            updateHistory.Points = purchaseHistory.Points;
            updateHistory.Amount = purchaseHistory.Amount;
            updateHistory.PaymentId = purchaseHistory.PaymentId;
            updateHistory.PaymentStatus = purchaseHistory.PaymentStatus;
            updateHistory.CreatedDate = purchaseHistory.CreatedDate;
            updateHistory.UpdatedDate = purchaseHistory.UpdatedDate;
            _unitOfWork.GetRepository<PointPurchaseHistory>().UpdateAsync(updateHistory);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess)
            {
                throw new ApiException("Update product failed", 500, "INTERNAL_SERVER_ERROR");
            }

            var response = new PointPurchaseHistory
            {
                Id = updateHistory.Id,
                EntityId = updateHistory.EntityId,
                EntityType = updateHistory.EntityType,
                PointPackageId = updateHistory.PointPackageId,
                Points = updateHistory.Points,
                Amount = updateHistory.Amount,
                PaymentId = updateHistory.PaymentId,
                PaymentStatus = updateHistory.PaymentStatus,
                CreatedDate = updateHistory.CreatedDate,
                UpdatedDate = updateHistory.UpdatedDate

            };

            return response;
        }
    }
}
