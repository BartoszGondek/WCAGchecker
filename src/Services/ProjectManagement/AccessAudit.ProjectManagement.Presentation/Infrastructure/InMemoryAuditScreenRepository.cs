using System.Collections.Concurrent;
using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Aggregates;

namespace AccessAudit.ProjectManagement.Presentation.Infrastructure;

public sealed class InMemoryAuditScreenRepository : IAuditScreenRepository
{
    private readonly ConcurrentDictionary<Guid, AuditScreen> _screens = new();

    public Task<AuditScreen?> GetByIdAsync(Guid screenId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_screens.GetValueOrDefault(screenId));
    }

    public Task<IReadOnlyCollection<AuditScreen>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<AuditScreen> result = _screens.Values.Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.CapturedAt).ToArray();
        return Task.FromResult(result);
    }

    public Task SaveAsync(AuditScreen screen, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _screens[screen.Id] = screen;
        return Task.CompletedTask;
    }
}
