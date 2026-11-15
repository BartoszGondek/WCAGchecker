using AccessAudit.ProjectManagement.Domain.States;

namespace AccessAudit.ProjectManagement.Domain.Exceptions;

public sealed class InvalidProjectAuditTransitionException(Guid projectId,
    ProjectAuditState currentState, ProjectAuditState requestedState)
    : DomainException("project.invalid_audit_transition", $"Cannot change audit state from {currentState} to {requestedState}.")
{
    public Guid ProjectId { get; } = projectId;
    public ProjectAuditState CurrentState { get; } = currentState;
    public ProjectAuditState RequestedState { get; } = requestedState;
}
