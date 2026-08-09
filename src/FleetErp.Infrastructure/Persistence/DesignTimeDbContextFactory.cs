using FleetErp.Application.Abstractions;
using FleetErp.Domain.Enums;
using FleetErp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FleetErp.Infrastructure.Persistence;

/// <summary>
/// Construye el contexto para las herramientas de EF Core (crear y aplicar
/// migraciones) sin levantar la aplicación. La cadena de conexión puede venir de
/// la variable de entorno <c>FLEETERP_CONNECTION</c>; para generar el SQL basta
/// con que sea sintácticamente válida.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FleetDbContext>
{
    private const string DefaultConnection =
        "Host=localhost;Port=5432;Database=fleeterp;Username=postgres;Password=admin";

    public FleetDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("FLEETERP_CONNECTION") ?? DefaultConnection;

        var options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseNpgsql(connection, npgsql => npgsql.MigrationsAssembly(typeof(FleetDbContext).Assembly.FullName))
            .Options;

        // En tiempo de diseño no hay petición ni usuario: se usan implementaciones
        // neutras para que el modelo se pueda construir igual.
        return new FleetDbContext(options, new CurrentTenant(), new DesignTimeUser(), new SystemClock());
    }

    private sealed class DesignTimeUser : ICurrentUser
    {
        public Guid? UserId => null;
        public string? Email => "migraciones";
        public UserRole? Role => UserRole.Administrator;
    }
}
