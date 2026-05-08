using AtraccionesTuristicas.Backend.LA.Api.Configuration;
using AtraccionesTuristicas.Backend.LA.Api.Middleware;
using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.Configuration;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var connectionString = builder.Configuration.GetConnectionString("AtraccionesDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings: AtraccionesDb no está configurada.");
}

builder.Services.AddDbContext<AtraccionesDbContext>(options =>
    options.UseNpgsql(connectionString));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
{
    jwtOptions.SecretKey = "development-secret-development-secret";
}

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddDataManagementLayer();
builder.Services.AddBusinessLayer(jwtOptions);
builder.Services.AddScoped<ICurrentUserFactory, CurrentUserFactory>();
builder.Services.AddApiControllers();
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddJwtAuthentication(jwtOptions);
builder.Services.AddApiAuthorization();
builder.Services.AddApiSwagger();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (builder.Configuration.GetValue("Swagger:Enabled", app.Environment.IsDevelopment()))
{
    app.UseMiddleware<SwaggerProtectionMiddleware>();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
