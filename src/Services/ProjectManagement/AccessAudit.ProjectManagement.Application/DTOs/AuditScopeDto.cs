namespace AccessAudit.ProjectManagement.Application.DTOs;

public sealed record AuditScopeDto(Guid Id, Guid ProjectId, int DomLength,
    IReadOnlyCollection<string> ScreenshotReferences, DateTimeOffset CapturedAt);               