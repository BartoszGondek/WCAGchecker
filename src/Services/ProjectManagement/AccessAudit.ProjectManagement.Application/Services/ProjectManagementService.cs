using AccessAudit.ProjectManagement.Application.DTOs;
using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Exceptions;

namespace AccessAudit.ProjectManagement.Application.Services;

public sealed class ProjectManagementService(IProjectRepository projects, IAuditScopeRepository scopes)
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

    public async Task<AuditScopeDto> ExtractDomAndCaptureScreenshotsAsync(Guid projectId, string? dom,
        IReadOnlyCollection<string>? screenshotReferences, CancellationToken cancellationToken = default)
    {
        await RequireProjectAsync(projectId, cancellationToken);
        var scope = new AuditScope(projectId, dom, screenshotReferences);
        await scopes.SaveAsync(scope, cancellationToken);
        return ToDto(scope);
    }

    public async Task<AuditScopeDto?> GetScopeAsync(Guid projectId, Guid scopeId,
        CancellationToken cancellationToken = default)
    {
        var scope = await scopes.GetByIdAsync(scopeId, cancellationToken);
        return scope is null || scope.ProjectId != projectId ? null : ToDto(scope);
    }

    public async Task<InteractiveIssueDashboardDto> GetDashboardAsync(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await RequireProjectAsync(projectId, cancellationToken);
        var captures = await scopes.GetByProjectIdAsync(projectId, cancellationToken);
        return new(ToDto(project), Array.AsReadOnly(captures.Select(ToDto).ToArray()));
    }

    private async Task<Project> RequireProjectAsync(Guid projectId, CancellationToken cancellationToken)
        => await projects.GetByIdAsync(projectId, cancellationToken) ?? throw new ProjectNotFoundException(projectId);

    private static ProjectDto ToDto(Project project)
        => new(project.Id, project.WebsiteUrl, project.WcagVersion, project.CreatedAt);

    private static AuditScopeDto ToDto(AuditScope scope)
        => new(scope.Id, scope.ProjectId, scope.Dom.Length, scope.ScreenshotReferences, scope.CapturedAt);
}