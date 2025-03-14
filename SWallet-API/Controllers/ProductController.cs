using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Product;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Product;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly Mapper _mapper;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct(CreateProductModel creation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi validation
            }

            try
            {
                var result = await _productService.CreateProduct(creation);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var productResponse = await _productService.GetProductById(id);
            if (productResponse == null)
            {
                return NotFound();
            }
            return Ok(productResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<ProductResponse>>> GetAllProducts(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var productResponse = await _productService.GetProducts(searchName, page, size);
                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting products");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, UpdateProductModel update)
        {
            try
            {
                var productResponse = await _productService.UpdateProduct(id, update);
                if (productResponse == null)
                {
                    return NotFound();
                }
                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating product");
            }
        }
    }
}
