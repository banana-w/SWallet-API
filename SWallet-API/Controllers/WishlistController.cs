using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.WishList;
using SWallet.Repository.Services.Interfaces;

namespace SWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>
        /// Lấy danh sách wishlist với phân trang và bộ lọc
        /// </summary>
        /// <param name="studentIds">Danh sách ID sinh viên</param>
        /// <param name="brandIds">Danh sách ID thương hiệu</param>
        /// <param name="search">Từ khóa tìm kiếm (BrandName hoặc Description)</param>
        /// <param name="state">Trạng thái wishlist</param>
        /// <param name="size">Kích thước trang (mặc định 10)</param>
        /// <returns>Danh sách wishlist phân trang</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] List<string> studentIds,
            [FromQuery] List<string> brandIds,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            try
            {
                var wishlists = await _wishlistService.GetAll(studentIds, brandIds, search, page, size);
                return Ok(wishlists);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new
                {
                    error = ex.Message,
                    code = ex.ErrorCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật hoặc tạo mới wishlist cho sinh viên và thương hiệu
        /// </summary>
        /// <param name="update">Thông tin wishlist cần cập nhật/tạo mới</param>
        /// <returns>Wishlist đã cập nhật hoặc tạo mới</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateWishlist([FromBody] WishListUpdateModel update)
        {
            try
            {
                var wishlist = await _wishlistService.UpdateWishlist(update);
                return Ok(wishlist);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new
                {
                    error = ex.Message,
                    code = ex.ErrorCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("getWishlishBrand/{studentId}")]
        public async Task<IActionResult> GetWishlishBrand(string studentId)
        {
            try
            {
                var wishlist = await _wishlistService.GetWishlishBrandIdByStudentId(studentId);
                return Ok(wishlist);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new
                {
                    error = ex.Message,
                    code = ex.ErrorCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("getUnWishlishBrand/{studentId}")]
        public async Task<IActionResult> GetUnWishlishBrand(string studentId)
        {
            try
            {
                var wishlist = await _wishlistService.GetUnWishlishBrandIdByStudentId(studentId);
                return Ok(wishlist);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new
                {
                    error = ex.Message,
                    code = ex.ErrorCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred",
                    detail = ex.Message
                });
            }
        }
    }
}
