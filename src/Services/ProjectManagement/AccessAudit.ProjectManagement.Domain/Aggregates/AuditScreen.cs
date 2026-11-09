using AccessAudit.ProjectManagement.Domain.Exceptions;

namespace AccessAudit.ProjectManagement.Domain.Aggregates;

// A separate aggregate, linked by ProjectId rather than a mutable Project reference.
public sealed class AuditScreen
{
    public Guid Id { get; }
    public Guid ProjectId { get; }
    public string Dom { get; }
    public IReadOnlyCollection<string> ScreenshotReferences { get; }
    public DateTimeOffset CapturedAt { get; }

    public AuditScreen(Guid projectId, string? dom, IEnumerable<string>? screenshotReferences)
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
    }
}
