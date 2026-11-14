using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Application.Services;
using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using Moq;
using Xunit;

namespace AccessAudit.ProjectManagement.Application.Tests;

public class ProjectManagementServiceTests
{
    private readonly Mock<IProjectRepository> _projects = new();
    private readonly Mock<IAuditScopeRepository> _scopes = new();
    private ProjectManagementService CreateService() => new(_projects.Object, _scopes.Object);

    [Fact]
    public async Task RegisterWebsite_WhenValid_SavesProject()
    {
        var dto = await CreateService().RegisterWebsiteAsync("https://example.com");
        Assert.Equal("https://example.com", dto.WebsiteUrl);
        _projects.Verify(r => r.SaveAsync(It.Is<Project>(p => p.Id == dto.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterWebsite_WhenInvalid_NeverSaves()
    {
        await Assert.ThrowsAsync<InvalidWebsiteUrlException>(() => CreateService().RegisterWebsiteAsync("bad"));
        _projects.Verify(r => r.SaveAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SelectAuditParameters_WhenProjectExists_UpdatesAndSaves()
    {
        var project = ExistingProject();
        var dto = await CreateService().SelectAuditParametersAsync(project.Id, "2.2");
        Assert.Equal("2.2", dto.WcagVersion);
        _projects.Verify(r => r.SaveAsync(project, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SelectAuditParameters_WhenUnsupported_NeverSaves()
    {
        var project = ExistingProject();
        await Assert.ThrowsAsync<UnsupportedWcagVersionException>(() => CreateService().SelectAuditParametersAsync(project.Id, "3.0"));
        _projects.Verify(r => r.SaveAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SelectAuditParameters_WhenMissing_ThrowsWithIdAndNeverSaves()
    {
        var id = Guid.NewGuid();
        await Assert.ThrowsAsync<ProjectNotFoundException>(() => CreateService().SelectAuditParametersAsync(id, "2.2"));
        _projects.Verify(r => r.SaveAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetProject_WhenExists_ReturnsReadModel()
    {
        var project = ExistingProject();
        var dto = await CreateService().GetProjectAsync(project.Id);
        Assert.NotNull(dto);
        Assert.Equal(project.Id, dto.Id);
    }

    [Fact]
    public async Task GetProject_WhenMissing_ReturnsNull()
    {
        Assert.Null(await CreateService().GetProjectAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ExtractDomAndCaptureScreenshots_WhenValid_SavesScope()
    {
        var project = ExistingProject();
        var dto = await CreateService().ExtractDomAndCaptureScreenshotsAsync(project.Id, "<html></html>", ["home.png"]);
        Assert.Equal(project.Id, dto.ProjectId);
        _scopes.Verify(r => r.SaveAsync(It.Is<AuditScope>(s => s.Id == dto.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExtractDomAndCaptureScreenshots_WhenDomEmpty_NeverSaves()
    {
        var project = ExistingProject();
        await Assert.ThrowsAsync<EmptyDomException>(() => CreateService().ExtractDomAndCaptureScreenshotsAsync(project.Id, "", []));
        _scopes.Verify(r => r.SaveAsync(It.IsAny<AuditScope>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExtractDomAndCaptureScreenshots_WhenProjectMissing_NeverSaves()
    {
        await Assert.ThrowsAsync<ProjectNotFoundException>(() => CreateService().ExtractDomAndCaptureScreenshotsAsync(Guid.NewGuid(), "<html></html>", []));
        _scopes.Verify(r => r.SaveAsync(It.IsAny<AuditScope>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private Project ExistingProject()
    {
        var project = new Project("https://example.com");
        _projects.Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>())).ReturnsAsync(project);
        return project;
    }
}