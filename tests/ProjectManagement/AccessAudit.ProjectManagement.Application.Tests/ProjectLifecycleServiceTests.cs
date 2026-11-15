using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Application.Services;
using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using Moq;

namespace AccessAudit.ProjectManagement.Application.Tests;

public class ProjectLifecycleServiceTests
{
    private readonly Mock<IProjectRepository> _projects = new();
    private readonly Mock<IAuditScopeRepository> _scopes = new();
    private ProjectManagementService Service => new(_projects.Object, _scopes.Object);

    [Fact]
    public async Task GetProjects_ReturnsMaterializedReadModels()
    {
        var project = new Project("https://example.com");
        _projects.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { project });
        var dto = Assert.Single(await Service.GetProjectsAsync());
        Assert.Equal(project.Id, dto.Id);
        Assert.Equal("Registered", dto.State);
    }

    [Fact]
    public async Task DeleteProject_WhenExists_DelegatesToRepository()
    {
        var project = ExistingProject();
        await Service.DeleteProjectAsync(project.Id);
        _projects.Verify(r => r.DeleteAsync(project, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_WhenMissing_DoesNotDelete()
    {
        await Assert.ThrowsAsync<ProjectNotFoundException>(() => Service.DeleteProjectAsync(Guid.NewGuid()));
        _projects.Verify(r => r.DeleteAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CompleteAudit_WhenExtracted_SavesTerminalState()
    {
        var project = ExistingProject();
        project.ExtractDom();
        var dto = await Service.CompleteAuditAsync(project.Id);
        Assert.Equal("AuditCompleted", dto.State);
        _projects.Verify(r => r.SaveAsync(project, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CompleteAudit_WhenRegistered_DoesNotSave()
    {
        var project = ExistingProject();
        await Assert.ThrowsAsync<InvalidProjectAuditTransitionException>(() => Service.CompleteAuditAsync(project.Id));
        _projects.Verify(r => r.SaveAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExtractDom_WhenCompleted_DoesNotSaveScope()
    {
        var project = ExistingProject();
        project.ExtractDom();
        project.CompleteAudit();
        await Assert.ThrowsAsync<InvalidProjectAuditTransitionException>(() => Service.ExtractDomAndCaptureScreenshotsAsync(project.Id, "<html/>", []));
        _scopes.Verify(r => r.SaveAsync(It.IsAny<AuditScope>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private Project ExistingProject()
    {
        var project = new Project("https://example.com");
        _projects.Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>())).ReturnsAsync(project);
        return project;
    }
}
