FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy sln + csproj first for better layer caching
COPY AtraccionesTuristicas.Backend.LA.Api/AtraccionesTuristicas.Backend.LA.Api.csproj AtraccionesTuristicas.Backend.LA.Api/
COPY AtraccionesTuristicas.Backend.LA.Business/AtraccionesTuristicas.Backend.LA.Business.csproj AtraccionesTuristicas.Backend.LA.Business/
COPY AtraccionesTuristicas.Backend.LA.DataAccess/AtraccionesTuristicas.Backend.LA.DataAccess.csproj AtraccionesTuristicas.Backend.LA.DataAccess/
COPY AtraccionesTuristicas.Backend.LA.DataManagement/AtraccionesTuristicas.Backend.LA.DataManagement.csproj AtraccionesTuristicas.Backend.LA.DataManagement/

RUN dotnet restore AtraccionesTuristicas.Backend.LA.Api/AtraccionesTuristicas.Backend.LA.Api.csproj

COPY . .
RUN dotnet publish AtraccionesTuristicas.Backend.LA.Api/AtraccionesTuristicas.Backend.LA.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render provides $PORT at runtime; default to 8080 for local/docker.
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet AtraccionesTuristicas.Backend.LA.Api.dll"]
