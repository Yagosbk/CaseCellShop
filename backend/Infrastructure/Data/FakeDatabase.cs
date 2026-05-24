using Backend.Domain.Models;

namespace Backend.Infrastructure.Data;

public static class FakeDatabase
{
    public static List<Product> Products = new()
    {
        new Product { Id = 1,  Name = "Capinha iPhone 15",          Stock = 5,  Price = 49.90m  },
        new Product { Id = 2,  Name = "Capinha Galaxy S24",          Stock = 3,  Price = 39.90m  },
        new Product { Id = 3,  Name = "Capinha iPhone 14",           Stock = 8,  Price = 44.90m  },
        new Product { Id = 4,  Name = "Capinha Galaxy S23",          Stock = 6,  Price = 34.90m  },
        new Product { Id = 5,  Name = "Capinha Xiaomi 13",           Stock = 10, Price = 29.90m  },
        new Product { Id = 6,  Name = "Capinha Motorola Edge 40",    Stock = 7,  Price = 32.90m  },
        new Product { Id = 7,  Name = "Capinha iPhone 15 Pro",       Stock = 4,  Price = 59.90m  },
        new Product { Id = 8,  Name = "Capinha Galaxy S24 Ultra",    Stock = 2,  Price = 64.90m  },
        new Product { Id = 9,  Name = "Capinha Redmi Note 12",       Stock = 12, Price = 24.90m  },
        new Product { Id = 10, Name = "Capinha Poco X5 Pro",         Stock = 9,  Price = 27.90m  },
        new Product { Id = 11, Name = "Capinha OnePlus 12",           Stock = 6,  Price = 36.90m  },
        new Product { Id = 12, Name = "Capinha Samsung Galaxy A55",   Stock = 11, Price = 22.90m  },
    };
}
