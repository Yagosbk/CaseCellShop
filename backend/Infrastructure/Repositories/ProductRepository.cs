using Backend.Infrastructure.Data;
using Backend.Domain.Models;
using Backend.Domain.Repositories.Interfaces;

namespace Backend.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ILogger<ProductRepository> logger)
    {
        _logger = logger;
    }

    public List<Product> GetAll()
    {
        var products = FakeDatabase.Products;
        _logger.LogDebug("Fetched {Count} products from store", products.Count);
        return products;
    }

    public Product? GetById(int id)
    {
        var product = FakeDatabase.Products.FirstOrDefault(p => p.Id == id);

        if (product is null)
            _logger.LogDebug("Product {Id} not found in store", id);
        else
            _logger.LogDebug("Product {Id} found in store", id);

        return product;
    }

    public void Update(Product product)
    {
    }
}
