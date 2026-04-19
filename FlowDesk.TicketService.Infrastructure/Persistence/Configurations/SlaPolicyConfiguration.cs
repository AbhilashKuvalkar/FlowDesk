using FlowDesk.TicketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.TicketService.Infrastructure.Persistence.Configurations;

public class SlaPolicyConfiguration : IEntityTypeConfiguration<SlaPolicy>
{
    public void Configure(EntityTypeBuilder<SlaPolicy> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.ResponseTimeMinutes)
            .IsRequired();

        builder.Property(s => s.ResolutionTimeMinutes)
            .IsRequired();

        builder.HasIndex(t => new { t.TenantId, t.Priority })
            .IsUnique();
    }
}
