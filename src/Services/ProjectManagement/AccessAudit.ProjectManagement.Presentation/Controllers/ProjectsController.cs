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

    [HttpPost("{projectId:guid}/captures")]
    public async Task<ActionResult<AuditScreenDto>> RecordScreenCapture(Guid projectId, RecordScreenCaptureRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await service.RecordScreenCaptureAsync(projectId, request.Dom, request.ScreenshotReferences, cancellationToken);
        return CreatedAtAction(nameof(GetScreen), new { projectId, screenId = dto.Id }, dto);
    }

    [HttpGet("{projectId:guid}/captures/{screenId:guid}")]
    public async Task<ActionResult<AuditScreenDto>> GetScreen(Guid projectId, Guid screenId, CancellationToken cancellationToken)
    {
        var dto = await service.GetScreenAsync(projectId, screenId, cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{projectId:guid}/dashboard")]
    public async Task<ActionResult<InteractiveIssueDashboardDto>> GetDashboard(Guid projectId, CancellationToken cancellationToken)
        => Ok(await service.GetDashboardAsync(projectId, cancellationToken));
}
