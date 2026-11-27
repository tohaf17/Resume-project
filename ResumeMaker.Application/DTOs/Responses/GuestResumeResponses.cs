using System;
using ResumeMaker.Application.DTOs.Request;

namespace ResumeMaker.Application.DTOs.Responses
{
    public class CreateGuestResumeResponse
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class GuestResumeDraftResponse
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public GuestResumeContentDto Content { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}