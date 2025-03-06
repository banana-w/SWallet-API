using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.Request.Account;
using SWallet.Repository.Payload.Request.Brand;
using SWallet.Repository.Payload.Response.Lecturer;
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

        public LecturerController(ILecturerService lecturerService, ILogger<LecturerController> logger)
        {
            _lecturerService = lecturerService;
            _logger = logger;
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
