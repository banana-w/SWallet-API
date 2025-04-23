using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Response.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IPurchaseHistory
    {
        Task<PointPurchaseHistory> SavePurchaseHistoryAsync(PointPurchaseHistory purchaseHistory);
        Task<PointPurchaseHistory> GetPurchaseHistoryByPaymentIdAsync(long paymentId);
        Task<PointPurchaseHistory> GetPurchaseHistoryById(string Id);
        Task<PointPurchaseHistory> UpdatePurchaseHistoryAsync(PointPurchaseHistory purchaseHistory);

        Task<IPaginate<PointPurchaseHistory>> GetHistoriesById(string searchName, int page, int size, string? id);
    }
}
