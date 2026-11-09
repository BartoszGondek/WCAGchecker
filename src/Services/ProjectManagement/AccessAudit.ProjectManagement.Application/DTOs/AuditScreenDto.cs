namespace AccessAudit.ProjectManagement.Application.DTOs;

public sealed record AuditScreenDto(Guid Id, Guid ProjectId, int DomLength,
    IReadOnlyCollection<string> ScreenshotReferences, DateTimeOffset CapturedAt);
