namespace AccessAudit.ProjectManagement.Application.DTOs;

// Initial projection for the board's dashboard: project and collected audit inputs.
// Accessibility findings will be supplied by AuditEngine in a later laboratory.
public sealed record InteractiveIssueDashboardDto(ProjectDto Project, IReadOnlyCollection<AuditScreenDto> Screens);
