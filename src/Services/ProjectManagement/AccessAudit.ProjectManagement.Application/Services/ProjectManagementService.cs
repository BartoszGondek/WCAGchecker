using AccessAudit.ProjectManagement.Application.DTOs;
using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Exceptions;

namespace AccessAudit.ProjectManagement.Application.Services;

public sealed class ProjectManagementService(IProjectRepository projects, IAuditScreenRepository screens)
    : IProjectManagementService
{
    public async Task<ProjectDto> RegisterWebsiteAsync(string? websiteUrl, CancellationToken cancellationToken = default)
    {
        var project = new Project(websiteUrl);
        await projects.SaveAsync(project, cancellationToken);
        return ToDto(project);
    }

    public async Task<ProjectDto?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await projects.GetByIdAsync(projectId, cancellationToken);
        return project is null ? null : ToDto(project);
    }

    public async Task<ProjectDto> SelectAuditParametersAsync(Guid projectId, string? wcagVersion,
        CancellationToken cancellationToken = default)
    {
        var project = await RequireProjectAsync(projectId, cancellationToken);
        project.SelectAuditParameters(wcagVersion);
        await projects.SaveAsync(project, cancellationToken);
        return ToDto(project);
    }

    // Receives outputs of a capture tool. It does not claim to browse or audit websites.
    public async Task<AuditScreenDto> RecordScreenCaptureAsync(Guid projectId, string? dom,
        IReadOnlyCollection<string>? screenshotReferences, CancellationToken cancellationToken = default)
    {
        await RequireProjectAsync(projectId, cancellationToken);
        var screen = new AuditScreen(projectId, dom, screenshotReferences);
        await screens.SaveAsync(screen, cancellationToken);
        return ToDto(screen);
    }

    public async Task<AuditScreenDto?> GetScreenAsync(Guid projectId, Guid screenId,
        CancellationToken cancellationToken = default)
    {
        var screen = await screens.GetByIdAsync(screenId, cancellationToken);
        return screen is null || screen.ProjectId != projectId ? null : ToDto(screen);
    }

    public async Task<InteractiveIssueDashboardDto> GetDashboardAsync(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await RequireProjectAsync(projectId, cancellationToken);
        var captures = await screens.GetByProjectIdAsync(projectId, cancellationToken);
        return new(ToDto(project), Array.AsReadOnly(captures.Select(ToDto).ToArray()));
    }

    private async Task<Project> RequireProjectAsync(Guid projectId, CancellationToken cancellationToken)
        => await projects.GetByIdAsync(projectId, cancellationToken) ?? throw new ProjectNotFoundException(projectId);

    private static ProjectDto ToDto(Project project)
        => new(project.Id, project.WebsiteUrl, project.WcagVersion, project.CreatedAt);

    private static AuditScreenDto ToDto(AuditScreen screen)
        => new(screen.Id, screen.ProjectId, screen.Dom.Length, screen.ScreenshotReferences, screen.CapturedAt);
}
