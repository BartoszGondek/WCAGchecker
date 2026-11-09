using AccessAudit.ProjectManagement.Domain.Exceptions;

namespace AccessAudit.ProjectManagement.Domain.Aggregates;

// Aggregate Root corresponding to the yellow "Project" sticky note.
public sealed class Project
{
    public Guid Id { get; }
    public string WebsiteUrl { get; }
    public string? WcagVersion { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public Project(string? websiteUrl)
    {
        if (string.IsNullOrWhiteSpace(websiteUrl)
            || !Uri.TryCreate(websiteUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrWhiteSpace(uri.Host))
            throw new InvalidWebsiteUrlException(websiteUrl);

        Id = Guid.NewGuid();
        WebsiteUrl = websiteUrl;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void SelectAuditParameters(string? wcagVersion)
    {
        if (wcagVersion is not ("2.1" or "2.2"))
            throw new UnsupportedWcagVersionException(wcagVersion);

        WcagVersion = wcagVersion;
    }

    // Repositories return snapshots so a rejected/unsaved operation cannot mutate storage.
    public Project Snapshot() => (Project)MemberwiseClone();
}
