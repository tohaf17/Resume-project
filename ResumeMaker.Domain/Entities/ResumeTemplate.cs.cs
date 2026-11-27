using System;

namespace ResumeMaker.Domain.Entities
{
    public class ResumeTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}