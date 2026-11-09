namespace AccessAudit.ProjectManagement.Domain.Exceptions;

public sealed class UnsupportedWcagVersionException(string? attemptedVersion)
    : DomainException("audit.unsupported_wcag_version", "Supported WCAG versions are 2.1 and 2.2.")
{
    public string? AttemptedVersion { get; } = attemptedVersion;
}
