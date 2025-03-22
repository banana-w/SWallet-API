using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Models;
using SWallet.Repository.Payload.Request.LuckyPrize;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuckyPrizeController : ControllerBase
    {
        private readonly ILuckyPrizeService _luckyPrizeService;

        public LuckyPrizeController(ILuckyPrizeService luckyPrizeService)
        {
            _luckyPrizeService = luckyPrizeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLuckyPrizes()
        {
            var result = await _luckyPrizeService.GetLuckyPrizes();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddLuckyPrize(LuckyPrizeRequest luckyPrize)
        {
            var result = await _luckyPrizeService.AddLuckyPrize(luckyPrize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLuckyPrize(int id, LuckyPrizeRequest luckyPrize)
        {
            var result = await _luckyPrizeService.UpadteLucyPrize(id, luckyPrize);
            return Ok(result);
        }
    }
}
