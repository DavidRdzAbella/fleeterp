using FleetErp.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FleetErp.Application;

/// <summary>
/// Registro de la capa de aplicación. Cada capa expone su propio módulo de
/// composición para que el host (API) no tenga que conocer las clases concretas.
/// </summary>
public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ITripService, TripService>();
        services.AddScoped<IFuelLogService, FuelLogService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        services.AddValidatorsFromAssemblyContaining<DependencyInjectionMarker>(ServiceLifetime.Scoped);

        return services;
    }
}

/// <summary>Ancla de ensamblado para el escaneo de validadores.</summary>
public sealed class DependencyInjectionMarker;
