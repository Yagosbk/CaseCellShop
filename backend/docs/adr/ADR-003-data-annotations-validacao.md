# ADR-003: Data Annotations + [ApiController] for Input Validation

**Status:** Accepted
**Date:** 2026-05-23

## Context

Request DTOs need input validation before reaching business logic. Options considered were: manual `if` checks in controllers, Data Annotations with `[ApiController]`, or a dedicated library like FluentValidation.

## Decision

Use `System.ComponentModel.DataAnnotations` attributes on DTOs (`[Required]`, `[Range]`) combined with the `[ApiController]` attribute on controllers.

`[ApiController]` automatically intercepts the request pipeline before the action runs: if `ModelState` is invalid, it returns `400 Bad Request` with a `ValidationProblemDetails` body — no manual checks needed.

Example on `CheckoutRequestDto`:
```csharp
[Required]
[Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
public int ProductId { get; set; }

[Required]
[Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
public int Quantity { get; set; }
```

## Consequences

**Positive:**
- Zero boilerplate: validation is declared alongside the model, not scattered across controllers.
- The 400 response format (`ValidationProblemDetails`) is RFC 7807-compliant out of the box.
- No additional packages required.

**Negative:**
- Data Annotations are limited for complex cross-field or conditional validation rules.
- If validation rules grow complex, migration to FluentValidation would be needed.
