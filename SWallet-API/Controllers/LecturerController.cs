using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Models;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.QRCodeRequest;
using SWallet.Repository.Payload.Response.Lecturer;
using SWallet.Repository.Payload.Response.QRCodeResponse;
using SWallet.Repository.Payload.Response.Store;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;
        private readonly ILogger<LecturerController> _logger;
        private readonly IQRCodeService _qrCodeService;
        

        public LecturerController(ILecturerService lecturerService, ILogger<LecturerController> logger, IQRCodeService qrCodeService)
        {
            _lecturerService = lecturerService;
            _logger = logger;
            _qrCodeService = qrCodeService;
        }

        [HttpPost]
        public async Task<ActionResult<LecturerResponse>> CreateLecturer(CreateLecturerModel creation)
        {
            try
            {
                var lecturerResponse = await _lecturerService.CreateLecturerAccount(creation);
                return Ok(lecturerResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lecturer");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating lecturer");
            }
        }

        [HttpPost("generate-qrcode")]
        public async Task<ActionResult<QRCodeResponse>> GenerateQRCode([FromBody] GenerateQRCodeRequest request)
        {
            var response = await _qrCodeService.GenerateQRCode(request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLecturerById(string id)
        {
            var lecturerResponse = await _lecturerService.GetLecturerById(id);
            if (lecturerResponse == null)
            {
                return NotFound();
            }
            return Ok(lecturerResponse);
        }


        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetLecturerByAccountId(string accountId)
        {
            var lecturerResponse = await _lecturerService.GetLecturerByAccountId(accountId);
            if (lecturerResponse == null)
            {
                return NotFound();
            }
            return Ok(lecturerResponse);
        }


        [HttpPost("scan-qrcode")]
        public async Task<IActionResult> ScanQRCode([FromBody] ScanQRCodeRequest request)
        {
            var response = await _qrCodeService.ScanQRCode(request);
            return Ok(response);
        }



        [HttpGet("campus")]
        public async Task<ActionResult<IPaginate<LecturerResponse>>> GetAllLecturerByCampusId(string campusId, string searchName = "", int page = 1, int size = 10)
        {

            var result = await _lecturerService.GetLecturersByCampusId(campusId, searchName, page, size);
            if (result == null)
            {
                return NotFound("Không tìm thấy giảng viên nào thuộc campus này.");
            }
            return Ok(result);


        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<LecturerResponse>>> GetAllLecturers(string searchName = "", int page = 1, int size = 10)
        {
            try
            {
                var result = await _lecturerService.GetLecturers(searchName, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lecturer");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting lecturer");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLecturer(string id, UpdateLecturerModel update)
        {
            try
            {
                var lecturerResponse = await _lecturerService.UpdateLecturer(id, update);
                if (lecturerResponse == null)
                {
                    return NotFound();
                }
                return Ok(lecturerResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating lecturer by ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating lecturer");
            }
        }

    }
}
