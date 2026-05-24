using Backend.Application.Services.Interfaces;
using Backend.Infrastructure.Services;

namespace Backend.Tests.Unit.Services;

public class IdempotencyStoreTests
{
    [Fact]
    public void Set_ThenTryGet_ReturnsTrueAndCorrectResult()
    {
        var store = new IdempotencyStore();

        store.Set("key-1", CheckoutResult.Success);
        var found = store.TryGet("key-1", out var result);

        Assert.True(found);
        Assert.Equal(CheckoutResult.Success, result);

        store.Set("key-2", CheckoutResult.ProductNotFound);
        store.TryGet("key-2", out var result2);
        Assert.Equal(CheckoutResult.ProductNotFound, result2);

        var notFound = store.TryGet("key-missing", out _);
        Assert.False(notFound);
    }
}
