using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResumeMaker.Application.Interfaces;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Application.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ResumeMaker.Infrastructure.Services;

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
        var telegramUsername = update.Message.From?.Username;

        if (!text.StartsWith("/start ", StringComparison.OrdinalIgnoreCase))
        {
            if (text.Equals("/start", StringComparison.OrdinalIgnoreCase))
            {
                await client.SendMessage(chatId, "Для входу перейдіть за посиланням із сайту Resume Maker.", cancellationToken: ct);
            }
            return;
        }

        var payload = text["/start ".Length..].Trim();
        if (!Guid.TryParse(payload, out var requestId))
        {
            await client.SendMessage(chatId, "Заявка недійсна або прострочена. Спробуйте знову на сайті.", cancellationToken: ct);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<ITelegramAuthService>();

        var otpCode = await authService.IssueCodeAsync(requestId, telegramUsername, chatId, ct);

        if (otpCode is null)
        {
            await client.SendMessage(
                chatId,
                "Заявка недійсна або прострочена. Спробуйте знову на сайті.",
                cancellationToken: ct);
            return;
        }

        await client.SendMessage(
            chatId,
            $"Ваш код підтвердження: {otpCode}. Введіть його на сайті протягом 5 хвилин.",
            cancellationToken: ct);
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Помилка під час роботи Telegram-бота.");
        return Task.CompletedTask;
    }
}
