namespace AccessAudit.ProjectManagement.Domain.Exceptions;

public sealed class InvalidProjectIdException(Guid attemptedProjectId)
    : DomainException("audit.invalid_project_id", "A screen capture must refer to a non-empty project identifier.")
{
    public Guid AttemptedProjectId { get; } = attemptedProjectId;
}
