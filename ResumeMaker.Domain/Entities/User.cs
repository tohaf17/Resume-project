using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ResumeMaker.Domain.Enums;

namespace ResumeMaker.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required long TelegramUserId { get; set; }
        public string? TelegramUsername { get; set; }
        
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public Guid? TariffPlanId { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        
    }
}
