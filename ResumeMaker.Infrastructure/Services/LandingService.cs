using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;

namespace ResumeMaker.Infrastructure.Services;

public sealed class LandingService : ILandingService
{
    private const string CtaText = "Створити резюме";
    private const string CtaUrl = "/templates";
    private const string ShortDescription = "Створюй професійне резюме за 10 хвилин";

    public Task<LandingPageResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        var response = new LandingPageResponse(
            CtaText,
            CtaUrl,
            ShortDescription);

        return Task.FromResult(response);
    }
}
