using System.Collections.Concurrent;
using Backend.Application.Services.Interfaces;

namespace Backend.Infrastructure.Services;

public class IdempotencyStore : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, CheckoutResult> _cache = new();

    public bool TryGet(string key, out CheckoutResult result) =>
        _cache.TryGetValue(key, out result);

    public void Set(string key, CheckoutResult result) =>
        _cache[key] = result;
}
