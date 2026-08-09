using FleetErp.Application.Abstractions;
using FleetErp.Domain.Abstractions;
using FleetErp.Infrastructure.Identity;
using FleetErp.Infrastructure.Persistence;
using FleetErp.Infrastructure.Persistence.Queries;
using FleetErp.Infrastructure.Persistence.Repositories;
using FleetErp.Infrastructure.Persistence.Seed;
using FleetErp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FleetErp.Infrastructure;

/// <summary>Proveedores de base de datos soportados por el host.</summary>
public enum DatabaseProvider
{
    /// <summary>PostgreSQL: el destino real de producción.</summary>
    PostgreSql = 0,

    /// <summary>
    /// Base en memoria para llevar la demostración a un equipo sin instalar nada.
    /// Los datos se pierden al cerrar la aplicación.
    /// </summary>
    InMemory = 1
}

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = ResolveProvider(configuration);

        services.AddDbContext<FleetDbContext>(options =>
        {
            if (provider == DatabaseProvider.InMemory)
            {
                options.UseInMemoryDatabase("fleet-erp-demo");
                return;
            }

            var connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'Postgres'. Configúrela o use Database:Provider=InMemory para la demo.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(FleetDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure(3);
            });
        });

        // Contexto de la petición: la API los llena, el resto del sistema los lee.
        services.AddScoped<CurrentTenant>();
        services.AddScoped<ICurrentTenant>(sp => sp.GetRequiredService<CurrentTenant>());
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IFolioGenerator, FolioGenerator>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITripQueries, TripQueries>();
        services.AddScoped<IVehicleQueries, VehicleQueries>();
        services.AddScoped<IDriverQueries, DriverQueries>();
        services.AddScoped<ICustomerQueries, CustomerQueries>();
        services.AddScoped<IExpenseQueries, ExpenseQueries>();
        services.AddScoped<IFuelLogQueries, FuelLogQueries>();
        services.AddScoped<IMaintenanceQueries, MaintenanceQueries>();
        services.AddScoped<IUserQueries, UserQueries>();
        services.AddScoped<IAnalyticsDataSource, EfAnalyticsDataSource>();

        services.AddScoped<DemoDataSeeder>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        return services;
    }

    public static DatabaseProvider ResolveProvider(IConfiguration configuration) =>
        Enum.TryParse<DatabaseProvider>(configuration["Database:Provider"], ignoreCase: true, out var parsed)
            ? parsed
            : DatabaseProvider.PostgreSql;
}
