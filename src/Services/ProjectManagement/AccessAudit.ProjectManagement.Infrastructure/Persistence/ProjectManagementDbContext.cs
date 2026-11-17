using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessAudit.ProjectManagement.Infrastructure.Persistence;

internal sealed class ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options)
    : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<AuditScope> AuditScopes => Set<AuditScope>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("projectmanagement");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectManagementDbContext).Assembly);
    }
}
