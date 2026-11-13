using ResumeMaker.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.Interfaces.Services
{
    public interface ITelegramAuthService
    {
        Task<TelegramAuthResponse> InitAsync(CancellationToken token=default!);
        Task TelegramWebhookAsync(long chatId, string? text, CancellationToken token = default);
    }
}
