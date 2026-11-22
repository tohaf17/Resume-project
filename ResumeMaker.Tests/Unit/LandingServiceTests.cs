using ResumeMaker.Infrastructure.Services;
using Xunit;

namespace ResumeMaker.Tests.Unit;

public class LandingServiceTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnLandingContentWithTemplatesCta()
    {
        // Arrange
        var service = new LandingService();

        // Act
        var result = await service.GetAsync();

        // Assert
        Assert.Equal("Створити резюме", result.CtaText);
        Assert.Equal("/templates", result.CtaUrl);
        Assert.Equal("Створюй професійне резюме за 10 хвилин", result.ShortDescription);
    }
}
