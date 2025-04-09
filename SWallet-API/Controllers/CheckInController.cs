using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Services.Implements;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost("gps")]
        public async Task<IActionResult> CheckInWithGPS([FromBody] CheckInGpsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ" });
            }

            try
            {
                bool result = await _checkInService.CheckInWithGPS(
                    request.StudentId,
                    request.LocationId,
                    request.Latitude,
                    request.Longitude
                );

                if (result)
                {
                    return Ok(new { message = "Check-in thành công", pointsAwarded = 10 });
                }
                else
                {
                    return BadRequest(new { message = "Check-in thất bại: Bạn không ở gần địa điểm này" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPost("qr")]
        public async Task<IActionResult> CheckInWithQR([FromBody] CheckInQrRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ" });
            }

            try
            {
                bool result = await _checkInService.CheckInWithQR(
                    request.StudentId,
                    request.QrCode
                );

                if (result)
                {
                    return Ok(new { message = "Check-in thành công", pointsAwarded = 10 });
                }
                else
                {
                    return BadRequest(new { message = "Check-in thất bại: Mã QR không hợp lệ" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }
    }

    public class CheckInGpsRequest
    {
        public string StudentId { get; set; }
        public string LocationId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CheckInQrRequest
    {
        public string StudentId { get; set; }
        public string QrCode { get; set; }
    }
}
