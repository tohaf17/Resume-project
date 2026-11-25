//added
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Application.DTOs.Responses;

namespace ResumeMaker.Application.Interfaces.Services;

public interface IStaticPageService
{
    Task<IReadOnlyList<StaticPageSummaryResponse>> GetAllPublishedAsync(
        CancellationToken cancellationToken = default);

    Task<StaticPageDetailsResponse?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);
}
