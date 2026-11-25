//added
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Responses;

public sealed record TemplateSummaryResponse(
    Guid Id,
    string Name,
    string Slug,
    string Category,
    string PreviewImageUrl,
    bool IsPremium);

public sealed record TemplateDetailsResponse(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string Category,
    string PreviewImageUrl,
    string StructureSchemaJson,
    bool IsPremium);