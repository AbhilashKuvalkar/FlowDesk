namespace FlowDesk.TicketService.Domain.Entities;

public class Ticket : BaseEntity
{
    public string Title { get; private set; } = default!;

    public string Description { get; private set; } = default!;

    public TicketStatus Status { get; private set; }

    public TicketPriority Priority { get; private set; }

    public TicketCategory Category { get; private set; }

    public Guid? AssignedAgentId { get; private set; }

    public Agent? AssignedAgent { get; private set; }

    private readonly List<Comment> _comments = [];

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();


    private Ticket() { }

    public static Ticket Create(string title, string description, TicketPriority priority, TicketCategory category, Guid tenantId)
    {
        return new Ticket
        {
            Title = title,
            Description = description,
            Priority = priority,
            Category = category,
            TenantId = tenantId
        };
    }

    public void AssignTo(Agent agent)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Resolved)
            throw new InvalidOperationException($"Cannot assign a ticket in {Status} status.");

        AssignedAgentId = agent.Id;
        AssignedAgent = agent;
        Status = TicketStatus.Assigned;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartProgress()
    {
        if (Status != TicketStatus.Assigned)
            throw new InvalidOperationException("Ticket must be assigned before it can be started.");

        Status = TicketStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve()
    {
        if (Status != TicketStatus.InProgress)
            throw new InvalidOperationException($"Only in-progress tickets can be resolved.");

        Status = TicketStatus.Resolved;
        UpdatedAt = DateTime.UtcNow;
    }

}
