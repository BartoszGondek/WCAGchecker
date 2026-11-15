using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Infrastructure.Persistence;
using AccessAudit.ProjectManagement.Infrastructure.Persistence.Repositories;
using AccessAudit.ProjectManagement.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccessAudit.ProjectManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProjectManagementDbContext>(options =>
        {
            if (bool.TryParse(configuration["Persistence:UseInMemoryDatabase"], out var useInMemory) && useInMemory)
            {
                options.UseInMemoryDatabase(configuration["Persistence:InMemoryDatabaseName"] ?? "AccessAuditProjectManagementLab4");
            }
            else
            {
                var connection = configuration.GetConnectionString("ProjectManagementDb")
                    ?? throw new InvalidOperationException("Missing connection string 'ProjectManagementDb'.");
                options.UseSqlServer(connection, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "projectmanagement"));
            }
        });
        services.AddScoped<IProjectRepository, SqlProjectRepository>();
        services.AddScoped<IAuditScopeRepository, SqlAuditScopeRepository>();
        services.AddScoped<IProjectManagementDataSeeder, ProjectManagementDataSeeder>();
        return services;
    }
}
