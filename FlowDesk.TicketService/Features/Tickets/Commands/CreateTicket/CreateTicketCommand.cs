using System;
using FlowDesk.TicketService.Domain;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority TicketPriority,
    TicketCategory TicketCategory,
    Guid TenantId) : IRequest<Guid>;
