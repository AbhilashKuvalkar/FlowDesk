using FlowDesk.TicketService.Domain.Enums;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.UpdateSlaPolicy;

public record UpdateSlaPolicyCommand(
    Guid PolicyId,
    TicketPriority Priority,
    int ResponseTimeMinutes,
    int ResolutionTimeMinutes,
    Guid TenantId) : IRequest;