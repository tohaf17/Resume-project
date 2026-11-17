using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Application.DTOs.Request;
namespace Resume_Maker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class TelegramAuthController : Controller
    {
        private readonly ITelegramAuthService _telegramAuthService;

        public TelegramAuthController(ITelegramAuthService telegramAuthService)
        {
            _telegramAuthService = telegramAuthService;
        }

        [HttpPost("telegram/init")]
        [EnableRateLimiting("ip-limit")]
        public async Task<IActionResult> Init(CancellationToken token=default)
        {
            var result = await _telegramAuthService.InitAsync(token);
            
                return Ok(result);
            
        }
        [HttpGet("telegram/{requestId:guid}/status")]
        public async Task<IActionResult> GetStatus(Guid requestId,CancellationToken token=default)
        {
            var result = await _telegramAuthService.GetStatusAsync(requestId, token);
            if (result is null)
            {
                return NotFound(new { message = "Заявку авторизації не знайдено." });
            }
            return Ok(result);
        }
        [HttpPost("telegram/verify")]
        public async Task<IActionResult> VerifyAuth([FromBody] VerifyTelegramAuthRequest request,CancellationToken token=default!)
        {
            var (response, error) = await _telegramAuthService.VerifyAuthAsync(request, token);
            if (error != null)
            {
                return NotFound(new { message = error });
            }
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken token = default!)
        {
            var (response,error) = await _telegramAuthService.RefreshTokenAsync(request, token);
            if (response is null)
                return Unauthorized(new { message = error });

            return Ok(response);
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken token = default!)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var success = await _telegramAuthService.LogoutAsync(userId, request.RefreshToken, token);
            if (!success) 
            { 
                return BadRequest(new { message = "Недійсний або вже відкликаний refresh token." });
            }
            return Ok(new { message = "Вихід виконано успішно." });
        }

    }
}
