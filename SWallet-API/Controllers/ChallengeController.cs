using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWallet.Repository.Enums;
using SWallet.Repository.Payload.ExceptionModels;
using SWallet.Repository.Payload.Request;
using SWallet.Repository.Payload.Response.Challenge;
using SWallet.Repository.Services.Interfaces;

namespace SWallet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly IChallengeService _challengeService;
        public ChallengeController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(ChallengeResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateChallenge([FromForm] ChallengeRequest request)
        {
            var result = await _challengeService.CreateChallenge(request);
            if (result)
            {
                return Ok(result);
            }
            throw new ApiException("Create challenge fail", 400, "CHALLENGE_FAIL");
        }
        [HttpGet]
        public async Task<IActionResult> GetChallenges(string? search,[FromQuery] IEnumerable<ChallengeType> types, int page, int size)
        {
            var result = await _challengeService.GetChallenges(search, types, page, size);
            return Ok(result);
        }

        [HttpGet("extra")]
        [ProducesResponseType(typeof(ChallengeResponseExtra), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStudentChallenges(string studentId, string? search, [FromQuery] IEnumerable<ChallengeType> types, int page, int size)
        {
            var result = await _challengeService.GetStudentChallenges(studentId, search, types, page, size);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetChallenge(string id)
        {
            var result = await _challengeService.GetChallenge(id);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChallenge(string id, [FromForm] ChallengeRequest request)
        {
            var result = await _challengeService.UpdateChallenge(id, request);
            if (result)
            {
                return Ok(result);
            }
            throw new ApiException("Update challenge fail", 400, "CHALLENGE_FAIL");
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteChallenge(string id)
        //{
        //    var result = await _challengeService.DeleteChallenge(id);
        //    if (result)
        //    {
        //        return Ok();
        //    }
        //    return BadRequest();
        //}
    }
}
