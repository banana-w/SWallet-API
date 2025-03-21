using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.PointPackage;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointPackageController : ControllerBase
    {
        private readonly IPointPackageService _pointPackageService;
        private readonly ILogger<PointPackageController> _logger;


        public PointPackageController(IPointPackageService pointPackageService, ILogger<PointPackageController> logger)
        {
            _pointPackageService = pointPackageService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<PointPackage>> CreatePointPackage(PointPackageModel creation)
        {
            try
            {
                var brandResponse = await _pointPackageService.CreatePointPackage(creation);
                return Ok(brandResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating package"); // Return 500 Internal Server Error
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackgeById(string id)
        {
            var brandResponse = await _pointPackageService.GetPointPackageById(id);
            if (brandResponse == null)
            {
                return NotFound();
            }
            return Ok(brandResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<PointPackage>>> GetAllPackages(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var brandResponses = await _pointPackageService.GetPointPackage(searchName, page, size);
                return Ok(brandResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting package");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePointPackage(string id, PointPackageModel update)
        {
            try
            {
                var brandResponse = await _pointPackageService.UpdatePointPackage(id, update);
                if (brandResponse == null)
                {
                    return NotFound();
                }
                return Ok(brandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating package by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating package");
            }
        }
    }
}
