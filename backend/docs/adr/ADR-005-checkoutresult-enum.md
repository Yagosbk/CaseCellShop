# ADR-005: CheckoutResult Enum as Typed Service Return

**Status:** Accepted
**Date:** 2026-05-23

## Context

`ICheckoutService.ProcessCheckout` originally returned `bool`. A `false` result could mean either "product not found" or "insufficient stock", forcing the controller to make an additional call to determine which error to surface. This resulted in duplicate repository queries and imprecise HTTP status codes.

## Decision

Replace the `bool` return with a `CheckoutResult` enum:

```csharp
public enum CheckoutResult
{
    Success,
    ProductNotFound,
    InsufficientStock
}
```

The controller maps each value to its correct HTTP status code using a switch expression:

```csharp
return result switch
{
    CheckoutResult.ProductNotFound   => NotFound(new { message = "Product not found." }),
    CheckoutResult.InsufficientStock => UnprocessableEntity(new { message = "Insufficient stock." }),
    _                                => Ok(new { message = "Checkout completed successfully." })
};
```

## Consequences

**Positive:**
- HTTP status codes are now precise: `404` for missing product, `422` for business rule failure (insufficient stock).
- The service communicates intent without throwing exceptions for expected outcomes.
- The switch expression is exhaustive — the compiler warns if a new enum value is added without a corresponding case.

**Negative:**
- Adding a new failure mode requires updating the enum, the service, the controller, and any tests.
- Mix of patterns: checkout uses `CheckoutResult` while other errors use `ApiException`. Acceptable for current scale; should be unified if the domain grows.
