using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessAudit.ProjectManagement.Infrastructure.Persistence.Configurations;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Identity);
        builder.Property(p => p.Identity).HasColumnName("Id")
            .HasConversion(id => id.Value, value => new ProjectId(value)).ValueGeneratedNever();
        builder.Property(p => p.Url).HasColumnName("WebsiteUrl")
            .HasConversion(url => url.Value, value => new WebsiteUrl(value)).IsRequired();
        builder.Property(p => p.Version).HasColumnName("WcagVersion")
            .HasConversion(version => version!.Version, value => new WcagVersion(value)).HasMaxLength(3);
        builder.Property(p => p.State).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Ignore(p => p.Id);
        builder.Ignore(p => p.WebsiteUrl);
        builder.Ignore(p => p.WcagVersion);
        builder.Ignore(p => p.DomainEvents);
    }
}
