//added
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Domain.Enums;

namespace ResumeMaker.Application.DTOs.Request;

public sealed record TrackRegistrationPromptRequest(
    RegistrationPromptEventType EventType,
    string SourcePage,
    string? TemplateSlug = null,
    string? AnonymousSessionId = null);
