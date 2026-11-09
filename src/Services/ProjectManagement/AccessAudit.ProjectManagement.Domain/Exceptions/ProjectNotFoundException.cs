namespace AccessAudit.ProjectManagement.Domain.Exceptions;

public sealed class ProjectNotFoundException(Guid projectId)
    : DomainException("project.not_found", $"Project '{projectId}' was not found.")
{
    public Guid ProjectId { get; } = projectId;
}
