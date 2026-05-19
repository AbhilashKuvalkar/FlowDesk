namespace FlowDesk.TicketService.GraphQL.Types;

public class SlaStatusType
{
    public int ResponseTimeMinutes { get; init; }
    public int ResolutionTimeMinutes { get; init; }
    public bool IsBreaching { get; init; }
}
