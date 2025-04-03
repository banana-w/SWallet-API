using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Campaign;
using SWallet.Repository.Payload.Response.Campaign;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignTypeController : ControllerBase
    {
        private readonly ICampaignTypeService _campaignTypeService;
        private readonly ILogger<CampaignTypeController> _logger; // Inject ILogger

        public CampaignTypeController(ICampaignTypeService campaignTypeService, ILogger<CampaignTypeController> logger) // Constructor injection
        {
            _campaignTypeService = campaignTypeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CampaignTypeResponse>> CreateCampaignType(CreateCampaignTypeModel creation)
        {
            try
            {
                var typeResponse = await _campaignTypeService.CreateCampaignType(creation);
                return Ok(typeResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating campaign type"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating campaign type"); // Return 500 Internal Server Error
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaignTypeById(string id)
        {
            var typeResponse = await _campaignTypeService.GetCampaignTypeById(id);
            if (typeResponse == null)
            {
                return NotFound();
            }
            return Ok(typeResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<CampaignTypeResponse>>> GetAllCampaignType(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var campaignType = await _campaignTypeService.GetCampaignType(searchName, page, size);
                return Ok(campaignType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating campaign type"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campaign type");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaignType(string id, UpdateCampaignTypeModel update)
        {
            try
            {
                var typeResponse = await _campaignTypeService.UpdateCampaignType(id, update);
                if (typeResponse == null)
                {
                    return NotFound();
                }
                return Ok(typeResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating campaign type by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating campaign type");
            }
        }
    }
}
