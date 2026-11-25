using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;

namespace Resume_Maker.Controllers;

[ApiController]
[Route("api/landing")]
[AllowAnonymous]
public sealed class LandingController : ControllerBase
{
    private readonly ILandingService _landingService;

    public LandingController(ILandingService landingService)
    {
        _landingService = landingService ?? throw new ArgumentNullException(nameof(landingService));
    }

    [HttpGet]
    public async Task<ActionResult<LandingPageResponse>> GetAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _landingService.GetAsync(cancellationToken);
        return Ok(response);
    }
}