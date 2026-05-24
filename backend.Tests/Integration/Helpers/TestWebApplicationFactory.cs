using Backend.Infrastructure.Data;
using Backend.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.Integration.Helpers;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
    }

    /// <summary>
    /// Resets FakeDatabase to a clean, predictable state before each test.
    /// Call this in test constructors or test setup methods.
    /// </summary>
    public static void ResetDatabase()
    {
        FakeDatabase.Products = new List<Product>
        {
            new() { Id = 1,  Name = "Capinha iPhone 15",          Stock = 5,  Price = 49.90m },
            new() { Id = 2,  Name = "Capinha Galaxy S24",          Stock = 3,  Price = 39.90m },
            new() { Id = 3,  Name = "Capinha iPhone 14",           Stock = 8,  Price = 44.90m },
            new() { Id = 4,  Name = "Capinha Galaxy S23",          Stock = 6,  Price = 34.90m },
            new() { Id = 5,  Name = "Capinha Xiaomi 13",           Stock = 10, Price = 29.90m },
            new() { Id = 6,  Name = "Capinha Motorola Edge 40",    Stock = 7,  Price = 32.90m },
            new() { Id = 7,  Name = "Capinha iPhone 15 Pro",       Stock = 4,  Price = 59.90m },
            new() { Id = 8,  Name = "Capinha Galaxy S24 Ultra",    Stock = 2,  Price = 64.90m },
            new() { Id = 9,  Name = "Capinha Redmi Note 12",       Stock = 12, Price = 24.90m },
            new() { Id = 10, Name = "Capinha Poco X5 Pro",         Stock = 9,  Price = 27.90m },
            new() { Id = 11, Name = "Capinha OnePlus 12",           Stock = 6,  Price = 36.90m },
            new() { Id = 12, Name = "Capinha Samsung Galaxy A55",   Stock = 11, Price = 22.90m },
        };
    }
}
