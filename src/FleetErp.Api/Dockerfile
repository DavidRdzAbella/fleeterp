# Imagen de la API. Se construye desde la raíz de la solución:
#   docker build -f src/FleetErp.Api/Dockerfile -t fleeterp-api .
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Los .csproj se copian primero para que la restauración quede en una capa
# aparte y no se repita cada vez que cambia el código.
COPY FleetErp.sln ./
COPY src/FleetErp.Domain/FleetErp.Domain.csproj src/FleetErp.Domain/
COPY src/FleetErp.Application/FleetErp.Application.csproj src/FleetErp.Application/
COPY src/FleetErp.Infrastructure/FleetErp.Infrastructure.csproj src/FleetErp.Infrastructure/
COPY src/FleetErp.Api/FleetErp.Api.csproj src/FleetErp.Api/
RUN dotnet restore src/FleetErp.Api/FleetErp.Api.csproj

COPY src/ src/
RUN dotnet publish src/FleetErp.Api/FleetErp.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Sin privilegios: la API no necesita root para nada.
USER $APP_UID

ENTRYPOINT ["dotnet", "FleetErp.Api.dll"]
