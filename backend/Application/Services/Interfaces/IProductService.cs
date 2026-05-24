using Backend.Domain.Models;

namespace Backend.Application.Services.Interfaces;

public interface IProductService
{
    List<Product> GetProducts();

    bool HasStock(int productId, int quantity);

    bool ReduceStock(int productId, int quantity);
}