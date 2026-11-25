//added
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Responses;

public sealed record StaticPageSummaryResponse(
    string Slug,
    string Title,
    DateTime UpdatedAt);

public sealed record StaticPageDetailsResponse(
    string Slug,
    string Title,
    string Content,
    DateTime UpdatedAt);
