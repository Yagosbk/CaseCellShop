using System.Linq;
using Backend.Application.DTOs.Checkout;
using Backend.Application.Services.Interfaces;

namespace Backend.Application.Services;

public class CheckoutService : ICheckoutService
{
    private readonly IProductService _productService;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(IProductService productService, ILogger<CheckoutService> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public CheckoutResult ProcessCheckout(IEnumerable<CheckoutItemDto> items)
    {
        var itemList = items?.ToList() ?? new List<CheckoutItemDto>();
        _logger.LogInformation("Processing cart checkout for {ItemCount} item(s)", itemList.Count);

        foreach (var item in itemList)
        {
            if (!_productService.HasStock(item.ProductId, item.Quantity))
            {
                var productExists = _productService.HasStock(item.ProductId, 0);
                if (!productExists)
                {
                    _logger.LogWarning("Cart checkout failed: product {ProductId} not found", item.ProductId);
                    return CheckoutResult.ProductNotFound;
                }

                _logger.LogWarning("Cart checkout failed: insufficient stock for productId={ProductId}", item.ProductId);
                return CheckoutResult.InsufficientStock;
            }
        }

        foreach (var item in itemList)
        {
            _productService.ReduceStock(item.ProductId, item.Quantity);
        }

        _logger.LogInformation("Cart checkout successful for {ItemCount} item(s)", itemList.Count);
        return CheckoutResult.Success;
    }
}
