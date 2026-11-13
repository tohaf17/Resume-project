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
        public IActionResult Init(string code)
        {
            throw new NotImplementedException("This endpoint is not implemented yet.");
            //var result = _telegramAuthService.Init(code);
            //if (result.IsSuccess)
            //{
            //    return Ok(result);
            //}
            //else
            //{
            //    return BadRequest(result);
            //}
        }
    }
}
