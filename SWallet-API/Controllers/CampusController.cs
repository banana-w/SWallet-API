using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWallet.API.Controllers;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Request.Campus;
using SWallet.Repository.Payload.Request.DistributionPoint;
using SWallet.Repository.Payload.Request.PointPackage;
using SWallet.Repository.Payload.Response.Brand;
using SWallet.Repository.Payload.Response.Campus;
using SWallet.Repository.Services.Implements;
using SWallet.Repository.Services.Interfaces;
using VNPAY.NET;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;
        private readonly IWalletService _walletService;
        private readonly ILecturerService _lecturerService;
        private readonly IPointPackageService _pointPackageService;
        private readonly ILogger<CampusController> _logger;
        private readonly IVnpay _vnPayService;

        public CampusController(ICampusService campusService, ILogger<CampusController> logger, IPointPackageService pointPackageService, IVnpay vnPayService, IWalletService walletService, ILecturerService lecturerService)
        {
            _campusService = campusService;
            _pointPackageService = pointPackageService;
            _logger = logger;
            _vnPayService = vnPayService;
            _walletService = walletService;
            _lecturerService = lecturerService;
        }


        [HttpPost("distribute-points")]
        public async Task<IActionResult> DistributePoints([FromQuery]PointDistributionRequest request)
        {
            var campus = await _campusService.GetCampusById(request.CampusId);
            if (campus == null)
            {
                return NotFound("Không tìm thấy campus.");
            }

            var campusWallet = await _walletService.GetWalletByCampusId(request.CampusId, 1);
            if (campusWallet == null)
            {
                return NotFound("Campus wallet not found.");
            }

            if (campusWallet.Balance < request.Points * request.lecturerIds.Count)
            {
                return BadRequest("Số điểm trong wallet của campus không đủ để phân phối.");
            }

            foreach (var lecturerId in request.lecturerIds)
            {
                var lecturer = await _lecturerService.GetLecturerById(lecturerId);
                if (lecturer == null)
                {
                    return NotFound($"Không tìm thấy giảng viên với ID: {lecturerId}");
                }

                var lecturerWallet = await _walletService.GetWalletByLecturerId(lecturerId, 1); // Assuming each lecturer has one wallet
                if (lecturerWallet == null)
                {
                    return NotFound($"Không tìm thấy wallet của giảng viên với ID: {lecturerId}");
                }

                // Cộng điểm vào wallet của lecturer
                await _walletService.UpdateWallet(lecturerWallet.Id, (decimal)(lecturerWallet.Balance + request.Points));
            }

            // Trừ điểm từ wallet của campus
            await _walletService.UpdateWallet(campusWallet.Id, (decimal)(campusWallet.Balance - (request.Points * request.lecturerIds.Count)));

            return Ok("Phân phối điểm thành công.");



            //int totalPointsToDistribute = 0;
            //foreach (var lecturerDistribution in request.Lecturers)
            //{
            //    totalPointsToDistribute += lecturerDistribution.Points;
            //}

            //if (campusWallet.Balance < totalPointsToDistribute)
            //{
            //    return BadRequest("Số điểm trong wallet của campus không đủ để phân phối.");
            //}

            //foreach (var lecturerDistribution in request.Lecturers)
            //{
            //    var lecturer = await _lecturerService.GetLecturerById(lecturerDistribution.LecturerId);
            //    if (lecturer == null)
            //    {
            //        return NotFound($"Không tìm thấy giảng viên với ID: {lecturerDistribution.LecturerId}");
            //    }

            //    var  lecturerWallet = await _walletService.GetWalletByLecturerId(lecturerDistribution.LecturerId, 1); // Assuming each lecturer has one wallet
            //    if (lecturerWallet == null)
            //    {
            //        return NotFound($"Không tìm thấy wallet của giảng viên với ID: {lecturerDistribution.LecturerId}");
            //    }

            //    // Cộng điểm vào wallet của lecturer
            //    await _walletService.UpdateWallet(lecturerWallet.Id, (decimal)(lecturerWallet.Balance + lecturerDistribution.Points));
            //}


        }



        [HttpPost]
        public async Task<ActionResult<CampusResponse>> CreateCampus(CreateCampusModel creation)
        {
            try
            {
                var brandResponse = await _campusService.CreateCampus(creation);
                return Ok(brandResponse); // Return 201 Created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin"); // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating admin"); // Return 500 Internal Server Error
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampus(string id, [FromForm] UpdateCampusModel campus)
        {
            var result = await _campusService.UpdateCampus(id, campus);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<CampusResponse>>> GetAllCampus(string searchName = "", int page = 1, int size = 10) // Pagination parameters
        {
            try
            {
                var brandResponses = await _campusService.GetCampus(searchName, page, size);
                return Ok(brandResponses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting campus");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampusById(string id)
        {
            var result = await _campusService.GetCampusById(id);
            return Ok(result);
        }

        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetCampusByAccountId(string id)
        {
            var result = await _campusService.GetCampusByAccountId(id);
            return Ok(result);
        }


    }
}
