//added
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Infrastructure.Persistence;

namespace ResumeMaker.Infrastructure.Services;

public sealed class TemplateService : ITemplateService
{
    private readonly AppDbContext _dbContext;

    public TemplateService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TemplateSummaryResponse>> GetGalleryAsync(
        string? category = null,
        bool? isPremium = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ResumeTemplates
            .AsNoTracking()
            .Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(category))
        {
            var trimmedCategory = category.Trim();
            query = query.Where(t => t.Category == trimmedCategory);
        }

        if (isPremium.HasValue)
        {
            query = query.Where(t => t.IsPremium == isPremium.Value);
        }

        return await query
            .OrderBy(t => t.IsPremium)
            .ThenBy(t => t.Name)
            .Select(t => new TemplateSummaryResponse(
                t.Id,
                t.Name,
                t.Slug,
                t.Category,
                t.PreviewImageUrl,
                t.IsPremium))
            .ToListAsync(cancellationToken);
    }

    public async Task<TemplateDetailsResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ResumeTemplates
            .AsNoTracking()
            .Where(t => t.IsActive && t.Id == id)
            .Select(t => new TemplateDetailsResponse(
                t.Id,
                t.Name,
                t.Slug,
                t.Description,
                t.Category,
                t.PreviewImageUrl,
                t.StructureSchemaJson,
                t.IsPremium))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
