using Microsoft.AspNetCore.Mvc;
using ResumeMaker.Application.Interfaces.Services;
using Telegram.Bot.Types;

namespace Resume_Maker.Controllers
{
    [ApiController]
    [Route("api/telegram")]
    public class TelegramController : Controller
    {
        private readonly ITelegramAuthService _telegramAuthService;

        public TelegramController(ITelegramAuthService telegramAuthService)
        {
            _telegramAuthService = telegramAuthService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { Chat.Id: var chatId, Text: var text })
            {
                await _telegramAuthService.TelegramWebhookAsync(chatId, text, cancellationToken);
            }

            return Ok();
        }
    }
}
