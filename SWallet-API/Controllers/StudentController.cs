using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Domain.Paginate;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request.Student;
using SWallet.Repository.Payload.Response.Student;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [Tags("🧑‍🎓Student API")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IPaginate<StudentResponse>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> GetStudentAsync(string search, bool? isAsc, int page, int size)
        {
            var result = await _studentService.GetStudentsAsync(search, isAsc, page, size);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> GetStudentAsync(string id)
        {
            var result = await _studentService.GetStudentAsync(id);
            if (result == null)
            {
                throw new ApiException("Student not found", StatusCodes.Status404NotFound, "STUDENT_NOT_FOUND");
            }
            return Ok(result);
        }

        [HttpGet("account/{id}")]
        [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [Authorize]
        public async Task<IActionResult> GetStudentByAccountId(string id)
        {
            var result = await _studentService.GetStudentByAccountIdAsync(id);
            if (result == null)
            {
                throw new ApiException("Student not found", StatusCodes.Status404NotFound, "STUDENT_NOT_FOUND");
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateStudentAsync(string id, StudentRequest studentRequest)
        {
            var result = await _studentService.UpdateStudentAsync(id, studentRequest);
            return Ok(result);
        }

        [HttpPost("validSudentEmail/{email}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> ValidEmailStudent(string email)
        {
            var result = await _studentService.ValidEmailStudent(email);
            return Ok(result);
        }

        [HttpPut("{id}/studentCardFront")]
        [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateStudentCardFront(string id, IFormFile studentCardFront)
        {
            var result = await _studentService.UpdateStudentCardFront(id, studentCardFront);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách BrandId từ Wishlists của một student dựa trên studentId.
        /// </summary>
        /// <param name="studentId">ID của student</param>
        /// <returns>Danh sách BrandId</returns>
        /// <response code="200">Trả về danh sách BrandId</response>
        /// <response code="400">Nếu studentId không hợp lệ</response>
        /// <response code="404">Nếu không tìm thấy student</response>
        /// <response code="500">Nếu có lỗi server</response>
        [HttpGet("{studentId}/wishlists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWishlistsByStudentId(string studentId)
        {
            try
            {
                // Gọi service để lấy danh sách BrandId
                var brandIds = await _studentService.GetWishlistsByStudentIdAsync(studentId);

                // Nếu danh sách rỗng, có thể coi là student không có wishlist (tùy yêu cầu)
                if (brandIds == null || !brandIds.Any())
                {
                    return NotFound(new { message = "Student not found or no wishlists available." });
                }

                return Ok(brandIds);
            }
            catch (ArgumentException ex)
            {
                // Xử lý lỗi đầu vào (studentId không hợp lệ)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi server
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}
