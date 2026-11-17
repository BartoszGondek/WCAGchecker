using AccessAudit.ProjectManagement.Domain.Events;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using AccessAudit.ProjectManagement.Domain.States;
using AccessAudit.ProjectManagement.Domain.ValueObjects;
using AccessAudit.ProjectManagement.Domain.Entities;
using WebsiteAddress = AccessAudit.ProjectManagement.Domain.ValueObjects.WebsiteUrl;
using AuditVersion = AccessAudit.ProjectManagement.Domain.ValueObjects.WcagVersion;

namespace AccessAudit.ProjectManagement.Domain.Aggregates;

public sealed class Project
{
    private List<IDomainEvent> _domainEvents = [];

    private readonly List<AuditScope> _scopes = [];

    public ProjectId Identity { get; private set; }
    public WebsiteAddress Url { get; private set; } = null!;
    public AuditVersion? Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public ProjectAuditState State { get; private set; } = ProjectAuditState.Registered;

    public Guid Id => Identity.Value;
    public string WebsiteUrl => Url.Value;
    public string? WcagVersion => Version?.Version;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public IReadOnlyCollection<AuditScope> Scopes => _scopes.AsReadOnly();

    private Project() { }

    public Project(string? websiteUrl)
    {
        Url = new WebsiteAddress(websiteUrl);
        Identity = new ProjectId(Guid.NewGuid());
        CreatedAt = DateTimeOffset.UtcNow;
        _domainEvents.Add(new AddedAProject(Id, WebsiteUrl));
    }

    public void SelectAuditParameters(string? wcagVersion)
    {
        Version = new AuditVersion(wcagVersion);
        _domainEvents.Add(new SelectedAuditPerimeter(Id, Version.Version));
    }

    public void AddAuditScope(string? dom, IEnumerable<string>? screenshotReferences)
    {
        var newScope = new AuditScope(Id, dom, screenshotReferences);
        _scopes.Add(newScope);
    }

    public void ExtractDom()
    {
        if (State is not (ProjectAuditState.Registered or ProjectAuditState.DomExtracted))
            throw new InvalidProjectAuditTransitionException(Id, State, ProjectAuditState.DomExtracted);
        State = ProjectAuditState.DomExtracted;
    }

    public void CompleteAudit() => TransitionFromExtracted(ProjectAuditState.AuditCompleted);
    public void FailAudit() => TransitionFromExtracted(ProjectAuditState.AuditFailed);

    private void TransitionFromExtracted(ProjectAuditState next)
    {
        if (State != ProjectAuditState.DomExtracted)
            throw new InvalidProjectAuditTransitionException(Id, State, next);
        State = next;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    public Project Snapshot()
    {
        var copy = (Project)MemberwiseClone();
        copy._domainEvents = new List<IDomainEvent>(_domainEvents);
        return copy;
    }
}
