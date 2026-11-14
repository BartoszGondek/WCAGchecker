namespace AccessAudit.ProjectManagement.Presentation.Contracts;

public sealed record RegisterWebsiteRequest(string? WebsiteUrl);
public sealed record SelectAuditParametersRequest(string? WcagVersion);
public sealed record ExtractDomAndCaptureScreenshotsRequest(string? Dom, string[]? ScreenshotReferences);