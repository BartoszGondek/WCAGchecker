namespace AccessAudit.ProjectManagement.Domain.Events;

public sealed record AddedAProject(Guid ProjectId, string WebsiteUrl) : IDomainEvent;

public sealed record SelectedAuditPerimeter(Guid ProjectId, string WcagVersion) : IDomainEvent;

public sealed record ScrapedProjectDataAndTookScreenshots(Guid ProjectId, Guid ScreenId) : IDomainEvent;