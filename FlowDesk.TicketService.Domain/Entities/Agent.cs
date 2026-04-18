using FlowDesk.TicketService.Domain.Common;

namespace FlowDesk.TicketService.Domain.Entities;

public class Agent : BaseEntity
{
    public string Name { get; private set; } = default!;

    public string Email { get; private set; } = default!;

    public bool IsAvailable { get; private set; }

    private Agent() { }

    public static Agent Create(string name, string email, Guid tenantId)
    {
        return new Agent
        {
            Name = name,
            Email = email,
            IsAvailable = true,
            TenantId = tenantId
        };
    }
}
