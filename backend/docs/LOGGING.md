# Logging Strategy — CaseCellShop Backend

## Overview

All logging is done via `ILogger<T>` injected through the constructor in every class that benefits from observability. No third-party libraries are required — `Microsoft.Extensions.Logging` is included with ASP.NET Core.

---

## Log Levels

| Level | When to use |
|-------|-------------|
| `Debug` | Internal repository queries — low-level detail, disabled in production by default |
| `Information` | Normal flow milestones: requests received, operations completed successfully |
| `Warning` | Expected business failures: product not found, insufficient stock |
| `Error` | Unhandled exceptions caught by `ErrorHandlingMiddleware` |

---

## What Each Class Logs

### Controllers

| Class | Event | Level |
|-------|-------|-------|
| `ProductsController` | GET /products received | Information |
| `ProductsController` | Returned N products | Information |
| `CheckoutController` | Checkout requested (productId, quantity) | Information |
| `CheckoutController` | Insufficient stock for productId | Warning |
| `CheckoutController` | Checkout completed for productId | Information |

### Services

| Class | Event | Level |
|-------|-------|-------|
| `ProductService` | Fetching all products | Information |
| `ProductService` | Product not found | Warning |
| `ProductService` | Insufficient stock (requested vs available) | Warning |
| `ProductService` | Stock reduced (productId, new stock) | Information |
| `CheckoutService` | Processing checkout (productId, quantity) | Information |
| `CheckoutService` | Checkout failed for productId | Warning |

### Infrastructure

| Class | Event | Level |
|-------|-------|-------|
| `ProductRepository` | Fetched N products from store | Debug |
| `ProductRepository` | Product found/not found by id | Debug |
| `ErrorHandlingMiddleware` | Unhandled exception (method, path, exception) | Error |

---

## Configuring Log Levels

Log levels are controlled in `appsettings.json` (and `appsettings.Development.json`):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Backend.Infrastructure.Repositories": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

- In **development**, set `Backend.Infrastructure.Repositories` to `Debug` to see repository-level queries.
- In **production**, keep `Default` at `Information` or higher to reduce noise.

---

## Structured Logging

All log calls use structured message templates with named parameters:

```csharp
_logger.LogInformation("Stock reduced for productId={ProductId}: new stock={Stock}", productId, product.Stock);
```

This allows log aggregation tools (Seq, Elasticsearch, Application Insights) to index and filter by property values — not just free-text search.

---

## Dependency Injection Pattern

```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
}
```

`ILogger<T>` is automatically registered by `WebApplication.CreateBuilder(args)` — no manual registration in `Program.cs` is needed.
