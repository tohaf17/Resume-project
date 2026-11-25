//added
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Domain.Enums;

namespace ResumeMaker.Domain.Entities;

public class RegistrationPromptEvent
{
    public Guid Id { get; set; }
    public RegistrationPromptEventType EventType { get; set; }
    public string SourcePage { get; set; } = string.Empty;
    public string? TemplateSlug { get; set; }
    public string? AnonymousSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
}
