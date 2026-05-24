using System.Net;
using System.Text.Json;
using Backend.Tests.Integration.Helpers;

namespace Backend.Tests.Integration.Controllers;

public class ProductsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(TestWebApplicationFactory factory)
    {
        TestWebApplicationFactory.ResetDatabase();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Products_Returns200_WithValidProductArray()
    {
        var response = await _client.GetAsync("/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync()).RootElement;

        Assert.Equal(JsonValueKind.Array, products.ValueKind);
        Assert.True(products.GetArrayLength() > 0);

        var first = products[0];
        Assert.True(first.TryGetProperty("id", out _));
        Assert.True(first.TryGetProperty("name", out _));
        Assert.True(first.TryGetProperty("price", out _));
        Assert.True(first.TryGetProperty("stock", out _));
    }
}
