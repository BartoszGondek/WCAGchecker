using System.Collections.Concurrent;
using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Aggregates;

namespace AccessAudit.ProjectManagement.Presentation.Infrastructure;

public sealed class InMemoryAuditScopeRepository : IAuditScopeRepository
{
    private readonly ConcurrentDictionary<Guid, AuditScope> _scopes = new();

    public Task<AuditScope?> GetByIdAsync(Guid scopeId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_scopes.GetValueOrDefault(scopeId));
    }

    public Task<IReadOnlyCollection<AuditScope>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<AuditScope> result = _scopes.Values.Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.CapturedAt).ToArray();
        return Task.FromResult(result);
    }

    public Task SaveAsync(AuditScope scope, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _scopes[scope.Id] = scope;
        return Task.CompletedTask;
    }
}