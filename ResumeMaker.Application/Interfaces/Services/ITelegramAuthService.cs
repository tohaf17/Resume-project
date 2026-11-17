using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.Interfaces.Services
{
    public interface ITelegramAuthService
    {
        Task<TelegramAuthResponse> InitAsync(CancellationToken token=default!);
        Task<string?> IssueCodeAsync(Guid requestId, string? telegramUsername, long chatId, CancellationToken token = default);
        Task<TelegramAuthStatusResponse?> GetStatusAsync(Guid requestId, CancellationToken token=default);
        Task<(VerifyTelegramAuthResponse? response, string? error)> VerifyAuthAsync(VerifyTelegramAuthRequest request, CancellationToken token = default);
        Task<(RefreshTokenResponse? Response, string? Error)> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken token = default);
        Task<bool> LogoutAsync(Guid userId, string? refreshToken, CancellationToken token = default);
    }
}
