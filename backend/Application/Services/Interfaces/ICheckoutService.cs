using Backend.Application.DTOs.Checkout;

namespace Backend.Application.Services.Interfaces;

public enum CheckoutResult
{
    Success,
    ProductNotFound,
    InsufficientStock
}

public interface ICheckoutService
{
    CheckoutResult ProcessCheckout(IEnumerable<CheckoutItemDto> items);
}
