using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Application.Services;
using AccessAudit.ProjectManagement.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();
builder.Services.AddSingleton<IAuditScopeRepository, InMemoryAuditScopeRepository>();
builder.Services.AddScoped<IProjectManagementService, ProjectManagementService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "AccessAudit.ProjectManagement",
    status = "Running",
    scope = "Lab 3: project registration, audit parameters and imported screen captures",
    openApi = "/openapi/v1.json"
}));

app.Run();