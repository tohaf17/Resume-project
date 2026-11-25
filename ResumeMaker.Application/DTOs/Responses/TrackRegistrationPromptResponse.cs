//added
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Responses;

public sealed record TrackRegistrationPromptResponse(
    Guid EventId,
    DateTime RecordedAt);
