# ADR-001: Layered Architecture (Clean Architecture)

**Status:** Accepted
**Date:** 2026-05-23

## Context

The project needed a structural organization that separates concerns, makes each layer independently testable, and avoids coupling between business rules and infrastructure details (database, HTTP framework, etc.).

## Decision

Adopt a four-layer architecture inspired by Clean Architecture:

```
Domain/          — Entities, interfaces, exceptions, business rules (no external dependencies)
Application/     — Use cases, services, DTOs (depends only on Domain)
Infrastructure/  — Repositories, middleware, data access, external concerns (depends on Application/Domain)
Presentation/    — Controllers, HTTP layer (depends on Application)
```

Dependencies flow inward: Presentation → Application → Domain. Infrastructure implements interfaces defined in Domain.

## Consequences

**Positive:**
- Business logic in `Application/Services` is isolated from HTTP and database concerns.
- Adding a new data source (e.g., a real database) only requires a new `Infrastructure/Repositories` implementation.
- Each layer can be unit tested independently.

**Negative:**
- More files and folders than a flat structure for a project of this size.
- Requires discipline to not break layer boundaries (e.g., importing Infrastructure types in Domain).
