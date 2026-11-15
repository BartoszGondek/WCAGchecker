using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using AccessAudit.ProjectManagement.Domain.States;
using AccessAudit.ProjectManagement.Domain.ValueObjects;

namespace AccessAudit.ProjectManagement.Domain.Tests;

public class RichModelTests
{
    [Fact]
    public void WebsiteUrl_ToHttps_ReturnsANewValueWithoutChangingOriginal()
    {
        var original = new WebsiteUrl("http://example.com/a");
        var secure = original.ToHttps();
        Assert.Equal("http://example.com/a", original.Value);
        Assert.Equal("https://example.com/a", secure.Value);
    }

    [Theory]
    [InlineData("2.0")]
    [InlineData("2.1")]
    [InlineData("2.2")]
    public void WcagVersion_Upgrade_PreservesOriginal(string version)
    {
        var original = new WcagVersion(version);
        Assert.Equal("2.2", original.UpgradeToLatest().Version);
        Assert.Equal(version, original.Version);
    }

    [Fact]
    public void CompleteAudit_BeforeExtraction_ThrowsAndPreservesState()
    {
        var project = new Project("https://example.com");
        var error = Assert.Throws<InvalidProjectAuditTransitionException>(project.CompleteAudit);
        Assert.Equal(project.Id, error.ProjectId);
        Assert.Equal(ProjectAuditState.Registered, error.CurrentState);
        Assert.Equal(ProjectAuditState.AuditCompleted, error.RequestedState);
        Assert.Equal(ProjectAuditState.Registered, project.State);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TerminalState_BlocksFurtherExtraction(bool success)
    {
        var project = new Project("https://example.com");
        project.ExtractDom();
        if (success) project.CompleteAudit(); else project.FailAudit();
        Assert.Equal(success ? ProjectAuditState.AuditCompleted : ProjectAuditState.AuditFailed, project.State);
        Assert.Throws<InvalidProjectAuditTransitionException>(project.ExtractDom);
    }

    [Fact]
    public void Scope_ProtectsScreenshotCollectionAndRejectsEmptyProjectId()
    {
        var source = new List<string> { "home.png" };
        var scope = new AuditScope(Guid.NewGuid(), "<html/>", source);
        source.Clear();
        Assert.Single(scope.ScreenshotReferences);
        var view = Assert.IsAssignableFrom<ICollection<string>>(scope.ScreenshotReferences);
        Assert.Throws<NotSupportedException>(() => view.Add("injected.png"));
        Assert.Throws<InvalidProjectIdException>(() => new AuditScope(Guid.Empty, "<html/>", []));
    }

    [Fact]
    public void Snapshot_DomainEventQueue_IsIndependent()
    {
        var project = new Project("https://example.com");
        project.Snapshot().ClearDomainEvents();
        Assert.Single(project.DomainEvents);
    }
}
