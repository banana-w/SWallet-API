using SWallet.Domain.Models;
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
        Task<PointPurchaseHistory> GetPurchaseHistoryByPaymentIdAsync(string paymentId);
        Task<PointPurchaseHistory> UpdatePurchaseHistoryAsync(PointPurchaseHistory purchaseHistory);
    }
}
