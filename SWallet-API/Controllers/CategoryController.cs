using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.Category;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(string searchName = "", int page = 1, int size = 10)
        {
            var result = await _categoryService.GetCategory(searchName, page, size);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var result = await _categoryService.GetCateogoryById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryModel category)
        {
            var result = await _categoryService.CreateCategory(category);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromForm] UpdateCategoryModel category)
        {
            var result = await _categoryService.UpdateCategory(id, category);
            return Ok(result);
        }

        //[HttpDelete("{id}")]
        //public IActionResult Delete(string id)
        //{
        //    _categoryService.Delete(id);
        //    return Ok();
        //}
    }
}
