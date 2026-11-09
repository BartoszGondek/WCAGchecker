namespace AccessAudit.ProjectManagement.Domain.Exceptions;

public sealed class EmptyDomException(Guid projectId)
    : DomainException("audit.empty_dom", "A screen capture must contain a non-empty DOM.")
{
    public Guid ProjectId { get; } = projectId;
}
