//added
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;

namespace ResumeMaker.Application.Interfaces.Services;

public interface IRegistrationPromptAnalyticsService
{
    Task<(TrackRegistrationPromptResponse? Response, string? Error)> TrackAsync(
        TrackRegistrationPromptRequest request,
        CancellationToken cancellationToken = default);
}
