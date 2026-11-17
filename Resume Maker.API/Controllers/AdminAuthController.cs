using Microsoft.AspNetCore.Mvc;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.Interfaces.Services;

namespace Resume_Maker.Controllers
{
    [ApiController]
    [Route("api/admin/auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAdminAuthService _adminAuthService;

        public AdminAuthController(IAdminAuthService adminAuthService)
        {
            _adminAuthService = adminAuthService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] AdminLoginRequest request,
            CancellationToken cancellationToken)
        {
            var (response, error) = await _adminAuthService.LoginAsync(request, cancellationToken);

            if (response is null)
                return Unauthorized(new { message = error });

            return Ok(response);
        }
    }
}
