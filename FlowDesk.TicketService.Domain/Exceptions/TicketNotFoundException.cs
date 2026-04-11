namespace FlowDesk.TicketService.Domain.Exceptions;

public class TicketNotFoundException : Exception
{
    public TicketNotFoundException(Guid ticketId) : base($"Ticket {ticketId} was not found") { }
}
