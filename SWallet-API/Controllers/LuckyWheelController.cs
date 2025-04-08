using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Services.Implements;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuckyWheelController : ControllerBase
    {
        private readonly ILuckyWheelService _luckyWheelService;

        public LuckyWheelController(ILuckyWheelService luckyWheelService)
        {
            _luckyWheelService = luckyWheelService;
        }

        /// <summary>
        /// Lấy số lượt quay của Student trong một ngày cụ thể
        /// </summary>
        /// <param name="studentId">ID của Student</param>
        /// <param name="date">Ngày cần kiểm tra (định dạng YYYY-MM-DD)</param>
        /// <returns>Số lượt quay</returns>
        [HttpGet("spin-count/{studentId}/{date}")]
        public async Task<IActionResult> GetSpinCount(string studentId, string date)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest(new { message = "StudentId cannot be empty" });
            }

            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD." });
            }

            try
            {
                var spinCount = await _luckyWheelService.GetSpinCountAsync(studentId, parsedDate);
                return Ok(new { spinCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Tăng số lượt quay của Student trong ngày hiện tại
        /// </summary>
        /// <param name="request">Thông tin StudentId và ngày</param>
        /// <returns>Thông báo thành công hoặc lỗi</returns>
        [HttpPost("increment-spin-count")]
        public async Task<IActionResult> IncrementSpinCount([FromBody] SpinRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.StudentId))
            {
                return BadRequest(new { message = "StudentId cannot be empty" });
            }

            if (!DateTime.TryParse(request.Date, out var parsedDate))
            {
                return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD." });
            }

            try
            {
                await _luckyWheelService.IncrementSpinCountAsync(request.StudentId, parsedDate);
                return Ok(new { message = "Spin count incremented successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class SpinRequest
    {
        public string StudentId { get; set; }
        public string Date { get; set; }
    }
}

