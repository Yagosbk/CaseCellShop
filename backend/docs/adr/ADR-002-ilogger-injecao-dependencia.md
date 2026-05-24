# ADR-002: ILogger via Dependency Injection

**Status:** Accepted
**Date:** 2026-05-23

## Context

The application needs structured, observable logs to trace the flow of requests, diagnose failures, and monitor behavior in production. The logging solution should not create tight coupling to a specific library.

## Decision

Use `ILogger<T>` from `Microsoft.Extensions.Logging`, injected via the constructor in every class that needs logging. No third-party logging library is added — the built-in ASP.NET Core provider is sufficient for the current scale.

Log levels follow this convention:

| Level | When used |
|-------|-----------|
| `Debug` | Internal repository queries (disabled in production) |
| `Information` | Request received, operation completed successfully |
| `Warning` | Expected business failures (product not found, insufficient stock) |
| `Error` | Unhandled exceptions caught by middleware |

All log messages use structured templates with named parameters (e.g., `{ProductId}`) to enable filtering in log aggregation tools.

## Consequences

**Positive:**
- No external dependency — `ILogger<T>` is registered automatically by `WebApplication.CreateBuilder`.
- Structured logging enables filtering by property in tools like Seq, Application Insights, or Elasticsearch.
- Easy to replace the sink (console, file, cloud) via `appsettings.json` without changing application code.

**Negative:**
- No log enrichment (correlation IDs across services) without additional middleware or a library like Serilog.
- `Debug` logs are silent in production by default — requires explicit configuration to enable.
