using ResumeMaker.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.Interfaces.Services
{
    public interface ITelegramAuthService
    {
        Task<TelegramAuthResponse> InitAsync(CancellationToken token=default!);
        Task<string?> IssueCodeAsync(Guid requestId, long chatId, CancellationToken token = default);
    }
}
