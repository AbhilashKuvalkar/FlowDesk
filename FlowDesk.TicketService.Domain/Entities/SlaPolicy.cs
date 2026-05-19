using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.Domain.Entities;

public class SlaPolicy : BaseEntity
{
    public string Name { get; private set; } = default!;

    public TicketPriority Priority { get; private set; }

    public int ResponseTimeMinutes { get; private set; }

    public int ResolutionTimeMinutes { get; private set; }


    public SlaPolicy() { }


    public static SlaPolicy Create(
        string name,
        TicketPriority priority,
        int responseTimeMinutes,
        int resolutionTimeMinutes,
        Guid tenantId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(responseTimeMinutes);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(resolutionTimeMinutes);

        return new SlaPolicy
        {
            Name = name,
            Priority = priority,
            ResponseTimeMinutes = responseTimeMinutes,
            ResolutionTimeMinutes = resolutionTimeMinutes,
            TenantId = tenantId
        };
    }

    public void Update(int responseTimeMinutes, int resolutionTimeMinutes)
    {
        ResponseTimeMinutes = responseTimeMinutes;
        ResolutionTimeMinutes = resolutionTimeMinutes;
    }
}
