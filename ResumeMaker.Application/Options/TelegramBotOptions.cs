namespace ResumeMaker.Application.Options;

public sealed class TelegramBotOptions
{
    public const string SectionName = "TelegramBot";
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
