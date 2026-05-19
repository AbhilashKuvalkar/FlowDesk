using FlowDesk.TicketService.Domain.Enums;

namespace FlowDesk.TicketService.Domain.Services;

public interface ISlaServiceClient
{
    Task<SlaPolicyDto?> GetSlaPolicyAsync(
        TicketPriority priority,
        Guid tenantId,
        CancellationToken cancellationToken
    );
}
