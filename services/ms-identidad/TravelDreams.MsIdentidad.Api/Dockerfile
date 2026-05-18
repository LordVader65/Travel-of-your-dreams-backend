FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish services/ms-identidad/TravelDreams.MsIdentidad.Api/TravelDreams.MsIdentidad.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "dotnet TravelDreams.MsIdentidad.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
