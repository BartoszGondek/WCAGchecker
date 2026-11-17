using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccessAudit.ProjectManagement.Infrastructure.Persistence.Seeders;

internal sealed class ProjectManagementDataSeeder(ProjectManagementDbContext db, ILogger<ProjectManagementDataSeeder> logger)
    : IProjectManagementDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (db.Database.IsRelational()) await db.Database.MigrateAsync(cancellationToken);
        else await db.Database.EnsureCreatedAsync(cancellationToken);

        if (await db.Projects.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Database already contains projects. Skipping ProjectManagementDataSeeder.");
            return;
        }

        var ready = new Project("https://ready.example.com");
        ready.SelectAuditParameters("2.2");
        ready.AddAuditScope("<html lang=\"pl\"><body><h1>Demo</h1></body></html>", ["demo/home.png"]);
        ready.ExtractDom();

        var edge = new Project("https://registered.example.com");
        edge.SelectAuditParameters("2.1");

        await db.Projects.AddRangeAsync([ready, edge], cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded 2 projects. ReadyProjectId: {ReadyProjectId}; EdgeProjectId: {EdgeProjectId}", ready.Id, edge.Id);
    }
}