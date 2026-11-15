using AccessAudit.ProjectManagement.Domain.Exceptions;

namespace AccessAudit.ProjectManagement.Domain.ValueObjects;

public sealed record WcagVersion
{
    public string Version { get; }

    public WcagVersion(string? version)
    {
        if (version is not ("2.0" or "2.1" or "2.2"))
            throw new UnsupportedWcagVersionException(version);
        Version = version;
    }

    public WcagVersion UpgradeToLatest() => new("2.2");
}
