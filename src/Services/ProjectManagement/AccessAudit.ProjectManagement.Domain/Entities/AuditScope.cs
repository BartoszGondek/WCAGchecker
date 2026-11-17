using AccessAudit.ProjectManagement.Domain.Events;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using AccessAudit.ProjectManagement.Domain.ValueObjects;

namespace AccessAudit.ProjectManagement.Domain.Entities;

public sealed class AuditScope
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private List<string> _screenshotReferences = [];

    public AuditScopeId Identity { get; private set; }
    public ProjectId ProjectIdentity { get; private set; }
    public string Dom { get; private set; } = null!;
    public DateTimeOffset CapturedAt { get; private set; }

    public Guid Id => Identity.Value;
    public Guid ProjectId => ProjectIdentity.Value;
    public IReadOnlyCollection<string> ScreenshotReferences => _screenshotReferences.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private AuditScope() { }

    internal AuditScope(Guid projectId, string? dom, IEnumerable<string>? screenshotReferences)
    {
        if (projectId == Guid.Empty) throw new InvalidProjectIdException(projectId);
        if (string.IsNullOrWhiteSpace(dom)) throw new EmptyDomException(projectId);

        Identity = new AuditScopeId(Guid.NewGuid());
        ProjectIdentity = new ProjectId(projectId);
        Dom = dom;
        _screenshotReferences = (screenshotReferences ?? []).ToList();
        CapturedAt = DateTimeOffset.UtcNow;
        _domainEvents.Add(new ScrapedProjectDataAndTookScreenshots(ProjectId, Id));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}