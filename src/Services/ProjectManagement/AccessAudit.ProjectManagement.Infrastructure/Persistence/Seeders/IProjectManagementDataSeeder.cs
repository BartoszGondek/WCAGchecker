namespace AccessAudit.ProjectManagement.Infrastructure.Persistence.Seeders;

public interface IProjectManagementDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
