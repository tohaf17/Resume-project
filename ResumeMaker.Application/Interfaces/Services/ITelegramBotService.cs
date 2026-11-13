using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.Interfaces.Services
{
    public interface ITelegramBotService
    {
        Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default);
    }
}
