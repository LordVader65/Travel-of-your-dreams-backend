using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.Api.Configuration;
using TravelDreams.MsFacturacion.Api.Infrastructure.EventBus.V3;
using TravelDreams.MsFacturacion.Api.Middleware;
using TravelDreams.MsFacturacion.Business;
using TravelDreams.MsFacturacion.DataAccess.Context;
using TravelDreams.MsFacturacion.DataManagement;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("FacturacionDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:FacturacionDb o ConnectionStrings:DefaultConnection no esta configurada.");
}

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<FacturacionDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddFacturacionDataManagement();
builder.Services.AddFacturacionBusiness();
builder.Services.AddFacturacionEventBusV3(builder.Configuration);

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
    service = "ms-facturacion",
    status = "running"
}));

app.Run();
