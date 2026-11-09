using AccessAudit.ProjectManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AccessAudit.ProjectManagement.Presentation.Infrastructure;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException error) return false;

        var status = error switch
        {
            ProjectNotFoundException => StatusCodes.Status404NotFound,
            InvalidWebsiteUrlException or InvalidProjectIdException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status422UnprocessableEntity
        };
        var problem = new ProblemDetails
        {
            Status = status,
            Title = "The requested operation could not be completed.",
            Detail = error.Message,
            Instance = context.Request.Path
        };
        problem.Extensions["code"] = error.Code;
        switch (error)
        {
            case InvalidWebsiteUrlException e: problem.Extensions["attemptedUrl"] = e.AttemptedUrl; break;
            case UnsupportedWcagVersionException e: problem.Extensions["attemptedVersion"] = e.AttemptedVersion; break;
            case EmptyDomException e: problem.Extensions["projectId"] = e.ProjectId; break;
            case ProjectNotFoundException e: problem.Extensions["projectId"] = e.ProjectId; break;
            case InvalidProjectIdException e: problem.Extensions["attemptedProjectId"] = e.AttemptedProjectId; break;
        }
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem, options: null,
            contentType: "application/problem+json", cancellationToken: cancellationToken);
        return true;
    }
}
