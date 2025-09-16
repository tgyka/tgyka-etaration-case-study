using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace EterationCaseStudy.IntegrationTest;

public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public HealthEndpointTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Health_Should_Return_Healthy()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/health");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var text = await resp.Content.ReadAsStringAsync();
        text.Should().Contain("Healthy");
    }
}

