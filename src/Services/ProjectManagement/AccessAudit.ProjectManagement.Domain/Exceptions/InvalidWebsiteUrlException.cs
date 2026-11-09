namespace AccessAudit.ProjectManagement.Domain.Exceptions;

public sealed class InvalidWebsiteUrlException(string? attemptedUrl)
    : DomainException("project.invalid_website_url", "Website URL must be an absolute HTTP or HTTPS address with a host.")
{
    public string? AttemptedUrl { get; } = attemptedUrl;
}
