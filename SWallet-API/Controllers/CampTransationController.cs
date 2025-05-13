using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.CampTransaction;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampTransationController : ControllerBase
    {
        private readonly ICampaignTransactionService _campaignTransactionService;
        public CampTransationController(ICampaignTransactionService campaignTransactionService)
        {
            _campaignTransactionService = campaignTransactionService;
        }
        [HttpPost("AddCampaignTransaction")]
        public async Task<IActionResult> AddCampaignTransaction([FromBody] CampaignTransactionRequest request)
        {
            var result = await _campaignTransactionService.AddCampaignTransaction(request);
            return Ok(result);
        }
        [HttpGet("GetByBrand")]
        public async Task<IActionResult> GetCampaignTransaction(string brandId, int page, int size)
        {
            var result = await _campaignTransactionService.GetCampaignTransaction(brandId, page, size);
            return Ok(result);
        }
    }
}
