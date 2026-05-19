using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.Infrastructure.Caching;

public static class CacheKeys
{
    public static string SlaPolicy(Guid tenantId, TicketPriority priority) =>
        $"sla:policy:{tenantId}:{priority}";

    public static string AgentAvailability(Guid agentId) =>
        $"agent:availability:{agentId}";
}
