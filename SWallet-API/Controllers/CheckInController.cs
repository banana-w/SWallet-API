using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Services.Implements;
using System.Text.Json;

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


        [HttpPost("qr")]
        public async Task<IActionResult> CheckInWithQR([FromBody] CheckInQrRequest request)
        {
            var (success, message, pointsAwarded) = await _checkInService.CheckInWithQR(
                request.StudentId,
                request.QrCode,
                request.Latitude,
                request.Longitude
            );

            if (success)
            {
                return Ok(new { message, pointsAwarded });
            }
            return BadRequest(new { message });
        }

        [HttpGet("get-check-in-data/{studentId}")]
        public async Task<IActionResult> GetCheckInData(string studentId)
        {
            try
            {
                var result = await _checkInService.GetCheckInDataAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("check-in/{studentId}")]
        public async Task<IActionResult> CheckIn(string studentId)
        {
            try
            {
                var result = await _checkInService.CheckInAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
