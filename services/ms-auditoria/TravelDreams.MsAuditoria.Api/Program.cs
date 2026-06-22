using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAuditoria.Api.Configuration;
using TravelDreams.MsAuditoria.Api.Infrastructure.EventBus.V3;
using TravelDreams.MsAuditoria.Api.Middleware;
using TravelDreams.MsAuditoria.Business;
using TravelDreams.MsAuditoria.DataAccess.Context;
using TravelDreams.MsAuditoria.DataManagement;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AuditoriaDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:AuditoriaDb o ConnectionStrings:DefaultConnection no esta configurada.");
}

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<AuditoriaDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddAuditoriaDataManagement();
builder.Services.AddAuditoriaBusiness();
builder.Services.AddAuditoriaEventBusV3(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "ms-auditoria",
    status = "running"
}));

app.Run();
