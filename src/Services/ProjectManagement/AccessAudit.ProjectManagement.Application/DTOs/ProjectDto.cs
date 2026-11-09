namespace AccessAudit.ProjectManagement.Application.DTOs;

public sealed record ProjectDto(Guid Id, string WebsiteUrl, string? WcagVersion, DateTimeOffset CreatedAt);
