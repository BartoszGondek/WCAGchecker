using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.Events;
using AccessAudit.ProjectManagement.Domain.Entities;
using AccessAudit.ProjectManagement.Domain.Exceptions;
using Xunit;

namespace AccessAudit.ProjectManagement.Domain.Tests;

public class ProjectTests
{
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/path")]
    public void Register_WhenUrlIsValid_CreatesProjectAndEmitsEvent(string url)
    {
        var project = new Project(url);
        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(url, project.WebsiteUrl);

        // Weryfikacja wygenerowania Zdarzenia Domenowego
        var domainEvent = Assert.Single(project.DomainEvents);
        var addedEvent = Assert.IsType<AddedAProject>(domainEvent);
        Assert.Equal(project.Id, addedEvent.ProjectId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("example.com")]
    [InlineData("https://")]
    public void Register_WhenUrlIsInvalid_ThrowsWithAttemptedUrl(string url)
    {
        Assert.Throws<InvalidWebsiteUrlException>(() => new Project(url));
    }

    [Theory]
    [InlineData("2.1")]
    [InlineData("2.2")]
    public void SelectAuditParameters_WhenSupported_UpdatesVersionAndEmitsEvent(string version)
    {
        var project = new Project("https://example.com");
        project.ClearDomainEvents();

        project.SelectAuditParameters(version);
        Assert.Equal(version, project.WcagVersion);

        var domainEvent = Assert.Single(project.DomainEvents);
        var selectedEvent = Assert.IsType<SelectedAuditPerimeter>(domainEvent);
        Assert.Equal(version, selectedEvent.WcagVersion);
    }

    [Fact]
    public void SelectAuditParameters_WhenUnsupported_ThrowsAndPreservesState()
    {
        var project = new Project("https://example.com");
        project.SelectAuditParameters("2.1");
        project.ClearDomainEvents();

        Assert.Throws<UnsupportedWcagVersionException>(() => project.SelectAuditParameters("3.0"));
        Assert.Empty(project.DomainEvents); // Błąd nie generuje zdarzenia
    }
}