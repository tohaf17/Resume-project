//added
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;

namespace Resume_Maker.Controllers;

[ApiController]
[Route("api/templates")]
[AllowAnonymous]
public sealed class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplatesController(ITemplateService templateService)
    {
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TemplateSummaryResponse>>> GetGalleryAsync(
        [FromQuery] string? category = null,
        [FromQuery] bool? isPremium = null,
        CancellationToken cancellationToken = default)
    {
        var templates = await _templateService.GetGalleryAsync(category, isPremium, cancellationToken);
        return Ok(templates);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TemplateDetailsResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var template = await _templateService.GetByIdAsync(id, cancellationToken);
        if (template is null)
            return NotFound(new { message = "Шаблон резюме не знайдено." });

        return Ok(template);
    }
}
