using Microsoft.Extensions.Options;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ResumeMaker.Infrastructure.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramBotService(IOptions<TelegramBotOptions> options)
        {
            _botClient = new TelegramBotClient(options.Value.Token);
        }

        public async Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default)
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: text,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }
    }
}
