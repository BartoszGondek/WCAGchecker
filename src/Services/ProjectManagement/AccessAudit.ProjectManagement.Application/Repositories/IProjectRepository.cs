using AccessAudit.ProjectManagement.Domain.Aggregates;

namespace AccessAudit.ProjectManagement.Application.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Project project, CancellationToken cancellationToken = default);
}
