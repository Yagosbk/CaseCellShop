using Backend.Application.Services;
using Backend.Domain.Models;
using Backend.Domain.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Tests.Unit.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly Mock<ILogger<ProductService>> _loggerMock = new();

    private ProductService CreateService() =>
        new(_repositoryMock.Object, _loggerMock.Object);

    [Fact]
    public void GetProducts_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Case A", Stock = 5, Price = 49.90m },
            new() { Id = 2, Name = "Case B", Stock = 3, Price = 39.90m }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(products);

        var result = CreateService().GetProducts();

        Assert.Equal(2, result.Count);
        Assert.Equal(products, result);
    }

    [Fact]
    public void HasStock_ProductNotFound_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        Assert.False(CreateService().HasStock(99, 1));
    }

    [Fact]
    public void HasStock_StockSufficient_ReturnsTrue()
    {
        _repositoryMock.Setup(r => r.GetById(1))
            .Returns(new Product { Id = 1, Stock = 5 });

        Assert.True(CreateService().HasStock(1, 3));
    }

    [Fact]
    public void ReduceStock_Success_DecreasesStock()
    {
        var product = new Product { Id = 1, Stock = 5 };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        var result = CreateService().ReduceStock(1, 2);

        Assert.True(result);
        Assert.Equal(3, product.Stock);
    }
}
