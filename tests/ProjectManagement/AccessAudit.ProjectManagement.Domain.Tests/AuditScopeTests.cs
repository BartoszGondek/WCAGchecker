using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Events;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using Xunit;

namespace AccessAudit.ProjectManagement.Domain.Tests;

public class AuditScopeTests
{
    [Fact]
    public void RecordCapture_WhenValid_PreservesDataAndEmitsEvent()
    {
        var projectId = Guid.NewGuid();
        var screenshots = new List<string> { "screens/home.png" };
        var scope = new AuditScope(projectId, "<html lang=\"pl\"></html>", screenshots);

        screenshots.Add("screens/other.png");

        Assert.NotEqual(Guid.Empty, scope.Id);
        Assert.Equal(projectId, scope.ProjectId);
        Assert.Contains("html", scope.Dom);

        // Weryfikacja dodanego Zdarzenia Domenowego (Event Storming)
        var domainEvent = Assert.Single(scope.DomainEvents);
        var scrapedEvent = Assert.IsType<ScrapedProjectDataAndTookScreenshots>(domainEvent);
        Assert.Equal(projectId, scrapedEvent.ProjectId);
        Assert.Equal(scope.Id, scrapedEvent.ScreenId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void RecordCapture_WhenDomIsEmpty_ThrowsWithProjectId(string? dom)
    {
        var projectId = Guid.NewGuid();
        var error = Assert.Throws<EmptyDomException>(() => new AuditScope(projectId, dom, []));
        Assert.Equal(projectId, error.ProjectId);
    }
}   