using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Application.Services;
using AccessAudit.ProjectManagement.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();
builder.Services.AddSingleton<IAuditScreenRepository, InMemoryAuditScreenRepository>();
builder.Services.AddScoped<IProjectManagementService, ProjectManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
