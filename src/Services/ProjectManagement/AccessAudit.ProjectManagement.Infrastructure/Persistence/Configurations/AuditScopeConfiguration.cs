using System.Text.Json;
using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Entities;
using AccessAudit.ProjectManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessAudit.ProjectManagement.Infrastructure.Persistence.Configurations;

internal sealed class AuditScopeConfiguration : IEntityTypeConfiguration<AuditScope>
{
    public void Configure(EntityTypeBuilder<AuditScope> builder)
    {
        builder.ToTable("AuditScopes");
        builder.HasKey(s => s.Identity);
        builder.Property(s => s.Identity).HasColumnName("Id")
            .HasConversion(id => id.Value, value => new AuditScopeId(value)).ValueGeneratedNever();
        builder.Property(s => s.ProjectIdentity).HasColumnName("ProjectId")
            .HasConversion(id => id.Value, value => new ProjectId(value)).IsRequired();
        builder.HasIndex(s => s.ProjectIdentity);
        builder.HasOne<Project>().WithMany().HasForeignKey(s => s.ProjectIdentity).OnDelete(DeleteBehavior.Cascade);
        builder.Property(s => s.Dom).IsRequired();
        builder.Property(s => s.CapturedAt).IsRequired();

        // A collection of text references, not separate domain entities. Persist the private list as JSON.
        var references = builder.Property<List<string>>("_screenshotReferences")
            .HasColumnName("ScreenshotReferences").IsRequired()
            .HasConversion(
                values => JsonSerializer.Serialize(values, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null)!);
        references.Metadata.SetValueComparer(new ValueComparer<List<string>>(
            (left, right) => left!.SequenceEqual(right!),
            values => values.Aggregate(0, (hash, value) => HashCode.Combine(hash, value)),
            values => values.ToList()));
        references.UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(s => s.Id);
        builder.Ignore(s => s.ProjectId);
        builder.Ignore(s => s.ScreenshotReferences);
        builder.Ignore(s => s.DomainEvents);
    }
}
