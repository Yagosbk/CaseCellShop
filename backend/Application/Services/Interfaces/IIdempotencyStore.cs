using Backend.Application.Services.Interfaces;

namespace Backend.Application.Services.Interfaces;

public interface IIdempotencyStore
{
    bool TryGet(string key, out CheckoutResult result);
    void Set(string key, CheckoutResult result);
}
