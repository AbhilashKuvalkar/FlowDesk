using System;

namespace FlowDesk.TicketService.GraphQL.Types;

public class AgentType
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Email { get; init; } = default!;
    public bool IsAvailable { get; init; }
}
