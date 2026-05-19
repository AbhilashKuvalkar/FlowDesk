using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.Domain.Services;

public static class SlaBreachCalculator
{
    public static bool IsBreaching(TicketStatus ticketStatus, DateTime createdAt, SlaPolicyDto slaPolicyDto)
    {
        if (ticketStatus == TicketStatus.Resolved)
            return false;

        var totalMinutes = (DateTime.UtcNow - createdAt).TotalMinutes;
        return totalMinutes > slaPolicyDto.ResponseTimeMinutes;
    }
}
