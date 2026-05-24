using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Tests.Integration.Helpers;

namespace Backend.Tests.Integration.Controllers;

public class CheckoutControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CheckoutControllerTests(TestWebApplicationFactory factory)
    {
        TestWebApplicationFactory.ResetDatabase();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_CheckoutCart_ValidItems_Returns200()
    {
        var payload = new
        {
            items = new[]
            {
                new { productId = 1, quantity = 1 },
                new { productId = 2, quantity = 1 }
            }
        };

        var response = await _client.PostAsJsonAsync("/checkout/cart", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task POST_CheckoutCart_InsufficientStock_Returns422()
    {
        var response = await _client.PostAsJsonAsync("/checkout/cart",
            new { items = new[] { new { productId = 1, quantity = 100 } } });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task POST_CheckoutCart_ProductNotFound_Returns404()
    {
        var response = await _client.PostAsJsonAsync("/checkout/cart",
            new { items = new[] { new { productId = 9999, quantity = 1 } } });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_CheckoutCart_SameIdempotencyKey_DoesNotReduceStockTwice()
    {
        var initialStock = await GetStock(1);
        var payload = new { items = new[] { new { productId = 1, quantity = 1 } } };

        var request1 = new HttpRequestMessage(HttpMethod.Post, "/checkout/cart")
        {
            Content = JsonContent.Create(payload),
            Headers = { { "Idempotency-Key", "unique-key-123" } }
        };
        var request2 = new HttpRequestMessage(HttpMethod.Post, "/checkout/cart")
        {
            Content = JsonContent.Create(payload),
            Headers = { { "Idempotency-Key", "unique-key-123" } }
        };

        await _client.SendAsync(request1);
        await _client.SendAsync(request2);

        Assert.Equal(initialStock - 1, await GetStock(1));
    }

    private async Task<int> GetStock(int productId)
    {
        var response = await _client.GetAsync("/products");
        var products = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync()).RootElement;
        return products
            .EnumerateArray()
            .First(p => p.GetProperty("id").GetInt32() == productId)
            .GetProperty("stock").GetInt32();
    }
}
