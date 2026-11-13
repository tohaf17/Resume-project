using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Domain.Enums;

namespace ResumeMaker.Domain.Entities
{
    public class TelegramAuthRequest
    {
        public Guid Id { get; set; }
        public StatusRequest Status { get; set; }
        public long? TelegramChatId { get; set; }
        public string? TelegramUsername { get; set; }
        public string? CodeHash { get; set; }
        public int Attempts { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(5);

        
    }
}
