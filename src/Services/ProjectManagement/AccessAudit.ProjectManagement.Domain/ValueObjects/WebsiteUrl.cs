using AccessAudit.ProjectManagement.Domain.Exceptions;

namespace AccessAudit.ProjectManagement.Domain.ValueObjects;

public sealed record WebsiteUrl
{
    public string Value { get; }

    public WebsiteUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || !Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrWhiteSpace(uri.Host))
            throw new InvalidWebsiteUrlException(value);
        Value = value;
    }

    public WebsiteUrl ToHttps()
    {
        var builder = new UriBuilder(Value) { Scheme = Uri.UriSchemeHttps };
        if (builder.Port == 80) builder.Port = -1;
        return new WebsiteUrl(builder.Uri.AbsoluteUri);
    }
}
