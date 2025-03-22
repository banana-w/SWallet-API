using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.LuckyPrize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ILuckyPrizeService
    {
        Task<List<LuckyPrize>> GetLuckyPrizes();
        Task<LuckyPrize> AddLuckyPrize(LuckyPrizeRequest luckyPrize);
        Task<LuckyPrize> UpadteLucyPrize(int id, LuckyPrizeRequest luckyPrize);

    }
}
