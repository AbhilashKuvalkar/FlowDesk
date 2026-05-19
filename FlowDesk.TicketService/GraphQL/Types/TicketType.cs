using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.GraphQL.Types;

public class TicketType
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public TicketStatus Status { get; init; }
    public TicketPriority Priority { get; init; }
    public TicketCategory Category { get; init; }
    public Guid? AssignedAgentId { get; init; }
    public DateTime CreatedAt { get; init; }

    // resolved separately — not from the ticket row
    public SlaStatusType? SlaStatus { get; init; }
    public AgentType? AssignedAgent { get; init; }

    [GraphQLIgnore]
    public Guid TenantId { get; init; }
}
