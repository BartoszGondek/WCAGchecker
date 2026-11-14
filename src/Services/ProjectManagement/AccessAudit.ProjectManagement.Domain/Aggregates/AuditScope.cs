using AccessAudit.ProjectManagement.Domain.Exceptions;
using AccessAudit.ProjectManagement.Domain.Events;

namespace AccessAudit.ProjectManagement.Domain.Aggregates;

public sealed class AuditScope
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; }
    public Guid ProjectId { get; }
    public string Dom { get; }
    public IReadOnlyCollection<string> ScreenshotReferences { get; }
    public DateTimeOffset CapturedAt { get; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public AuditScope(Guid projectId, string? dom, IEnumerable<string>? screenshotReferences)
    {
        if (projectId == Guid.Empty)
            throw new InvalidProjectIdException(projectId);
        if (string.IsNullOrWhiteSpace(dom))
            throw new EmptyDomException(projectId);

        Id = Guid.NewGuid();
        ProjectId = projectId;
        Dom = dom;
        ScreenshotReferences = Array.AsReadOnly((screenshotReferences ?? []).ToArray());
        CapturedAt = DateTimeOffset.UtcNow;

        
        _domainEvents.Add(new ScrapedProjectDataAndTookScreenshots(ProjectId, Id));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
