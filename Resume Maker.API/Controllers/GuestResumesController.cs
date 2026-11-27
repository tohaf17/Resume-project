using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.Interfaces.Services;

namespace Resume_Maker.Controllers
{
    [ApiController]
    [Route("api/guest/resumes")]
    public class GuestResumesController : ControllerBase
    {
        private const string GuestSessionCookieName = "GuestSessionId";
        private readonly IGuestResumeService _guestResumeService;

        public GuestResumesController(IGuestResumeService guestResumeService)
        {
            _guestResumeService = guestResumeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDraft(
            [FromBody] CreateGuestResumeRequest request,
            CancellationToken cancellationToken = default)
        {
            Guid? existingSessionId = null;
            if (Request.Cookies.TryGetValue(GuestSessionCookieName, out var cookieVal) &&
                Guid.TryParse(cookieVal, out var parsedId))
            {
                existingSessionId = parsedId;
            }

            var result = await _guestResumeService.CreateDraftAsync(request, existingSessionId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            var (response, guestSessionId) = result.Data;

            Response.Cookies.Append(GuestSessionCookieName, guestSessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = response.ExpiresAt
            });

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("{id:guid}")]
        [RequestSizeLimit(100 * 1024)]
        public async Task<IActionResult> UpdateDraft(
            Guid id,
            [FromBody] UpdateGuestResumeRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!Request.Cookies.TryGetValue(GuestSessionCookieName, out var cookieVal) ||
                !Guid.TryParse(cookieVal, out var guestSessionId))
            {
                return Unauthorized(new { message = "Відсутня або недійсна гостьова сесія." });
            }

            var result = await _guestResumeService.UpdateDraftAsync(id, guestSessionId, request, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    GuestResumeErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                    GuestResumeErrorType.Gone => StatusCode(StatusCodes.Status410Gone, new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }
    }
}