using Backend.Application.DTOs.Checkout;
using Backend.Application.Services;
using Backend.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Tests.Unit.Services;

public class CheckoutServiceTests
{
    private readonly Mock<IProductService> _productServiceMock = new();
    private readonly Mock<ILogger<CheckoutService>> _loggerMock = new();

    private CheckoutService CreateService() =>
        new(_productServiceMock.Object, _loggerMock.Object);

    [Fact]
    public void ProcessCartCheckout_MultipleItems_AllValid_ReturnsSuccess()
    {
        var items = new List<CheckoutItemDto>
        {
            new() { ProductId = 1, Quantity = 2 },
            new() { ProductId = 2, Quantity = 1 }
        };
        _productServiceMock.Setup(s => s.HasStock(1, 2)).Returns(true);
        _productServiceMock.Setup(s => s.HasStock(2, 1)).Returns(true);
        _productServiceMock.Setup(s => s.ReduceStock(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

        var result = CreateService().ProcessCheckout(items);

        Assert.Equal(CheckoutResult.Success, result);
        _productServiceMock.Verify(s => s.ReduceStock(1, 2), Times.Once);
        _productServiceMock.Verify(s => s.ReduceStock(2, 1), Times.Once);
    }

    [Fact]
    public void ProcessCartCheckout_SecondItemFails_DoesNotReduceFirstItem()
    {
        var items = new List<CheckoutItemDto>
        {
            new() { ProductId = 1, Quantity = 1 },
            new() { ProductId = 2, Quantity = 99 }
        };
        _productServiceMock.Setup(s => s.HasStock(1, 1)).Returns(true);
        _productServiceMock.Setup(s => s.HasStock(2, 99)).Returns(false);
        _productServiceMock.Setup(s => s.HasStock(2, 0)).Returns(true);

        var result = CreateService().ProcessCheckout(items);

        Assert.Equal(CheckoutResult.InsufficientStock, result);
        _productServiceMock.Verify(s => s.ReduceStock(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
}
