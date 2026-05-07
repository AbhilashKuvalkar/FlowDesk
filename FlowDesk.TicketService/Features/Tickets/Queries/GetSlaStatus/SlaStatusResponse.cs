using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetSlaStatus;

public record SlaStatusResponse(
    Guid TicketId, 
    TicketStatus TicketStatus, 
    int ResponseTimeMinutes, 
    int ResolutionTimeMinutes, 
    bool IsBreaching);
