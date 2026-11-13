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
        private readonly ITelegramBotService _botService;
        private readonly TelegramBotOptions _botOptions;
        public TelegramAuthService(ITelegramAuthRepository telegramAuthRepository, ITelegramBotService botService, IOptions<TelegramBotOptions> botOptions)
        {
            _repository = telegramAuthRepository;
            _botService = botService;
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
        public async Task TelegramWebhookAsync(long chatId, string? text, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(text) || !text.StartsWith("/start ", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(text?.Trim(), "/start", StringComparison.OrdinalIgnoreCase))
                {
                    await _botService.SendMessageAsync(
                        chatId,
                        "Для входу перейдіть за посиланням із сайту Resume Maker.",
                        token);
                }
                return;
            }

            var payload = text["/start ".Length..].Trim();
            if (!Guid.TryParse(payload, out var requestId))
            {
                await _botService.SendMessageAsync(chatId, "Некоректне посилання для входу.", token);
                return;
            }
            
            var authRequest = await _repository.GetByIdAsync(requestId, token);

            if (authRequest is null ||
                authRequest.Status != StatusRequest.Pending ||
                authRequest.ExpiresAt <= DateTime.UtcNow)
            {
                await _botService.SendMessageAsync(
                    chatId,
                    "Час дії запиту вийшов або він уже використаний. Спробуйте згенерувати нове посилання на сайті.",
                    token);
                return;
            }

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var codeHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(otpCode)));

            authRequest.TelegramChatId = chatId;
            authRequest.CodeHash = codeHash;
            authRequest.Status = StatusRequest.CodeIssued;

            await _repository.SaveChangesAsync(token);

            await _botService.SendMessageAsync(
                chatId,
                $"Ваш код підтвердження для входу в Resume Maker:\n\n*{otpCode}*\n\nКод дійсний протягом 5 хвилин.",
                token);
        }
    }
}
