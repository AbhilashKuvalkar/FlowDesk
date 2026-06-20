# ADR-003: Redis Cache-Aside Pattern for SLA Policy Lookups

## Status

Accepted

## Date

2025-06-20

## Context

SLA policies define the response and resolution time commitments for each
ticket priority level (Low, Medium, High, Critical) per tenant. These policies
are consumed on every ticket operation in FlowDesk:

- `CreateTicketCommandHandler` — checks initial SLA thresholds
- `AssignTicketCommandHandler` — verifies the ticket is within response window
- `GetSlaStatusQuery` — calculates whether the ticket is breaching SLA
- `GetAgentDashboardQuery` (GraphQL) — displays SLA status for every open ticket
- `SLA Service` via gRPC — serves policy data to the Ticket Service

Under realistic load, a tenant with 500 concurrent tickets generates thousands
of SLA policy lookups per minute — all against the same small set of policies
(at most 4 per tenant, one per priority level). These policies are configured
once by administrators and may change a few times per year.

Without caching, every one of these lookups incurs a SQL Server round-trip for
data that is effectively static. At scale this is a measurable, unnecessary
bottleneck.

Additionally, when the `GetAgentDashboardQuery` GraphQL resolver fetches tickets
for an agent dashboard, it calls `GetSlaPolicyAsync` for each distinct priority
level. Without request-scoped deduplication, the same policy could be fetched
via gRPC multiple times within a single GraphQL request.

## Decision

Implement the **cache-aside pattern** in `SlaPolicyRepository` using Redis as
the cache store.

### Cache-aside flow

**On read:**

```csharp
public async Task<SlaPolicy?> GetByPriorityAsync(
    TicketPriority priority, Guid tenantId, CancellationToken ct)
{
    var cacheKey = CacheKeys.SlaPolicy(tenantId, priority);

    // 1. Check cache first
    var cached = await _cache.GetAsync<SlaPolicy>(cacheKey, ct);
    if (cached is not null)
        return cached;

    // 2. Cache miss — fetch from SQL Server
    var policy = await _context.SlaPolicies
        .AsNoTracking()
        .FirstOrDefaultAsync(
            s => s.Priority == priority && s.TenantId == tenantId, ct);

    // 3. Populate cache with TTL
    if (policy is not null)
        await _cache.SetAsync(cacheKey, policy,
            TimeSpan.FromMinutes(60), ct);

    return policy;
}
```

**On write (update):**

```csharp
// In UpdateSlaPolicyCommandHandler — after SaveChangesAsync
await _slaPolicyRepository.InvalidateCacheAsync(
    request.Priority, request.TenantId, cancellationToken);
```

Invalidation is explicit and immediate — the stale entry is removed, not left
to expire. The next read triggers a fresh database fetch and repopulates the
cache.

### Cache key design

Cache keys are centralised in a static `CacheKeys` class:

```csharp
public static class CacheKeys
{
    public static string SlaPolicy(Guid tenantId, TicketPriority priority) =>
        $"sla:policy:{tenantId}:{priority}";
}
```

This prevents magic string scatter and ensures that the key used in
`SetAsync`, `GetAsync`, and `RemoveAsync` is always identical — a typo in any
one of them would cause silent invalidation failure.

### Abstraction layer

`ICacheService` is defined in the Domain project and `RedisCacheService`
implements it in Infrastructure. Handlers never inject `ICacheService` directly
— all cache interaction is owned by the repository. This ensures:

- The cache-aside logic (miss → fetch → populate) lives in one place.
- Handlers are testable without a Redis instance — the repository is mocked.
- Swapping Redis for another cache store (Azure Cache for Redis, Memcached)
  requires changing one implementation class.

### Request-scoped deduplication for GraphQL

For the `GetAgentDashboardQuery` resolver, an in-memory dictionary is used
as a request-scoped cache to avoid duplicate gRPC calls for tickets of the
same priority within a single request:

```csharp
var policyCache = new Dictionary<TicketPriority, SlaPolicyDto?>();

foreach (var ticket in activeTickets)
{
    if (!policyCache.TryGetValue(ticket.Priority, out var policy))
    {
        policy = await _slaClient.GetSlaPolicyAsync(
            ticket.Priority, tenantId, cancellationToken);
        policyCache[ticket.Priority] = policy;
    }
    // use policy
}
```

At most 4 gRPC calls per GraphQL request regardless of ticket count.

### TTL rationale

The 60-minute TTL balances two competing concerns:

| Concern | Direction | Reasoning |
|---|---|---|
| Data freshness | Shorter TTL | Stale policies mean incorrect SLA breach calculations |
| Database load reduction | Longer TTL | Fewer SQL round-trips under load |

SLA policies change infrequently — a 59-minute-old policy is operationally
harmless. Explicit invalidation on update ensures changes are reflected
immediately regardless of TTL expiry.

## Consequences

### Positive

- SQL Server load for SLA policy lookups reduced by approximately 95% under
  normal operation.
- Handler code remains clean — `ICacheService` is never injected into command
  or query handlers.
- Explicit cache invalidation on update ensures consistency without waiting
  for TTL expiry.
- `ICacheService` abstraction backed by `IDistributedCache` allows unit tests
  to use in-memory cache implementation — no Redis infrastructure required
  in tests.
- Centralised `CacheKeys` class prevents key mismatch between set and
  invalidate operations.

### Negative

- Brief window of stale data if `InvalidateCacheAsync` fails after a
  successful SQL update. Mitigated by the 60-minute TTL acting as a backstop.
- Redis adds operational complexity and infrastructure cost. Mitigated by
  running Redis in Docker for local development.
- Cache stampede risk: if many requests miss simultaneously (e.g. after a
  Redis restart), all hit SQL Server concurrently. Mitigated by the small
  number of distinct cache keys (4 per tenant) and low miss probability
  under normal operation.
- Developers must remember to call `InvalidateCacheAsync` in every command
  that updates an SLA policy. Enforced by code review and architecture tests.

## Alternatives Considered

### In-memory IMemoryCache

Rejected for production. In-memory cache is per-instance — in a multi-replica
AKS deployment each pod maintains its own independent cache. When one pod
invalidates its cache after an SLA policy update, the other pods continue
serving stale data until their TTL expires. Redis is a shared cache across
all instances.

Acceptable for single-instance deployments or local development. Available
as a drop-in via the `ICacheService` abstraction if needed.

### No caching — always query SQL Server

Rejected. Acceptable for low traffic but becomes a bottleneck at scale. SLA
policies are the ideal caching candidate: high read frequency, very low write
frequency, small payload (a few integer fields), and the same data is read by
multiple services (Ticket Service and SLA Service independently).

### Cache at the application layer in handlers

Rejected. Injecting `ICacheService` directly into handlers was attempted in
the initial implementation but couples the handler to cache infrastructure.
Handlers should express domain intent, not manage cache state. The repository
is the correct owner of the persistence strategy, which includes caching.

### Write-through cache

Considered. Write-through updates the cache on every write rather than
invalidating it. Rejected because the update path (`UpdateSlaPolicyCommand`)
is rare — the added complexity of keeping the cached object in sync with
the updated domain entity is not justified for an operation that happens
a few times per year.

## References

- Martin Fowler — Cache-Aside Pattern:
  https://martinfowler.com/bliki/CacheAside.html
- Microsoft — Caching guidance:
  https://learn.microsoft.com/en-us/azure/architecture/best-practices/caching
- FlowDesk implementation: `SlaPolicyRepository.cs`,
  `RedisCacheService.cs`, `CacheKeys.cs`
- Lesson 4: Redis caching implementation notes
