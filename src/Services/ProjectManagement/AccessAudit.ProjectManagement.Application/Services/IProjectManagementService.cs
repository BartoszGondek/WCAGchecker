using AccessAudit.ProjectManagement.Application.DTOs;

namespace AccessAudit.ProjectManagement.Application.Services;

public interface IProjectManagementService
{
    Task<ProjectDto> RegisterWebsiteAsync(string? websiteUrl, CancellationToken cancellationToken = default);
    Task<ProjectDto?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<ProjectDto> SelectAuditParametersAsync(Guid projectId, string? wcagVersion, CancellationToken cancellationToken = default);
    Task<AuditScopeDto> ExtractDomAndCaptureScreenshotsAsync(Guid projectId, string? dom,
        IReadOnlyCollection<string>? screenshotReferences, CancellationToken cancellationToken = default);
    Task<AuditScopeDto?> GetScopeAsync(Guid projectId, Guid scopeId, CancellationToken cancellationToken = default);
    Task<InteractiveIssueDashboardDto> GetDashboardAsync(Guid projectId, CancellationToken cancellationToken = default);
}