using AccessAudit.ProjectManagement.Application.DTOs;

namespace AccessAudit.ProjectManagement.Application.Services;

public interface IProjectManagementService
{
    Task<ProjectDto> RegisterWebsiteAsync(string? websiteUrl, CancellationToken cancellationToken = default);
    Task<ProjectDto?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<ProjectDto> SelectAuditParametersAsync(Guid projectId, string? wcagVersion, CancellationToken cancellationToken = default);
    Task<AuditScreenDto> RecordScreenCaptureAsync(Guid projectId, string? dom,
        IReadOnlyCollection<string>? screenshotReferences, CancellationToken cancellationToken = default);
    Task<AuditScreenDto?> GetScreenAsync(Guid projectId, Guid screenId, CancellationToken cancellationToken = default);
    Task<InteractiveIssueDashboardDto> GetDashboardAsync(Guid projectId, CancellationToken cancellationToken = default);
}
