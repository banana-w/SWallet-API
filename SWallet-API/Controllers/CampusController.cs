using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWallet.API.Controllers;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campus;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campus;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;
        private readonly ILogger<CampusController> _logger;

        public CampusController(ICampusService campusService, ILogger<CampusController> logger)
        {
            _campusService = campusService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CampusResponse>> CreateCampus(CreateCampusModel creation)
        {
            try
            {
                var brandResponse = await _campusService.CreateCampus(creation);
                return Ok(brandResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating admin"); // Return 500 Internal Server Error
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampus(string id, [FromForm] UpdateCampusModel campus)
        {
            var result = await _campusService.UpdateCampus(id, campus);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<CampusResponse>>> GetAllCampus(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var brandResponses = await _campusService.GetCampus(searchName, page, size);
                return Ok(brandResponses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campus");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampusById(string id)
        {
            var result = await _campusService.GetCampusById(id);
            return Ok(result);
        }

        
    }
}
