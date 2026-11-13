using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ResumeMaker.Application.Interfaces.Services;
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

        [HttpPost("telegram/init")]
        [EnableRateLimiting("ip-limit")]
        public async Task<IActionResult> Init()
        {
            CancellationToken token = new CancellationToken();
            var result = await _telegramAuthService.InitAsync(token);
            
                return Ok(result);
            
        }
    }
}
