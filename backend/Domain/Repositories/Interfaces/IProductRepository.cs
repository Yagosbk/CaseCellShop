using Backend.Domain.Models;

namespace Backend.Domain.Repositories.Interfaces;

public interface IProductRepository
{
    List<Product> GetAll();

    Product? GetById(int id);

    void Update(Product product);
}