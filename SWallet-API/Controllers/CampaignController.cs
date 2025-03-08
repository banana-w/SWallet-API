using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ILogger<CampaignController> _logger; 

        public CampaignController(ICampaignService campaignService, ILogger<CampaignController> logger)
        {
            _campaignService = campaignService;
            _logger = logger;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAllCampaignDetails()
        {
            try
            {
                var campaignDetails = await _campaignService.GetAllCampaignDetails();
                return Ok(campaignDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<CampaignResponse>>> GetAllCampaigns(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignRespone = await _campaignService.GetCampaigns(searchName, page, size);
                return Ok(campaignRespone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaigns"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaigns");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampaignResponse>> CreateCampaign(CreateCampaignModel creation)
        {
            try
            {
                var campaignResponse = await _campaignService.CreateCampaign(creation);
                return Ok(campaignResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating campaign"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating campaign"); // Return 500 Internal Server Error
            }
        }


    }
}
