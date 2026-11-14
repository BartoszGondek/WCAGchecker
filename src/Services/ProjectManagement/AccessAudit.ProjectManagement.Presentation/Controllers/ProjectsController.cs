using AccessAudit.ProjectManagement.Application.DTOs;
using AccessAudit.ProjectManagement.Application.Services;
using AccessAudit.ProjectManagement.Presentation.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AccessAudit.ProjectManagement.Presentation.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(IProjectManagementService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> RegisterWebsite(RegisterWebsiteRequest request, CancellationToken cancellationToken)
    {
        var dto = await service.RegisterWebsiteAsync(request.WebsiteUrl, cancellationToken);
        return CreatedAtAction(nameof(GetProject), new { projectId = dto.Id }, dto);
    }

    [HttpGet("{projectId:guid}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid projectId, CancellationToken cancellationToken)
    {
        var dto = await service.GetProjectAsync(projectId, cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("{projectId:guid}/audit-parameters")]
    public async Task<ActionResult<ProjectDto>> SelectAuditParameters(Guid projectId, SelectAuditParametersRequest request,
        CancellationToken cancellationToken)
        => Ok(await service.SelectAuditParametersAsync(projectId, request.WcagVersion, cancellationToken));

    [HttpPost("{projectId:guid}/scopes")]
    public async Task<ActionResult<AuditScopeDto>> ExtractDomAndCaptureScreenshots(Guid projectId, ExtractDomAndCaptureScreenshotsRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await service.ExtractDomAndCaptureScreenshotsAsync(projectId, request.Dom, request.ScreenshotReferences, cancellationToken);
        return CreatedAtAction(nameof(GetScope), new { projectId, scopeId = dto.Id }, dto);
    }

    [HttpGet("{projectId:guid}/scopes/{scopeId:guid}")]
    public async Task<ActionResult<AuditScopeDto>> GetScope(Guid projectId, Guid scopeId, CancellationToken cancellationToken)
    {
        var dto = await service.GetScopeAsync(projectId, scopeId, cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{projectId:guid}/dashboard")]
    public async Task<ActionResult<InteractiveIssueDashboardDto>> GetDashboard(Guid projectId, CancellationToken cancellationToken)
        => Ok(await service.GetDashboardAsync(projectId, cancellationToken));
}