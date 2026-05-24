# ADR-004: Global Error Handling Middleware + ApiException

**Status:** Accepted
**Date:** 2026-05-23

## Context

Without centralized error handling, each controller would need its own try/catch blocks, and unhandled exceptions would leak stack traces to the client. A single, consistent error response shape was needed across all endpoints.

## Decision

Implement `ErrorHandlingMiddleware` registered at the top of the pipeline (before all other middleware) that catches all exceptions and maps them to standardized JSON responses.

Two exception types are handled:

1. **`ApiException`** — a typed domain exception carrying an `HttpStatusCode`. Thrown intentionally for expected business or input errors. Logged at `Warning` level.
2. **`Exception`** (generic) — any unhandled runtime error. Always returns `500 Internal Server Error`. Logged at `Error` level.

All error responses include a `traceId` (from `HttpContext.TraceIdentifier`) that ties the client-facing response to the server log entry:

```json
{
  "success": false,
  "error": "Product not found.",
  "traceId": "0HN8ABC:00000001"
}
```

`ApiResponse<T>` in `Infrastructure/Utils` provides the static factory methods `Ok(data)` and `Fail(error, traceId)` for building consistent responses.

## Consequences

**Positive:**
- A single location to change error response format across the entire API.
- `TraceId` in both the log and the response allows correlating client errors with server logs without exposing internal details.
- `ApiException` factory methods (`NotFound`, `UnprocessableEntity`) make error throwing expressive and consistent.

**Negative:**
- If the middleware itself throws (e.g., during JSON serialization of the error response), the exception will propagate to ASP.NET Core's default error page.
- Controllers that handle some errors via result types (`CheckoutResult`) still coexist — the pattern is mixed rather than uniformly exception-based.
