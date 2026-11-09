var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { service = "AccessAudit.ComplianceAndAI", status = "Skeleton only — Lab 3" }));
app.Run();
