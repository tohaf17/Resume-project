namespace ResumeMaker.Application.DTOs.Responses;

public sealed record LandingPageResponse(
    string CtaText,
    string CtaUrl,
    string ShortDescription);