using System;

namespace ResumeMaker.Domain.Entities
{
    public class GuestResumeDraft
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public Guid GuestSessionId { get; set; }
        public string ContentJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public ResumeTemplate Template { get; set; } = null!;

        public bool IsExpired(DateTime utcNow) => ExpiresAt <= utcNow;
    }
}