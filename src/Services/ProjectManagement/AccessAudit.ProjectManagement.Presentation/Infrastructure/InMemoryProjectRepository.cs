using System.Collections.Concurrent;
using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Aggregates;

namespace AccessAudit.ProjectManagement.Presentation.Infrastructure;

public sealed class InMemoryProjectRepository : IProjectRepository
{
    private readonly ConcurrentDictionary<Guid, Project> _projects = new();

    public Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_projects.TryGetValue(projectId, out var project) ? project.Snapshot() : null);
    }

    public Task SaveAsync(Project project, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _projects[project.Id] = project.Snapshot();
        return Task.CompletedTask;
    }
}
