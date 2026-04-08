using FlowDesk.TicketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Agent> Agents => Set<Agent>();

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
