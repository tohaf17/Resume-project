using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;

namespace ResumeMaker.Infrastructure.Services;

public sealed class LandingService : ILandingService
{
    public Task<LandingPageResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        var response = new LandingPageResponse(
            CtaText: "Створити резюме",
            CtaUrl: "/templates",
            ShortDescription: "Створюй професійне резюме за 10 хвилин");

        return Task.FromResult(response);
    }
}
