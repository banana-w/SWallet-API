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
    }
}
