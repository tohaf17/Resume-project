using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Application.DTOs.Request;
namespace Resume_Maker.Controllers
{
    [ApiController]
    [Route("api/auth/telegram")]
    public class TelegramAuthController : Controller
    {
        private readonly ITelegramAuthService _telegramAuthService;

        public TelegramAuthController(ITelegramAuthService telegramAuthService)
        {
            _telegramAuthService = telegramAuthService;
        }

        [HttpPost("init")]
        [EnableRateLimiting("ip-limit")]
        public async Task<IActionResult> Init(CancellationToken token=default)
        {
            var result = await _telegramAuthService.InitAsync(token);
            
                return Ok(result);
            
        }
        [HttpGet("{requestId:guid}/status")]
        public async Task<IActionResult> GetStatus(Guid requestId,CancellationToken token=default)
        {
            var result = await _telegramAuthService.GetStatusAsync(requestId, token);
            if (result is null)
            {
                return NotFound(new { message = "Заявку авторизації не знайдено." });
            }
            return Ok(result);
        }
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAuth([FromBody] VerifyTelegramAuthRequest request,CancellationToken token=default!)
        {
            var (response, error) = await _telegramAuthService.VerifyAuthAsync(request, token);
            if (error != null)
            {
                return NotFound(new { message = error });
            }
            return Ok(response);
        }
    }
}
