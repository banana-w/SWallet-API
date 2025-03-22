using SWallet.Repository.Payload.Request.Wallet;
using SWallet.Repository.Payload.Response.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface IWalletService
    {
        Task<WalletResponse> AddWallet(WalletRequest walletRequest);
        Task<WalletResponse> UpdateWallet(string id, decimal balance);
        Task<WalletResponse> GetWalletByStudentId(string id, int type);
        Task<WalletResponse> GetWalletByBrandId(string id, int type);
        Task<WalletResponse> GetWalletByCampusId(string id, int type);
        Task<WalletResponse> GetWalletByLecturerId(string id, int type);
        
        Task AddPointsToWallet(string campusId, int points);
        
    }
}
