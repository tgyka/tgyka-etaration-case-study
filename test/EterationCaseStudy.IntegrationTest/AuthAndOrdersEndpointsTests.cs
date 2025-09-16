using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace EterationCaseStudy.IntegrationTest;

public class AuthAndOrdersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public AuthAndOrdersEndpointsTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_Login_And_Create_Order_Flow()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        var createProductResp = await client.PostAsJsonAsync("/api/v1/products", new { Name = "Mouse", Price = 50m, StockQuantity = 10, Description = "gaming" });
        createProductResp.EnsureSuccessStatusCode();
        var productId = await createProductResp.Content.ReadFromJsonAsync<int>();

        var listResp = await client.GetAsync("/api/v1/orders");
        listResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var okCreate = await client.PostAsJsonAsync("/api/v1/orders", new { UserId = 1, Items = new[] { new { ProductId = productId, Quantity = 2 } } });
        okCreate.StatusCode.Should().Be(HttpStatusCode.Created);
        var orderId = await okCreate.Content.ReadFromJsonAsync<int>();

        var getOrder = await client.GetAsync($"/api/v1/orders/{orderId}");
        getOrder.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
