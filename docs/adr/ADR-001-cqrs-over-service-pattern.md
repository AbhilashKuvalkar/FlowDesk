# ADR-001: CQRS with MediatR over traditional service pattern

## Status
Accepted

## Context
FlowDesk needs to handle ticket operations with different
consistency and performance characteristics. Read operations
(dashboards, status queries) are high-frequency and
read-only. Write operations (create, assign, resolve) are
low-frequency and require domain validation.

## Decision
Implement CQRS using MediatR. Commands and queries are
separate classes. Pipeline behaviours handle cross-cutting
concerns (validation, logging) without polluting handlers.

## Consequences
Positive:
- Handlers have single responsibility — easy to test in isolation
- Pipeline behaviours apply cross-cutting concerns consistently
- Read side can be optimised independently (AsNoTracking,
  projection) without affecting write side

Negative:
- More classes per feature than a traditional service pattern
- Overhead not justified for simple CRUD with no domain logic

## Alternatives considered
Traditional service pattern rejected — service classes grow
into God objects over time, mixing read and write concerns.