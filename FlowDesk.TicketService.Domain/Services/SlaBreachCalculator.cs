using FlowDesk.TicketService.Domain.Entities;

namespace FlowDesk.TicketService.Domain.Services;

public static class SlaBreachCalculator
{
    public static bool IsBreaching(Ticket ticket, SlaPolicyDto slaPolicyDto)
    {
        if (ticket.Status == Enums.TicketStatus.Resolved)
            return false;

        var totalMinutes = (DateTime.UtcNow - ticket.CreatedAt).TotalMinutes;
        return totalMinutes > slaPolicyDto.ResponseTimeMinutes;
    }
}
