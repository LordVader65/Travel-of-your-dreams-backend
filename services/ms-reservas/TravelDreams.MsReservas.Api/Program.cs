using Microsoft.EntityFrameworkCore;
using TravelDreams.MsReservas.Api.Configuration;
using TravelDreams.MsReservas.Api.Grpc;
using TravelDreams.MsReservas.Business;
using TravelDreams.MsReservas.DataAccess.Context;
using TravelDreams.MsReservas.DataManagement;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ReservasDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:ReservasDb o ConnectionStrings:DefaultConnection no esta configurada.");
}

builder.Services.AddOpenApi();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<ReservasDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddReservasDataManagement();
builder.Services.AddReservasBusiness();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapGrpcService<ReservasInternalGrpcService>();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "ms-reservas",
    status = "running"
}));

app.Run();
