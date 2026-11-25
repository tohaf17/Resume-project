//added
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Infrastructure.Persistence;

namespace ResumeMaker.Infrastructure.Services;

public sealed class StaticPageService : IStaticPageService
{
    private readonly AppDbContext _dbContext;

    public StaticPageService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<StaticPageSummaryResponse>> GetAllPublishedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.StaticPages
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderBy(p => p.Title)
            .Select(p => new StaticPageSummaryResponse(
                p.Slug,
                p.Title,
                p.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<StaticPageDetailsResponse?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        var normalizedSlug = slug.Trim().ToLowerInvariant();

        return await _dbContext.StaticPages
            .AsNoTracking()
            .Where(p => p.IsPublished && p.Slug == normalizedSlug)
            .Select(p => new StaticPageDetailsResponse(
                p.Slug,
                p.Title,
                p.Content,
                p.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
