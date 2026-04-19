namespace FlowDesk.TicketService.Domain.Exceptions;

public class PolicyNotFoundException : Exception
{
    public PolicyNotFoundException(Guid policyId) : base($"Policy {policyId} was not found") { }
}
