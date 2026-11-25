//added
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;

namespace Resume_Maker.Controllers;

[ApiController]
[Route("api/analytics/registration-prompt")]
[AllowAnonymous]
public sealed class RegistrationPromptAnalyticsController : ControllerBase
{
    private readonly IRegistrationPromptAnalyticsService _analyticsService;

    public RegistrationPromptAnalyticsController(IRegistrationPromptAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
    }

    [HttpPost]
    public async Task<ActionResult<TrackRegistrationPromptResponse>> TrackAsync(
        [FromBody] TrackRegistrationPromptRequest request,
        CancellationToken cancellationToken = default)
    {
        var (response, error) = await _analyticsService.TrackAsync(request, cancellationToken);
        if (error is not null)
            return BadRequest(new { message = error });

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
