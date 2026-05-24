using Backend.Application.Services.Interfaces;
using Backend.Domain.Models;
using Backend.Domain.Repositories.Interfaces;

namespace Backend.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public List<Product> GetProducts()
    {
        _logger.LogInformation("Fetching all products");
        return _repository.GetAll();
    }

    public bool HasStock(int productId, int quantity)
    {
        var product = _repository.GetById(productId);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", productId);
            return false;
        }

        return product.Stock >= quantity;
    }

    public bool ReduceStock(int productId, int quantity)
    {
        var product = _repository.GetById(productId);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", productId);
            return false;
        }

        if (product.Stock < quantity)
        {
            _logger.LogWarning("Insufficient stock for productId={ProductId}: requested={Quantity}, available={Stock}",
                productId, quantity, product.Stock);
            return false;
        }

        product.Stock -= quantity;

        _logger.LogInformation("Stock reduced for productId={ProductId}: new stock={Stock}", productId, product.Stock);
        return true;
    }

}
