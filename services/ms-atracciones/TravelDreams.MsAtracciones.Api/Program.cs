using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAtracciones.Api.Configuration;
using TravelDreams.MsAtracciones.Api.Grpc;
using TravelDreams.MsAtracciones.Business;
using TravelDreams.MsAtracciones.DataAccess.Context;
using TravelDreams.MsAtracciones.DataManagement;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AtraccionesDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:AtraccionesDb o ConnectionStrings:DefaultConnection no esta configurada.");
}

builder.Services.AddOpenApi();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<AtraccionesDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddAtraccionesDataManagement();
builder.Services.AddAtraccionesBusiness();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapGrpcService<AtraccionesAvailabilityGrpcService>();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "ms-atracciones",
    status = "running"
}));

app.Run();
