namespace FlowDesk.TicketService.GraphQL.Types;

public class AgentDashboardType
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public string Email { get; init; } = default!;

    public int TicketsResolved { get; init; }

    public IEnumerable<TicketType> Tickets { get; init; } = default!;

    [GraphQLIgnore]
    public Guid TenantId { get; init; }
}
