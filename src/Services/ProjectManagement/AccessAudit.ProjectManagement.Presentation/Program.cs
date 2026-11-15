using AccessAudit.ProjectManagement.Application.Services;
using AccessAudit.ProjectManagement.Infrastructure;
using AccessAudit.ProjectManagement.Infrastructure.Persistence.Seeders;
using AccessAudit.ProjectManagement.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IProjectManagementService, ProjectManagementService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<IProjectManagementDataSeeder>().SeedAsync();
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "AccessAudit.ProjectManagement",
    status = "Running",
    scope = "Lab 4: EF Core persistence, audit lifecycle and imported screen captures",
    openApi = "/openapi/v1.json"
}));

app.Run();
