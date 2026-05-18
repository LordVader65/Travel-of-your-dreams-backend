using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.Api.Configuration;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "ms-facturacion",
    status = "running"
}));

app.Run();
