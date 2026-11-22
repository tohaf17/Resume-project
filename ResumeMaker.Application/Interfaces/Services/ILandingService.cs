using ResumeMaker.Application.DTOs.Responses;

namespace ResumeMaker.Application.Interfaces.Services;

public interface ILandingService
{
    Task<LandingPageResponse> GetAsync(CancellationToken cancellationToken = default);
}
