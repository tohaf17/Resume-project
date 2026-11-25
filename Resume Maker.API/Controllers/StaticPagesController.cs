//added
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;

namespace Resume_Maker.Controllers;

[ApiController]
[Route("api/static-pages")]
[AllowAnonymous]
public sealed class StaticPagesController : ControllerBase
{
    private readonly IStaticPageService _staticPageService;

    public StaticPagesController(IStaticPageService staticPageService)
    {
        _staticPageService = staticPageService ?? throw new ArgumentNullException(nameof(staticPageService));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StaticPageSummaryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var pages = await _staticPageService.GetAllPublishedAsync(cancellationToken);
        return Ok(pages);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<StaticPageDetailsResponse>> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var page = await _staticPageService.GetBySlugAsync(slug, cancellationToken);
        if (page is null)
            return NotFound(new { message = $"Сторінку '{slug}' не знайдено." });

        return Ok(page);
    }
}
