using FlowDesk.TicketService.Domain.Common;

namespace FlowDesk.TicketService.Domain.Entities;

public class Comment : BaseEntity
{
    public string Body { get; private set; } = default!;

    public string Author { get; private set; } = default!;

    public Guid? TicketId { get; private set; }

    public Ticket? Ticket { get; private set; }

    private Comment() { }

    public static Comment Create(string body, string author, Guid ticketId, Guid tenantId)
    {
        return new Comment
        {
            Body = body,
            Author = author,
            TicketId = ticketId,
            TenantId = tenantId
        };
    }
}
