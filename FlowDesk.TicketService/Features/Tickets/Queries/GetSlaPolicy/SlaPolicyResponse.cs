using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetSlaPolicy;

public record SlaPolicyResponse(
    string Name,
    TicketPriority TicketPriority,
    int ResponseTimeMinutes,
    int ResolutionTimeMinutes,
    Guid TenantId,
    DateTime CreatedAt);
