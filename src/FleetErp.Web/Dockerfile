# Imagen del portal. Se construye desde la raíz de la solución:
#   docker build -f src/FleetErp.Web/Dockerfile -t fleeterp-web .
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FleetErp.sln ./
COPY src/FleetErp.Web/FleetErp.Web.csproj src/FleetErp.Web/
RUN dotnet restore src/FleetErp.Web/FleetErp.Web.csproj

COPY src/FleetErp.Web/ src/FleetErp.Web/
RUN dotnet publish src/FleetErp.Web/FleetErp.Web.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "FleetErp.Web.dll"]
