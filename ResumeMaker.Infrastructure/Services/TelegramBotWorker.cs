using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResumeMaker.Domain.Enums;
using ResumeMaker.Domain.Options;
using ResumeMaker.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ResumeMaker.Infrastructure.Services
{
    public class TelegramBotWorker : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TelegramBotWorker> _logger;

        public TelegramBotWorker(
            IOptions<TelegramBotOptions> options,
            IServiceScopeFactory scopeFactory,
            ILogger<TelegramBotWorker> logger)
        {
            _botClient = new TelegramBotClient(options.Value.Token);
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message]
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken);

            _logger.LogInformation("Telegram бот успішно запущений у режимі Long Polling.");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ct)
        {
            if (update.Message?.Text is not { } text)
                return;

            var chatId = update.Message.Chat.Id;

            if (text.StartsWith("/start ", StringComparison.OrdinalIgnoreCase))
            {
                var payload = text["/start ".Length..].Trim();

                if (!Guid.TryParse(payload, out var requestId))
                {
                    await client.SendMessage(chatId, "Некоректне посилання для входу. Спробуйте згенерувати нове на сайті.", cancellationToken: ct);
                    return;
                }

                await ProcessAuthRequestAsync(client, chatId, requestId, ct);
            }
            else if (text.Equals("/start", StringComparison.OrdinalIgnoreCase))
            {
                await client.SendMessage(chatId, "Для входу перейдіть за посиланням із сайту Resume Maker.", cancellationToken: ct);
            }
        }

        private async Task ProcessAuthRequestAsync(ITelegramBotClient client, long chatId, Guid requestId, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var authRequest = await dbContext.TelegramAuthRequests
                .FirstOrDefaultAsync(r => r.Id == requestId, ct);

            if (authRequest is null || authRequest.ExpiresAt <= DateTime.UtcNow || authRequest.Status != StatusRequest.Pending)
            {
                await client.SendMessage(chatId, "Час дії запиту вийшов або він уже використаний. Поверніться на сайт і спробуйте ще раз.", cancellationToken: ct);
                return;
            }

            // Генерація 6-значного OTP-коду та його хешу
            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var codeHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(otpCode)));

            // Оновлення запису в БД (адаптуйте під сеттери або методи вашої сутності TelegramAuthRequest)
            authRequest.TelegramChatId = chatId;
            authRequest.CodeHash = codeHash;
            authRequest.Status = StatusRequest.CodeIssued;

            await dbContext.SaveChangesAsync(ct);

            await client.SendMessage(
                chatId,
                $"Ваш код підтвердження для входу в Resume Maker:\n\n*{otpCode}*\n\nКод дійсний протягом 5 хвилин. Нікому його не передавайте.",
                parseMode: ParseMode.Markdown,
                cancellationToken: ct);
        }

        private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ct)
        {
            _logger.LogError(exception, "Помилка під час роботи Telegram-бота.");
            return Task.CompletedTask;
        }
    }
}
