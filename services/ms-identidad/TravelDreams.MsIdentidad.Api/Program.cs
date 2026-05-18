using Microsoft.EntityFrameworkCore;
using TravelDreams.MsIdentidad.Api.Configuration;
using TravelDreams.MsIdentidad.Business;
using TravelDreams.MsIdentidad.DataAccess.Context;
using TravelDreams.MsIdentidad.DataManagement;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("IdentidadDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:IdentidadDb o ConnectionStrings:DefaultConnection no esta configurada.");
}

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<IdentidadDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentidadDataManagement();
builder.Services.AddIdentidadBusiness();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "ms-identidad",
    status = "running"
}));

app.Run();
