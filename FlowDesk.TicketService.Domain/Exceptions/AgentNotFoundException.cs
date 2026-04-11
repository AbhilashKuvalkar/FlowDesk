namespace FlowDesk.TicketService.Domain.Exceptions;

public class AgentNotFoundException : Exception
{
    public AgentNotFoundException(Guid agentId) : base($"Agent {agentId} was not found") { }
}
