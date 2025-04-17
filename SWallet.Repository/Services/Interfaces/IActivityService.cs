using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Activity;
using SWallet.Repository.Payload.Request.Voucher;
using SWallet.Repository.Payload.Response.Activity;
using SWallet.Repository.Payload.Response.ActivityTransaction;
using SWallet.Repository.Payload.Response.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IActivityService
    {
        Task<bool> RedeemVoucherActivityAsync(ActivityRequest activityRequest);
        Task<UseVoucherResponse> UseVoucherActivityAsync(UseVoucherRequest request);
        Task<bool> UpdateActivityAsync(string id, ActivityRequest activityRequest);
        Task<IPaginate<ActivityResponse>> GetActivityAsync(string search, bool? isAsc, int page, int size);
        Task<IPaginate<TransactionResponse>> GetAllActivityTransactionAsync(
                     string studentId,
                     string search,
                     int type,
                     int page,
                     int size);
        Task<IPaginate<TransactionResponse>> GetAllUseVoucherTransactionAsync(
                     string studentId,
                     string search,
                     int page,
                     int size);
        Task<IPaginate<VoucherStorageResponse>> GetRedeemedVouchersByStudentAsync(string search, string studentId, bool? isUsed, int page, int size);
        Task<IPaginate<VoucherStorageGroupByBrandResponse>> GetRedeemedVouchersByStudentGroupedByBrandAsync(
            string search, string studentId, bool? isUsed, int page, int size);
        Task<bool> DeleteActivityAsync();

    }
}
