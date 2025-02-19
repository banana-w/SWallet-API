using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Admin;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using System.Threading.Tasks;

namespace SWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<BrandController> _logger; // Inject ILogger

        public BrandController(IBrandService brandService, ILogger<BrandController> logger) // Constructor injection
        {
            _brandService = brandService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<BrandResponse>> CreateBrand(CreateBrandModel creation)
        {
            try
            {
                var brandResponse = await _brandService.CreateBrand(creation);
                return Ok(brandResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating admin"); // Return 500 Internal Server Error
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(string id)
        {
            var brandResponse = await _brandService.GetBrandById(id);
            if (brandResponse == null)
            {
                return NotFound();
            }
            return Ok(brandResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<BrandResponse>>> GetAllBrands(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var brandResponses = await _brandService.GetBrands(searchName, page, size);
                return Ok(brandResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting admins");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(string id, UpdateBrandModel update)
        {
            try
            {
                var brandResponse = await _brandService.UpdateBrand(id, update);
                if (brandResponse == null)
                {
                    return NotFound();
                }
                return Ok(brandResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating brand by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating brand");
            }
        }

    }
}