using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
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

        private readonly IJwtService jwtService;

        public CampaignController(
            ICampaignService campaignService)
        {
            _campaignService = campaignService;
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
