using AccessAudit.ProjectManagement.Application.DTOs;
using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Entities;
using AccessAudit.ProjectManagement.Domain.Exceptions;

namespace AccessAudit.ProjectManagement.Application.Services;

public sealed class ProjectManagementService(IProjectRepository projects)
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
        var project = await RequireProjectAsync(projectId, cancellationToken);

        project.AddAuditScope(dom, screenshotReferences);
        project.ExtractDom();

      
        await projects.SaveAsync(project, cancellationToken);

        var newScope = project.Scopes.Last();
        return ToDto(newScope);
    }

    public async Task<AuditScopeDto?> GetScopeAsync(Guid projectId, Guid scopeId,
        CancellationToken cancellationToken = default)
    {
     
        var project = await projects.GetByIdAsync(projectId, cancellationToken);
        if (project is null) return null;

        var scope = project.Scopes.FirstOrDefault(s => s.Id == scopeId);
        return scope is null ? null : ToDto(scope);
    }

    public async Task<InteractiveIssueDashboardDto> GetDashboardAsync(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await RequireProjectAsync(projectId, cancellationToken);

      
        var captures = project.Scopes;

        return new(ToDto(project), Array.AsReadOnly(captures.Select(ToDto).ToArray()));
    }

    public async Task<IReadOnlyCollection<ProjectDto>> GetProjectsAsync(CancellationToken cancellationToken = default)
        => Array.AsReadOnly((await projects.GetAllAsync(cancellationToken)).Select(ToDto).ToArray());

    public async Task DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await projects.DeleteAsync(await RequireProjectAsync(projectId, cancellationToken), cancellationToken);

    public async Task<ProjectDto> CompleteAuditAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await RequireProjectAsync(projectId, cancellationToken);
        project.CompleteAudit();
        await projects.SaveAsync(project, cancellationToken);
        return ToDto(project);
    }

    public async Task<ProjectDto> FailAuditAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await RequireProjectAsync(projectId, cancellationToken);
        project.FailAudit();
        await projects.SaveAsync(project, cancellationToken);
        return ToDto(project);
    }

    private async Task<Project> RequireProjectAsync(Guid projectId, CancellationToken cancellationToken)
        => await projects.GetByIdAsync(projectId, cancellationToken) ?? throw new ProjectNotFoundException(projectId);

    private static ProjectDto ToDto(Project project)
        => new(project.Id, project.WebsiteUrl, project.WcagVersion, project.CreatedAt, project.State.ToString());

    private static AuditScopeDto ToDto(AuditScope scope)
        => new(scope.Id, scope.ProjectId, scope.Dom.Length, scope.ScreenshotReferences, scope.CapturedAt);
}