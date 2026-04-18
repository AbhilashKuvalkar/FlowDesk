using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions, IPublisher publisher) : base(dbContextOptions)
    {
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Agent> Agents => Set<Agent>();

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Count != 0)
            .ToList();

        var events = entities
            .Select(x => x.DomainEvents)
            .ToList();

        entities.ForEach(x => x.ClearDomainEvents());

        foreach (var domainEvent in events)
            await _publisher.Publish(domainEvent, cancellationToken);
    }
}
