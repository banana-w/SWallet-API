using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.Request.Wallet;
using SWallet.Repository.Payload.Response.Wallet;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddWallet([FromBody] WalletRequest walletRequest)
        {
            var result = await _walletService.AddWallet(walletRequest);
            return Ok(result);
        }

        [HttpGet("{studentId}/{type}")]
        [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWalletByStudentId(string studentId, int type)
        {
            var wallet = await _walletService.GetWalletByStudentId(studentId, type);
            return Ok(wallet);
        }

        [HttpPut("{id}/{balance}")]
        [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateWallet(string id, decimal balance)
        {
            var wallet = await _walletService.UpdateWallet(id, balance);
            return Ok(wallet);
        }

        [HttpPost("add-points-to-brand-wallet")]
        public async Task<IActionResult> AddPointsToBrandWallet(string brandId, int points)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(brandId))
                {
                    return BadRequest(new { error = "BrandId is required" });
                }
                if (points <= 0)
                {
                    return BadRequest(new { error = "Points must be greater than 0" });
                }

                // Gọi service để cộng điểm vào wallet của Brand
                await _walletService.AddPointsToBrandWallet(brandId, points);

                // Trả về phản hồi thành công
                return Ok(new { message = "Points added to Brand wallet successfully" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return StatusCode(500, new { error = "An error occurred while adding points", details = ex.Message });
            }
        }

        [HttpPost("student")]
        public async Task<IActionResult> AddPointsToStudentWallet(string studentId, int points)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(studentId))
                {
                    return BadRequest(new { error = "StudentId is required" });
                }
                if (points <= 0)
                {
                    return BadRequest(new { error = "Points must be greater than 0" });
                }

                // Gọi service để cộng điểm vào wallet của Brand
                await _walletService.AddPointsToStudentWallet(studentId, points);

                // Trả về phản hồi thành công
                return Ok(new { message = "Points added to Brand wallet successfully" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return StatusCode(500, new { error = "An error occurred while adding points", details = ex.Message });
            }
        }
    }
}
