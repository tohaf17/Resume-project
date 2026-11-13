using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Domain.Options
{
    public class TelegramBotOptions
    {
        public const string SectionName = "TelegramBot";
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
