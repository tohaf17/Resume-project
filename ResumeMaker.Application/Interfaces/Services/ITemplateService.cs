//added
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Application.DTOs.Responses;

namespace ResumeMaker.Application.Interfaces.Services;

public interface ITemplateService
{
    Task<IReadOnlyList<TemplateSummaryResponse>> GetGalleryAsync(
        string? category = null,
        bool? isPremium = null,
        CancellationToken cancellationToken = default);

    Task<TemplateDetailsResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
