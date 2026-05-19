using FlowDesk.TicketService.Domain.Enums;
using FlowDesk.TicketService.Domain.Repositories;
using FlowDesk.TicketService.Domain.Services;
using FlowDesk.TicketService.GraphQL.DataLoaders;
using FlowDesk.TicketService.GraphQL.Types;
using FlowDesk.TicketService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.GraphQL.Queries;

[QueryType]
public class TicketQuery
{
    public async Task<IEnumerable<TicketType>> GetTicketsAsync(
        Guid tenantId,
        TicketStatus? status,
        [Service] AppDbContext context,
        [Service] ISlaServiceClient slaService,
        [Service] IAgentRepository agentRepository,
        CancellationToken cancellationToken
    )
    {
        var query = context.Tickets
            .AsNoTracking()
            .Where(w => w.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(w => w.Status == status.Value);

        var tickets = await query.ToListAsync(cancellationToken);

        return tickets.Select(s => new TicketType
        {
            AssignedAgentId = s.AssignedAgentId,
            Category = s.Category,
            CreatedAt = s.CreatedAt,
            Description = s.Description,
            Id = s.Id,
            Priority = s.Priority,
            Status = s.Status,
            TenantId = s.TenantId,
            Title = s.Title
        });
    }

    public async Task<TicketType?> GetTicketByIdAsync(
        Guid ticketId,
        Guid tenantId,
        [Service] AppDbContext context,
        CancellationToken cancellationToken)
    {
        var ticket = await context.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.Id == ticketId && t.TenantId == tenantId,
                cancellationToken);

        if (ticket is null)
            return null;

        return new TicketType
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            Priority = ticket.Priority,
            Category = ticket.Category,
            AssignedAgentId = ticket.AssignedAgentId,
            CreatedAt = ticket.CreatedAt,
            TenantId = tenantId
        };
    }

    public async Task<SlaStatusType?> GetSlaStatusAsync(
        [Parent] TicketType ticket,
        [Service] ISlaServiceClient slaClient,
        CancellationToken cancellationToken)
    {
        var policy = await slaClient.GetSlaPolicyAsync(
            ticket.Priority, ticket.TenantId, cancellationToken);

        if (policy is null) return null;

        return new SlaStatusType
        {
            ResponseTimeMinutes = policy.ResponseTimeMinutes,
            ResolutionTimeMinutes = policy.ResolutionTimeMinutes,
            IsBreaching = SlaBreachCalculator.IsBreaching(ticket.Status, ticket.CreatedAt, policy)
        };
    }

    public async Task<AgentType?> GetAssignedAgentAsync(
        [Parent] TicketType ticket,
        [Service] AgentDataLoader agentLoader,
        CancellationToken cancellationToken)
    {
        if (!ticket.AssignedAgentId.HasValue)
            return null;

        var agent = await agentLoader.LoadAsync(
            ticket.AssignedAgentId.Value,
            cancellationToken);

        if (agent is null)
            return null;

        return new AgentType
        {
            Id = agent.Id,
            Name = agent.Name,
            Email = agent.Email,
            IsAvailable = agent.IsAvailable
        };
    }

    public async Task<AgentDashboardType?> GetAgentDashboardQuery(
        Guid agentId,
        Guid tenantId,
        [Service] AppDbContext context,
        [Service] AgentDataLoader agentDataLoader,
        [Service] ISlaServiceClient slaServiceClient,
        CancellationToken cancellationToken)
    {
        var agent = await agentDataLoader.LoadAsync(agentId, cancellationToken);

        if (agent is null || agent.TenantId != tenantId)
            return null;

        var tickets = await context.Tickets
            .AsNoTracking()
            .Where(w => w.AssignedAgentId == agentId && w.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        var ticketTypes = tickets
            .Where(w => w.Status == TicketStatus.Assigned ||
                        w.Status == TicketStatus.InProgress);

        var prorities = ticketTypes
            .Select(s => s.Priority)
            .Distinct();

        var slaPolicyCache = new Dictionary<TicketPriority, SlaPolicyDto?>();

        foreach (var priority in prorities)
            slaPolicyCache[priority] = await slaServiceClient.GetSlaPolicyAsync(priority, tenantId, cancellationToken);

        var agentTickets = ticketTypes
            .Select(s =>
            {
                slaPolicyCache.TryGetValue(s.Priority, out var slaPolicy);

                var isBreaching = slaPolicy is not null && 
                    SlaBreachCalculator.IsBreaching(s.Status, s.CreatedAt, slaPolicy);

                return new TicketType
                {
                    AssignedAgentId = agent.Id,
                    Category = s.Category,
                    CreatedAt = s.CreatedAt,
                    Description = s.Description,
                    Id = s.Id,
                    Priority = s.Priority,
                    Status = s.Status,
                    TenantId = s.TenantId,
                    Title = s.Title,
                    SlaStatus = slaPolicy is null ? null : new SlaStatusType
                    {
                        IsBreaching = isBreaching,
                        ResolutionTimeMinutes = slaPolicy.ResolutionTimeMinutes,
                        ResponseTimeMinutes = slaPolicy.ResponseTimeMinutes
                    }
                };
            });

        return new AgentDashboardType
        {
            Email = agent.Email,
            Id = agent.Id,
            Name = agent.Name,
            TicketsResolved = tickets.Count(w => w.Status == TicketStatus.Resolved),
            TenantId = agent.TenantId,
            Tickets = agentTickets
        };
    }
}
