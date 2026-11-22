using Microsoft.AspNetCore.Mvc;
using Resume_Maker.Controllers;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;
using Xunit;

namespace ResumeMaker.Tests.Unit;

public class LandingControllerTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnOkWithLandingContent()
    {
        // Arrange
        var expected = new LandingPageResponse(
            "Створити резюме",
            "/templates",
            "Створюй професійне резюме за 10 хвилин");
        var controller = new LandingController(new StubLandingService(expected));

        // Act
        var result = await controller.GetAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expected, okResult.Value);
    }

    private sealed class StubLandingService(LandingPageResponse response) : ILandingService
    {
        public Task<LandingPageResponse> GetAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(response);
    }
}
