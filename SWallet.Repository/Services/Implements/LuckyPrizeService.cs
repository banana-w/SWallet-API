using Microsoft.Extensions.Logging;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.LuckyPrize;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class LuckyPrizeService : BaseService<LuckyPrizeService>, ILuckyPrizeService
    {
        public LuckyPrizeService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<LuckyPrizeService> logger) : base(unitOfWork, logger)
        {
        }

        public async Task<LuckyPrize> AddLuckyPrize(LuckyPrizeRequest luckyPrize)
        {
            var newLuckyPrize = new LuckyPrize
            {
                PrizeName = luckyPrize.PrizeName,
                Probability = luckyPrize.Probability,
                Quantity = luckyPrize.Quantity,
                Status = luckyPrize.Status
            };

            await _unitOfWork.GetRepository<LuckyPrize>().InsertAsync(newLuckyPrize);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (isSuccess)
            {
                return new LuckyPrize
                {
                    Id = newLuckyPrize.Id,
                    PrizeName = newLuckyPrize.PrizeName,
                    Probability = newLuckyPrize.Probability,
                    Quantity = newLuckyPrize.Quantity,
                    Status = newLuckyPrize.Status

                };
            }
            throw new ApiException("Create Lucky Prize Fail", 400, "BAD_REQUEST");
        }

        public async Task<List<Domain.Models.LuckyPrize>> GetLuckyPrizes()
        {
            try
            {
                var result = await _unitOfWork.GetRepository<Domain.Models.LuckyPrize>().GetListAsync(x => x);
                return result.ToList();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error getting lucky prizes: {ex.Message}");
                throw; 
            }
        }

        public async Task<LuckyPrize> UpadteLucyPrize(int id, LuckyPrizeRequest luckyPrize)
        {
            var existingLuckyPrize = await _unitOfWork.GetRepository<LuckyPrize>().SingleOrDefaultAsync(predicate: x => x.Id == id); // Sửa toán tử gán thành so sánh

            if (existingLuckyPrize == null)
            {
                throw new ApiException("LuckyPrize not found", 404, "NOT_FOUND");
            }

            // Cập nhật các thuộc tính của existingLuckyPrize từ luckyPrize
            existingLuckyPrize.PrizeName = luckyPrize.PrizeName;
            existingLuckyPrize.Probability = luckyPrize.Probability;
            existingLuckyPrize.Quantity = luckyPrize.Quantity;
            existingLuckyPrize.Status = luckyPrize.Status;


            _unitOfWork.GetRepository<LuckyPrize>().UpdateAsync(existingLuckyPrize); // Cập nhật LuckyPrize trong repository
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (isSuccess)
            {
                return new LuckyPrize
                {
                    Id = existingLuckyPrize.Id,
                    PrizeName = existingLuckyPrize.PrizeName,
                    Probability = existingLuckyPrize.Probability,
                    Quantity = existingLuckyPrize.Quantity,
                    Status = existingLuckyPrize.Status
                };
            }
            throw new ApiException("Update Lucky Prize Fail", 400, "BAD_REQUEST");
        }


    }
}
