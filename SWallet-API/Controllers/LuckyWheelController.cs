using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Services.Implements;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuckyWheelController : ControllerBase
    {
        private readonly ILuckyWheelService _luckyWheelService;
        private readonly ILogger<LuckyWheelController> _logger;

        public LuckyWheelController(ILuckyWheelService luckyWheelService, ILogger<LuckyWheelController> logger)
        {
            _luckyWheelService = luckyWheelService;
            _logger = logger;
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
        [HttpGet("bonus-spins/{studentId}/{date}")]
        public async Task<IActionResult> GetBonusSpins(string studentId, string date)
        {
            try
            {
                var parsedDate = DateTime.Parse(date);
                var bonusSpins = await _luckyWheelService.GetBonusSpinsAsync(studentId, parsedDate);
                return Ok(new { bonusSpins });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetBonusSpins for studentId: {studentId}, date: {date}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // POST: api/LuckyWheel/increment-bonus-spins
        [HttpPost("increment-bonus-spins")]
        public async Task<IActionResult> IncrementBonusSpins([FromBody] SpinRequest request)
        {
            try
            {
                var parsedDate = DateTime.Parse(request.Date);
                await _luckyWheelService.IncrementBonusSpinsAsync(request.StudentId, parsedDate);
                return Ok(new { message = "Bonus spins incremented successfully" });
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in IncrementBonusSpins for studentId: {request.StudentId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }

    public class SpinRequest
    {
        public string StudentId { get; set; }
        public string Date { get; set; }
    }
}

