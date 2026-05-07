namespace FlowDesk.TicketService.Domain.Services;

public record SlaPolicyDto(
    Guid PolicyId,
    string Name,
    TicketPriority Priority,
    int ResponseTimeMinutes,
    int ResolutionTimeMinutes
);