using AccessAudit.ProjectManagement.Domain.Exceptions;
using AccessAudit.ProjectManagement.Domain.Events;

namespace AccessAudit.ProjectManagement.Domain.Aggregates;


public sealed class Project
{
  
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; }
    public string WebsiteUrl { get; }
    public string? WcagVersion { get; private set; }
    public DateTimeOffset CreatedAt { get; }

   
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

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

        
        _domainEvents.Add(new AddedAProject(Id, WebsiteUrl));
    }

    public void SelectAuditParameters(string? wcagVersion)
    {
        if (wcagVersion is not ("2.1" or "2.2"))
            throw new UnsupportedWcagVersionException(wcagVersion);

        WcagVersion = wcagVersion;

        // 4. Emitowanie zdarzenia "SelectedAuditPerimeter"
        _domainEvents.Add(new SelectedAuditPerimeter(Id, wcagVersion));
    }

    
    public void ClearDomainEvents() => _domainEvents.Clear();

    
    public Project Snapshot() => (Project)MemberwiseClone();
}