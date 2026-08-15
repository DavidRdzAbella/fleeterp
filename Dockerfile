# Fase de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia la solución principal y todos los proyectos
COPY ["FleetErp.sln", "./"]
COPY ["src/FleetErp.Api/FleetErp.Api.csproj", "src/FleetErp.Api/"]
COPY ["src/FleetErp.Application/FleetErp.Application.csproj", "src/FleetErp.Application/"]
COPY ["src/FleetErp.Domain/FleetErp.Domain.csproj", "src/FleetErp.Domain/"]
COPY ["src/FleetErp.Infrastructure/FleetErp.Infrastructure.csproj", "src/FleetErp.Infrastructure/"]
COPY ["src/FleetErp.Web/FleetErp.Web.csproj", "src/FleetErp.Web/"]
COPY ["tests/FleetErp.UnitTests/FleetErp.UnitTests.csproj", "tests/FleetErp.UnitTests/"]

# Restaura las dependencias
RUN dotnet restore "FleetErp.sln"

# Copia el resto del código
COPY . .

# Publica el proyecto web
RUN dotnet publish "src/FleetErp.Web/FleetErp.Web.csproj" -c Release -o /app/publish

# Fase de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# FORZAMOS el uso de polling para evitar el error de límite de inotify en Linux
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
# Aseguramos el entorno de producción
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FleetErp.Web.dll"]