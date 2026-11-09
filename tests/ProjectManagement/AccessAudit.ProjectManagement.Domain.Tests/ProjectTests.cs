using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using Xunit;

namespace AccessAudit.ProjectManagement.Domain.Tests;

public class ProjectTests
{
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/path")]
    public void Register_WhenUrlIsValid_CreatesProject(string url)
    {
        var project = new Project(url);
        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(url, project.WebsiteUrl);
        Assert.Null(project.WcagVersion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("example.com")]
    [InlineData("ftp://example.com")]
    [InlineData("https://")]
    public void Register_WhenUrlIsInvalid_ThrowsWithAttemptedUrl(string url)
    {
        var error = Assert.Throws<InvalidWebsiteUrlException>(() => new Project(url));
        Assert.Equal("project.invalid_website_url", error.Code);
        Assert.Equal(url, error.AttemptedUrl);
    }

    [Theory]
    [InlineData("2.1")]
    [InlineData("2.2")]
    public void SelectAuditParameters_WhenSupported_UpdatesVersion(string version)
    {
        var project = new Project("https://example.com");
        project.SelectAuditParameters(version);
        Assert.Equal(version, project.WcagVersion);
    }

    [Fact]
    public void SelectAuditParameters_WhenUnsupported_ThrowsAndPreservesState()
    {
        var project = new Project("https://example.com");
        project.SelectAuditParameters("2.1");
        var error = Assert.Throws<UnsupportedWcagVersionException>(() => project.SelectAuditParameters("3.0"));
        Assert.Equal("3.0", error.AttemptedVersion);
        Assert.Equal("audit.unsupported_wcag_version", error.Code);
        Assert.Equal("2.1", project.WcagVersion);
    }
}
