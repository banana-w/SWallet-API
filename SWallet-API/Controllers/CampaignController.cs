using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Payload.Response.Store;
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

        [HttpGet("getStoreByCampaignId/{campaignId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<IPaginate<StoreResponse>>> GetAllStoresByCampaignId(string campaignId, string searchName = "", int page = 1, int size = 10)
        {
            try
            {
                var result = await _campaignService.GetStoresByCampaignId(campaignId, searchName, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stores");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting stores");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<CampaignResponse>>> GetCampaignsInBrand(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignRespone = await _campaignService.GetCampaignsInBrand(searchName, page, size);
                return Ok(campaignRespone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaigns"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaigns");
            }
        }
        [HttpGet("brand/{brandId}")]
        [ProducesResponseType(typeof(IPaginate<CampaignResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IPaginate<CampaignResponse>>> GetCampaignsByBrandId(string brandId, string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignRespone = await _campaignService.GetCampaignsByBrandId(brandId, searchName, page, size);
                return Ok(campaignRespone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaigns"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaigns");
            }
        }

        [HttpGet("brandAllStatus/{brandId}")]
        [ProducesResponseType(typeof(IPaginate<CampaignResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IPaginate<CampaignResponse>>> GetCampaignsByBrandIdAllStatus(string brandId, string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignRespone = await _campaignService.GetCampaignsByBrandIdAllStatus(brandId, searchName, page, size);
                return Ok(campaignRespone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaigns"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaigns");
            }
        }


        [HttpGet("getAll")]
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

        [HttpGet("getCampaignsAllStatus")]
        [ProducesResponseType(typeof(CampaignResponseAllStatus), 200)]
        public async Task<ActionResult<IPaginate<CampaignResponse>>> GetAllCampaignsAll(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignRespone = await _campaignService.GetCampaignsAll(searchName, page, size);
                return Ok(campaignRespone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaigns"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaigns");
            }
        }

        [HttpGet("ranking/{brandId}")]
        [ProducesResponseType(typeof(List<CampaignRankingResponse>), 200)]
        public async Task<IActionResult> GetRanking(string brandId, int limit)
        {
                var ranking = await _campaignService.GetRankingByVoucherRatio(brandId, limit);
                return Ok(ranking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaign(string id, UpdateCampaignModel update)
        {
            try
            {
                var campaignResponse = await _campaignService.UpdateCampaign(id, update);
                if (campaignResponse == null)
                {
                    return NotFound();
                }
                return Ok(campaignResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating campaign by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating campaign");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampaignResponse>> CreateCampaign(CreateCampaignModel creation, [FromForm] string campaignDetails = "[{   \"voucherId\": \"Abc\",   \"quantity\": 1,   \"fromIndex\": 1,   \"description\": \"test 943\",   \"state\": true }]")
        {
            try
            {
                var detail = JsonConvert.DeserializeObject<List<CreateCampaignDetailModel>>(campaignDetails);

                var campaignResponse = await _campaignService.CreateCampaign(creation, detail!);

                return Ok(campaignResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating campaign"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating campaign"); // Return 500 Internal Server Error
            }
        }

        [HttpPut("approve-camp")]
        [ProducesResponseType(typeof(CampaignResponse), 200)]
        public async Task<IActionResult> ApproveCampaign(string campaignId, bool isApproved, string? rejectionReason = null)
        {
            var result = await _campaignService.ApproveOrRejectCampaign(campaignId, isApproved, rejectionReason);
            return Ok(result);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CampaignResponseExtra),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCampaignById(string id)
        {
            var typeResponse = await _campaignService.GetCampaignById(id);
            if (typeResponse == null)
            { 
                return NotFound();
            }
            return Ok(typeResponse);
        }
        
        [HttpGet("{id}/allStatus")]
        [ProducesResponseType(typeof(CampaignResponseExtraAllStatus),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCampaignByIdAllStatus(string id)
        {
            var typeResponse = await _campaignService.GetCampaignByIdAllStatus(id);
            if (typeResponse == null)
            { 
                return NotFound();
            }
            return Ok(typeResponse);
        }

    }
}
