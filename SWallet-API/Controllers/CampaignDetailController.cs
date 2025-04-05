using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignDetailController : ControllerBase
    {
        private readonly ICampaignDetailService _campaignDetailService;
        private readonly ILogger<CampaignDetailController> _logger; 

        public CampaignDetailController(ICampaignDetailService campaignDetailService, ILogger<CampaignDetailController> logger)
        {
            _campaignDetailService = campaignDetailService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<CampaignDetailResponse>>> GetAllCampaignDetails(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignDetailRespone = await _campaignDetailService.GetCampaignDetails(searchName, page, size);
                return Ok(campaignDetailRespone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign details"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaign details");
            }
        }

        [HttpGet("get-all-campaign-detail-by-storeId")]
        public async Task<ActionResult<IPaginate<CampaignDetailResponse>>> GetAllCampaignDetailByStoreId(string storeId, string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignDetailRespone = await _campaignDetailService.GetAllCampaignDetailByStore(storeId, searchName, page, size);
                return Ok(campaignDetailRespone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign details"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaign details");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaignDetailById(string id)
        {
            var campaignDetailResponse = await _campaignDetailService.GetById(id);
            if (campaignDetailResponse == null)
            {
                return NotFound();
            }
            return Ok(campaignDetailResponse);
        }

        [HttpGet("{id}/voucher-items")]
        public async Task<IActionResult> GetVoucherItemsByCampaignDetail(string id)
        {
            var voucherItems = _campaignDetailService.GetAllVoucherItemByCampaignDetail(id);
            if (voucherItems == null)
            {
                return NotFound();
            }
            return Ok(voucherItems);
        }
    }
}
