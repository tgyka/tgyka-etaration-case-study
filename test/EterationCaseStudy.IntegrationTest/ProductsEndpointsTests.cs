using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace EterationCaseStudy.IntegrationTest;

public class ProductsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public ProductsEndpointsTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_List_Should_Return_200_And_Header()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        var resp = await client.GetAsync("/api/v1/products");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.Contains("X-Total-Count").Should().BeTrue();
    }

    [Fact]
    public async Task Admin_Can_CRUD_Product()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        var createResp = await client.PostAsJsonAsync("/api/v1/products", new { Name = "Phone", Price = 999.99m, StockQuantity = 5, Description = "d" });
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = await createResp.Content.ReadFromJsonAsync<int>();
        id.Should().BeGreaterThan(0);

        var getResp = await client.GetAsync($"/api/v1/products/{id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateResp = await client.PutAsJsonAsync($"/api/v1/products/{id}", new { Id = id, Name = "Phone-2", Price = 500m, StockQuantity = 3, Description = "d2" });
        updateResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteResp = await client.DeleteAsync($"/api/v1/products/{id}");
        deleteResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
