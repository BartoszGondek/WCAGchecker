using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using Xunit;

namespace AccessAudit.ProjectManagement.Domain.Tests;

public class AuditScreenTests
{
    [Fact]
    public void RecordCapture_WhenValid_PreservesDataAndProtectsScreenshotCollection()
    {
        var projectId = Guid.NewGuid();
        var screenshots = new List<string> { "screens/home.png" };
        var screen = new AuditScreen(projectId, "<html lang=\"pl\"></html>", screenshots);
        screenshots.Add("screens/other.png");
        Assert.NotEqual(Guid.Empty, screen.Id);
        Assert.Equal(projectId, screen.ProjectId);
        Assert.Contains("html", screen.Dom);
        Assert.Single(screen.ScreenshotReferences);
        Assert.False(screen.ScreenshotReferences is string[]);
        var collection = Assert.IsAssignableFrom<ICollection<string>>(screen.ScreenshotReferences);
        Assert.Throws<NotSupportedException>(() => collection.Add("changed.png"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void RecordCapture_WhenDomIsEmpty_ThrowsWithProjectId(string? dom)
    {
        var projectId = Guid.NewGuid();
        var error = Assert.Throws<EmptyDomException>(() => new AuditScreen(projectId, dom, []));
        Assert.Equal(projectId, error.ProjectId);
        Assert.Equal("audit.empty_dom", error.Code);
    }
}
