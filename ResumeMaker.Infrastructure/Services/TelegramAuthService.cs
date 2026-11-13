using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ResumeMaker.Application.DTOs;
using ResumeMaker.Application.Interfaces.Repositories;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Domain.Enums;
using ResumeMaker.Domain.Options;
using ResumeMaker.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace ResumeMaker.Infrastructure.Services
{
    public class TelegramAuthService : ITelegramAuthService
    {
        private readonly ITelegramAuthRepository _repository;
        private readonly TelegramBotOptions _botOptions;
        public TelegramAuthService(ITelegramAuthRepository telegramAuthRepository, IOptions<TelegramBotOptions> botOptions)
        {
            _repository = telegramAuthRepository;
            _botOptions = botOptions.Value;
        }
        public async Task<TelegramAuthResponse> InitAsync(CancellationToken token = default!)
        {
            var request = new TelegramAuthRequest();

            request.Status = StatusRequest.Pending;
            await _repository.AddAsync(request, token);
            await _repository.SaveChangesAsync(token);

            var deepLink = $"https://t.me/{_botOptions.Username}?start={request.Id}";
            var expiresInSeconds = (int)(request.ExpiresAt - request.CreatedAt).TotalSeconds;
            return new TelegramAuthResponse(
                RequestId: request.Id,
                TelegramDeepLink: deepLink,
                ExpiresInSeconds: expiresInSeconds);
        }
        public async Task<string?> IssueCodeAsync(Guid requestId, long chatId, CancellationToken token = default)
        {
            var authRequest = await _repository.GetByIdAsync(requestId, token);

            if (authRequest is null ||
                authRequest.Status != StatusRequest.Pending ||
                authRequest.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var codeHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(otpCode)));

            authRequest.TelegramChatId = chatId;
            authRequest.CodeHash = codeHash;
            authRequest.Status = StatusRequest.CodeIssued;

            await _repository.SaveChangesAsync(token);

            return otpCode;
        }

    }
}
