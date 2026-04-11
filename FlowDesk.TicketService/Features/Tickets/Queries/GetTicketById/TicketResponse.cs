using FlowDesk.TicketService.Domain;
using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetTicketById;

public record TicketResponse(
    Guid Id,
    string Title,
    string Description,
    TicketStatus TicketStatus,
    TicketPriority TicketPriority,
    TicketCategory TicketCategory,
    Guid? AssignedAgentId,
    DateTime CreatedAt);
