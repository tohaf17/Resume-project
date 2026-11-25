//added
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Infrastructure.Persistence;

namespace ResumeMaker.Infrastructure.Services;

public sealed class RegistrationPromptAnalyticsService : IRegistrationPromptAnalyticsService
{
    private readonly AppDbContext _dbContext;

    public RegistrationPromptAnalyticsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(TrackRegistrationPromptResponse? Response, string? Error)> TrackAsync(
        TrackRegistrationPromptRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(request.EventType))
            return (null, "Некоректний тип події аналітики.");

        if (string.IsNullOrWhiteSpace(request.SourcePage))
            return (null, "Поле SourcePage є обов'язковим.");

        var entity = new RegistrationPromptEvent
        {
            Id = Guid.NewGuid(),
            EventType = request.EventType,
            SourcePage = request.SourcePage.Trim(),
            TemplateSlug = string.IsNullOrWhiteSpace(request.TemplateSlug)
                ? null
                : request.TemplateSlug.Trim(),
            AnonymousSessionId = string.IsNullOrWhiteSpace(request.AnonymousSessionId)
                ? null
                : request.AnonymousSessionId.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RegistrationPromptEvents.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (new TrackRegistrationPromptResponse(entity.Id, entity.CreatedAt), null);
    }
}
