using FleetErp.Application.Abstractions;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FleetErp.Infrastructure.Persistence.Seed;

/// <summary>
/// Carga inicial de una empresa de demostración con historia suficiente para que
/// los tableros tengan algo que mostrar. Es idempotente: si ya existe la empresa,
/// no vuelve a sembrar.
/// </summary>
/// <remarks>
/// La aleatoriedad usa una semilla fija a propósito: cada demo ante un cliente
/// debe verse igual que la anterior.
/// </remarks>
public sealed class DemoDataSeeder(
    FleetDbContext context,
    ICurrentTenant tenant,
    IPasswordHasher hasher,
    IClock clock,
    ILogger<DemoDataSeeder> logger)
{
    public const string DemoSlug = "demo";
    public const string DemoAdminEmail = "admin@demo.com";
    public const string DemoPassword = "Demo123$";

    private const int HistoryDays = 45;
    private readonly Random _random = new(20260804);

    public async Task SeedAsync(CancellationToken ct = default)
    {
        using var _ = tenant.BypassFilter();

        if (await context.Tenants.AnyAsync(t => t.Slug == DemoSlug, ct))
        {
            logger.LogInformation("La empresa de demostración ya existe; no se vuelve a sembrar.");
            return;
        }

        var company = CreateTenant();
        context.Tenants.Add(company);
        tenant.Set(company.Id, company.Slug);

        var users = CreateUsers(company.Id);
        var vehicleTypes = CreateVehicleTypes(company.Id);
        var expenseCategories = CreateExpenseCategories(company.Id);
        var customFields = CreateCustomFields(company.Id);
        var vehicles = CreateVehicles(company.Id, vehicleTypes);
        var drivers = CreateDrivers(company.Id);
        var customers = CreateCustomers(company.Id);

        context.Users.AddRange(users);
        context.VehicleTypes.AddRange(vehicleTypes);
        context.ExpenseCategories.AddRange(expenseCategories);
        context.CustomFieldDefinitions.AddRange(customFields);
        context.Vehicles.AddRange(vehicles);
        context.Drivers.AddRange(drivers);
        context.Customers.AddRange(customers);

        await context.SaveChangesAsync(ct);

        var (trips, fuelLogs, expenses, maintenance) =
            CreateOperationalHistory(company, vehicles, drivers, customers, expenseCategories);

        context.Trips.AddRange(trips);
        context.FuelLogs.AddRange(fuelLogs);
        context.Expenses.AddRange(expenses);
        context.MaintenanceOrders.AddRange(maintenance);

        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "Datos de demostración listos: {Trips} viajes, {Vehicles} unidades, {Drivers} conductores.",
            trips.Count, vehicles.Count, drivers.Count);
    }

    // ---- Empresa y usuarios ----------------------------------------------

    private static Tenant CreateTenant()
    {
        var company = new Tenant("Transportes del Norte", DemoSlug, new TenantSettings
        {
            CurrencyCode = "MXN",
            CurrencySymbol = "$",
            TimeZoneId = "America/Mexico_City",
            Locale = "es-MX",
            DistanceUnit = DistanceUnit.Kilometer,
            VolumeUnit = VolumeUnit.Liter,
            WeightUnit = WeightUnit.Kilogram,
            DefaultDriverPayScheme = DriverPayScheme.PerHour,
            DefaultDriverPayRate = 95m,
            DefaultFuelPricePerUnit = 25.90m,
            TripFolioPrefix = "VJ",
            BrandPrimaryColor = "#0E7C66",
            LicenseExpiryAlertDays = 30,
            MinAcceptableFuelEfficiency = 2.2m
        });
        company.SetContact("TNO250101ABC", "operaciones@transportesdelnorte.mx", "81 8000 1234");
        return company;
    }

    private List<AppUser> CreateUsers(Guid tenantId) =>
    [
        new(DemoAdminEmail, "Ana Ramírez", hasher.Hash(DemoPassword), UserRole.Administrator) { TenantId = tenantId },
        new("despacho@demo.com", "Luis Cárdenas", hasher.Hash(DemoPassword), UserRole.Dispatcher) { TenantId = tenantId },
        new("consulta@demo.com", "Mónica Peña", hasher.Hash(DemoPassword), UserRole.Viewer) { TenantId = tenantId }
    ];

    // ---- Catálogos --------------------------------------------------------

    private static List<VehicleType> CreateVehicleTypes(Guid tenantId) =>
    [
        new("TRACTO", "Tractocamión", VehicleCategory.Motorized) { TenantId = tenantId },
        new("RABON", "Rabón 8 toneladas", VehicleCategory.Motorized) { TenantId = tenantId },
        new("TORTON", "Torton 14 toneladas", VehicleCategory.Motorized) { TenantId = tenantId },
        new("CAJA53", "Caja seca 53 pies", VehicleCategory.Towed) { TenantId = tenantId },
        new("REFRI", "Caja refrigerada 48 pies", VehicleCategory.Towed) { TenantId = tenantId },
        new("PLATAF", "Plataforma", VehicleCategory.Towed) { TenantId = tenantId }
    ];

    private static List<ExpenseCategory> CreateExpenseCategories(Guid tenantId) =>
    [
        new("CASETAS", "Casetas y peajes") { TenantId = tenantId },
        new("VIATICOS", "Viáticos del operador") { TenantId = tenantId },
        new("MANIOBRA", "Maniobras de carga y descarga") { TenantId = tenantId },
        new("REFACC", "Refacciones y llantas") { TenantId = tenantId },
        new("MULTAS", "Multas e infracciones") { TenantId = tenantId },
        new("ADMIN", "Gastos administrativos", isTripRelated: false) { TenantId = tenantId }
    ];

    /// <summary>
    /// Ejemplo vivo de la parametrización: estos tres campos no existen en el
    /// producto base y aparecen en los formularios sin haber tocado código.
    /// </summary>
    private static List<CustomFieldDefinition> CreateCustomFields(Guid tenantId)
    {
        return
        [
            new(CustomFieldTarget.Trip, "permiso_sct", "Permiso SCT", CustomFieldType.Text,
                displayOrder: 1) { TenantId = tenantId },

            new(CustomFieldTarget.Trip, "tipo_carga", "Tipo de carga", CustomFieldType.Select,
                options: "General|Refrigerada|Peligrosa|Granel", displayOrder: 2) { TenantId = tenantId },

            new(CustomFieldTarget.Vehicle, "gps_id", "Identificador GPS", CustomFieldType.Text,
                displayOrder: 1) { TenantId = tenantId }
        ];
    }

    // ---- Inventario y plantilla ------------------------------------------

    private List<Vehicle> CreateVehicles(Guid tenantId, List<VehicleType> types)
    {
        var tracto = types.First(t => t.Code == "TRACTO").Id;
        var rabon = types.First(t => t.Code == "RABON").Id;
        var torton = types.First(t => t.Code == "TORTON").Id;
        var caja = types.First(t => t.Code == "CAJA53").Id;
        var refri = types.First(t => t.Code == "REFRI").Id;
        var plataforma = types.First(t => t.Code == "PLATAF").Id;

        var today = clock.Today;

        var specs = new (string Eco, string Plate, Guid Type, string Brand, string Model, int Year, decimal Capacity, decimal Tank, decimal Odometer, int InsuranceDays)[]
        {
            ("T-101", "AB-123-CD", tracto, "Kenworth", "T680", 2021, 30000m, 700m, 384_500m, 210),
            ("T-102", "AB-456-CD", tracto, "Freightliner", "Cascadia", 2020, 30000m, 680m, 512_300m, 95),
            ("T-103", "AB-789-CD", tracto, "International", "LT625", 2022, 30000m, 720m, 198_700m, 18),
            ("R-201", "CD-321-EF", rabon, "Isuzu", "Forward", 2023, 8000m, 200m, 76_400m, 300),
            ("R-202", "CD-654-EF", torton, "Hino", "FM 500", 2022, 14000m, 300m, 141_200m, 150),
            ("C-301", "EF-111-GH", caja, "Utility", "3000R", 2019, 28000m, 0m, 0m, 240),
            ("C-302", "EF-222-GH", caja, "Great Dane", "Champion", 2020, 28000m, 0m, 0m, 260),
            ("C-303", "EF-333-GH", refri, "Thermo King", "Precedent", 2021, 24000m, 0m, 0m, 120),
            ("P-401", "EF-444-GH", plataforma, "Lufkin", "Flatbed 48", 2018, 26000m, 0m, 0m, 60)
        };

        var vehicles = new List<Vehicle>();
        foreach (var s in specs)
        {
            var vehicle = new Vehicle(s.Eco, s.Plate, s.Type) { TenantId = tenantId };
            vehicle.SetSpecs(s.Brand, s.Model, s.Year, $"3AKJG{_random.Next(100000, 999999)}", s.Capacity,
                             s.Tank == 0 ? null : s.Tank);
            vehicle.SetInitialOdometer(s.Odometer);
            vehicle.SetCompliance(today.AddDays(s.InsuranceDays), today.AddDays(s.InsuranceDays + 45));
            if (s.Eco == "T-101") vehicle.CustomFields.Set("gps_id", "GPS-0091");
            vehicles.Add(vehicle);
        }

        // Una unidad en taller para que el tablero muestre la flotilla incompleta,
        // que es como se ve en la vida real.
        vehicles.First(v => v.EconomicNumber == "R-202").SendToMaintenance();

        return vehicles;
    }

    private List<Driver> CreateDrivers(Guid tenantId)
    {
        var today = clock.Today;
        var specs = new (string First, string Last, string License, DriverPayScheme Scheme, decimal Rate, int LicenseDays)[]
        {
            ("Ulises", "Mendoza", "LIC-889001", DriverPayScheme.PerHour, 110m, 400),
            ("Juanito", "Pérez", "LIC-889002", DriverPayScheme.PerHour, 95m, 22),
            ("Rogelio", "Salinas", "LIC-889003", DriverPayScheme.PerKilometer, 3.20m, 260),
            ("Carmen", "Ibarra", "LIC-889004", DriverPayScheme.PerHour, 105m, 540),
            ("Efraín", "González", "LIC-889005", DriverPayScheme.PercentageOfRevenue, 12m, 130),
            ("Marisol", "Aguilar", "LIC-889006", DriverPayScheme.FixedPerTrip, 2800m, 75)
        };

        var drivers = new List<Driver>();
        var index = 1;
        foreach (var s in specs)
        {
            var driver = new Driver(s.First, s.Last, s.License) { TenantId = tenantId };
            driver.SetLicense(s.License, "Federal Tipo E", today.AddDays(s.LicenseDays));
            driver.SetContact($"OP-{index:D3}", $"81 15{_random.Next(10, 99)} {_random.Next(1000, 9999)}",
                              $"{s.First.ToLowerInvariant()}@transportesdelnorte.mx", today.AddDays(-_random.Next(200, 1500)));
            driver.SetCompensation(s.Scheme, s.Rate);
            drivers.Add(driver);
            index++;
        }
        return drivers;
    }

    private static List<Customer> CreateCustomers(Guid tenantId)
    {
        var names = new[]
        {
            ("Cementos del Bajío", "CBA150320XY1"),
            ("Agroindustrias Sinaloa", "ASI180712QQ2"),
            ("Distribuidora Monterrey", "DMO200105LL3"),
            ("Refrigerados del Golfo", "RGO170228MM4")
        };

        return names.Select(n =>
        {
            var customer = new Customer(n.Item1) { TenantId = tenantId };
            customer.SetContact(n.Item2, "Compras", "81 8100 0000", "compras@cliente.mx", "Parque Industrial");
            return customer;
        }).ToList();
    }

    // ---- Historia operativa ----------------------------------------------

    /// <summary>Rutas típicas de la empresa demo, con su distancia de referencia.</summary>
    private static readonly (string Origin, string Destination, decimal Km)[] Routes =
    [
        ("Monterrey, NL", "Ciudad de México", 920m),
        ("Monterrey, NL", "Guadalajara, JAL", 780m),
        ("Saltillo, COAH", "Querétaro, QRO", 640m),
        ("Monterrey, NL", "Nuevo Laredo, TAMPS", 225m),
        ("San Luis Potosí, SLP", "Monterrey, NL", 515m),
        ("Monterrey, NL", "Torreón, COAH", 400m),
        ("Culiacán, SIN", "Monterrey, NL", 1120m)
    ];

    /// <summary>Estado compartido por las dos fases del sembrado de viajes.</summary>
    private sealed class HistoryBuilder
    {
        public required Tenant Company { get; init; }
        public required List<Vehicle> Tractors { get; init; }
        public required List<Vehicle> Trailers { get; init; }
        public required List<Driver> Drivers { get; init; }
        public required List<Customer> Customers { get; init; }
        public required List<ExpenseCategory> TripCategories { get; init; }
        public required Dictionary<Guid, decimal> Odometers { get; init; }

        public List<Trip> Trips { get; } = [];
        public List<FuelLog> FuelLogs { get; } = [];
        public List<Expense> Expenses { get; } = [];

        /// <summary>Recursos tomados por un viaje en ruta: nadie va en dos viajes a la vez.</summary>
        public HashSet<Guid> BusyDrivers { get; } = [];
        public HashSet<Guid> BusyVehicles { get; } = [];

        public int Sequence { get; set; } = 1;
    }

    private (List<Trip> Trips, List<FuelLog> Fuel, List<Expense> Expenses, List<MaintenanceOrder> Maintenance)
        CreateOperationalHistory(Tenant company, List<Vehicle> vehicles, List<Driver> drivers,
                                 List<Customer> customers, List<ExpenseCategory> categories)
    {
        var builder = new HistoryBuilder
        {
            Company = company,
            Tractors = vehicles.Where(v => v.EconomicNumber.StartsWith('T') || v.EconomicNumber.StartsWith('R')).ToList(),
            Trailers = vehicles.Where(v => v.EconomicNumber.StartsWith('C') || v.EconomicNumber.StartsWith('P')).ToList(),
            Drivers = drivers,
            Customers = customers,
            TripCategories = categories.Where(c => c.IsTripRelated).ToList(),
            Odometers = vehicles.ToDictionary(v => v.Id, v => v.CurrentOdometer)
        };

        CreateClosedHistory(builder);
        CreateTodaysOperation(builder);

        SyncFleetStatus(vehicles, drivers, builder.Trips, builder.Odometers);

        return (builder.Trips, builder.FuelLogs, builder.Expenses, CreateMaintenance(company.Id, vehicles));
    }

    /// <summary>Viajes ya cerrados o cancelados de días anteriores: alimentan gráficas y totales.</summary>
    private void CreateClosedHistory(HistoryBuilder builder)
    {
        var today = clock.UtcNow.UtcDateTime.Date;

        for (var dayOffset = HistoryDays; dayOffset >= 1; dayOffset--)
        {
            var day = today.AddDays(-dayOffset);

            for (var i = 0; i < _random.Next(1, 4); i++)
            {
                var route = Routes[_random.Next(Routes.Length)];
                var departure = new DateTimeOffset(day.AddHours(5 + _random.Next(0, 10)), TimeSpan.Zero);

                var trip = BuildTrip(builder, route, departure,
                    builder.Drivers[_random.Next(builder.Drivers.Count)],
                    builder.Tractors[_random.Next(builder.Tractors.Count)],
                    builder.Trailers[_random.Next(builder.Trailers.Count)],
                    out var expectedHours);

                if (_random.Next(0, 20) == 0)
                {
                    trip.Cancel("El cliente reprogramó la carga.");
                    builder.Trips.Add(trip);
                    continue;
                }

                DispatchAndComplete(builder, trip, route, expectedHours);
                builder.Trips.Add(trip);
            }
        }
    }

    /// <summary>
    /// Operación del día: viajes que ya salieron y llegaron, unidades todavía en
    /// ruta y salidas programadas para más tarde. Es lo que da contenido al bloque
    /// de entradas y salidas del tablero.
    /// </summary>
    private void CreateTodaysOperation(HistoryBuilder builder)
    {
        var now = clock.UtcNow;
        var shortRoutes = Routes.Where(r => r.Km <= 450m).ToArray();

        // Dos viajes cortos que salieron y ya llegaron hoy.
        for (var i = 0; i < 2; i++)
        {
            var route = shortRoutes[i % shortRoutes.Length];
            var trip = BuildTrip(builder, route, now.AddHours(-(11 + i * 2)),
                PickFreeDriver(builder), PickFreeTractor(builder),
                builder.Trailers[_random.Next(builder.Trailers.Count)], out var expectedHours);

            DispatchAndComplete(builder, trip, route, Math.Min(expectedHours, 8));
            builder.Trips.Add(trip);
        }

        // Dos unidades circulando en este momento, cada una con su propio operador.
        for (var i = 0; i < 2; i++)
        {
            var route = Routes[_random.Next(Routes.Length)];
            var driver = PickFreeDriver(builder);
            var tractor = PickFreeTractor(builder);
            var trailer = PickFreeTrailer(builder);

            var trip = BuildTrip(builder, route, now.AddHours(-(2 + i * 3)), driver, tractor, trailer, out _);
            trip.Dispatch(trip.ScheduledDepartureUtc, builder.Odometers[tractor.Id], trip.InitialFuel);

            builder.BusyDrivers.Add(driver.Id);
            builder.BusyVehicles.Add(tractor.Id);
            builder.BusyVehicles.Add(trailer.Id);
            builder.Trips.Add(trip);
        }

        // Dos salidas programadas para más tarde: la bandeja de despacho no está vacía.
        for (var i = 0; i < 2; i++)
        {
            var route = Routes[_random.Next(Routes.Length)];
            var trip = BuildTrip(builder, route, now.AddHours(4 + i * 3),
                PickFreeDriver(builder), PickFreeTractor(builder), PickFreeTrailer(builder), out _);
            builder.Trips.Add(trip);
        }
    }

    private Driver PickFreeDriver(HistoryBuilder builder) =>
        builder.Drivers.FirstOrDefault(d => !builder.BusyDrivers.Contains(d.Id))
        ?? builder.Drivers[_random.Next(builder.Drivers.Count)];

    private Vehicle PickFreeTractor(HistoryBuilder builder) =>
        builder.Tractors.FirstOrDefault(v => !builder.BusyVehicles.Contains(v.Id) && v.Status == VehicleStatus.Available)
        ?? builder.Tractors[_random.Next(builder.Tractors.Count)];

    private Vehicle PickFreeTrailer(HistoryBuilder builder) =>
        builder.Trailers.FirstOrDefault(v => !builder.BusyVehicles.Contains(v.Id))
        ?? builder.Trailers[_random.Next(builder.Trailers.Count)];

    /// <summary>Arma un viaje en planeación con todos los datos que captura el despachador.</summary>
    private Trip BuildTrip(HistoryBuilder builder, (string Origin, string Destination, decimal Km) route,
                           DateTimeOffset departure, Driver driver, Vehicle tractor, Vehicle trailer,
                           out double expectedHours)
    {
        expectedHours = (double)(route.Km / 62m) + _random.Next(3, 7);

        var folio = $"VJ-{departure.Year}-{builder.Sequence:D6}";
        builder.Sequence++;

        var trip = new Trip(folio, driver.Id, tractor.Id, route.Origin, route.Destination, departure)
        {
            TenantId = builder.Company.Id
        };

        trip.SetAssignment(driver.Id, tractor.Id, trailer.Id,
            builder.Customers[_random.Next(builder.Customers.Count)].Id);
        trip.SetRoute(route.Origin, route.Destination, route.Km);
        trip.SetSchedule(departure, departure.AddHours(expectedHours));
        trip.SetCargo(_random.Next(8, 28) * 1000m, WeightUnit.Kilogram, "Carga consolidada");
        trip.SetFuelPlan(_random.Next(200, 500), route.Km > 500);
        trip.SetCommercialTerms(Math.Round(route.Km * _random.Next(28, 42), 2), driver.PayScheme, driver.PayRate);
        trip.CustomFields.Set("tipo_carga", _random.Next(0, 4) switch
        {
            0 => "General", 1 => "Refrigerada", 2 => "Peligrosa", _ => "Granel"
        });

        return trip;
    }

    /// <summary>Despacha y cierra el viaje, con su carga de combustible y sus gastos de ruta.</summary>
    private void DispatchAndComplete(HistoryBuilder builder, Trip trip,
                                     (string Origin, string Destination, decimal Km) route, double expectedHours)
    {
        var odometerStart = builder.Odometers[trip.VehicleId];
        var departure = trip.ScheduledDepartureUtc;

        trip.Dispatch(departure, odometerStart, trip.InitialFuel);

        var actualKm = route.Km * (1 + _random.Next(-3, 8) / 100m);
        var odometerEnd = Math.Round(odometerStart + actualKm, 2);

        // Con esta dispersión alrededor de 7 de cada 10 viajes llegan a tiempo,
        // que es lo que se ve en una operación sana.
        var arrival = departure.AddHours(expectedHours + _random.Next(-4, 3));
        var consumed = actualKm / (decimal)(2.0 + _random.NextDouble());
        var finalFuel = Math.Max(20m, trip.InitialFuel + 300m - consumed);

        trip.Complete(arrival, odometerEnd, Math.Round(finalFuel, 2),
                      Math.Round((decimal)(arrival - departure).TotalHours, 2));
        builder.Odometers[trip.VehicleId] = odometerEnd;

        if (trip.RefuelPlanned)
        {
            var price = builder.Company.Settings.DefaultFuelPricePerUnit + _random.Next(-150, 150) / 100m;
            var log = new FuelLog(trip.VehicleId, departure.AddHours(2), _random.Next(180, 400), Math.Round(price, 2))
            {
                TenantId = builder.Company.Id
            };
            log.SetContext(trip.Id, trip.DriverId, Math.Round(odometerStart + actualKm / 3, 2),
                           "Estación Pemex km 120", $"TICKET-{_random.Next(10000, 99999)}");
            builder.FuelLogs.Add(log);
        }

        foreach (var category in builder.TripCategories.Where(_ => _random.Next(0, 3) > 0).Take(3))
        {
            var amount = category.Code switch
            {
                "CASETAS" => _random.Next(600, 2600),
                "VIATICOS" => _random.Next(400, 1200),
                "MANIOBRA" => _random.Next(300, 900),
                "REFACC" => _random.Next(500, 3500),
                _ => _random.Next(200, 800)
            };
            var expense = new Expense(category.Id, departure.AddHours(3), amount, category.Name)
            {
                TenantId = builder.Company.Id
            };
            expense.SetContext(trip.Id, trip.VehicleId, trip.DriverId, null);
            builder.Expenses.Add(expense);
        }
    }

    /// <summary>
    /// Deja unidades y conductores en el estado coherente con los viajes sembrados:
    /// si un viaje quedó en ruta, su unidad y su operador deben aparecer ocupados.
    /// </summary>
    private static void SyncFleetStatus(List<Vehicle> vehicles, List<Driver> drivers, List<Trip> trips,
                                        Dictionary<Guid, decimal> odometers)
    {
        foreach (var vehicle in vehicles.Where(v => odometers.TryGetValue(v.Id, out var reading) && reading > v.CurrentOdometer))
            vehicle.UpdateOdometer(odometers[vehicle.Id]);

        foreach (var trip in trips.Where(t => t.Status == TripStatus.InProgress))
        {
            var vehicle = vehicles.First(v => v.Id == trip.VehicleId);
            if (vehicle.Status == VehicleStatus.Available) vehicle.MarkOnTrip();

            var trailer = trip.TrailerId is null ? null : vehicles.FirstOrDefault(v => v.Id == trip.TrailerId);
            if (trailer is { Status: VehicleStatus.Available }) trailer.MarkOnTrip();

            var driver = drivers.First(d => d.Id == trip.DriverId);
            if (driver.Status == DriverStatus.Active) driver.MarkOnTrip();
        }
    }

    private List<MaintenanceOrder> CreateMaintenance(Guid tenantId, List<Vehicle> vehicles)
    {
        var now = clock.UtcNow;
        var inShop = vehicles.First(v => v.EconomicNumber == "R-202");
        var serviced = vehicles.First(v => v.EconomicNumber == "T-102");

        var open = new MaintenanceOrder($"OS-{now.Year}-000001", inShop.Id, MaintenanceKind.Corrective,
            now.AddDays(-3), "Falla en sistema de frenos traseros.") { TenantId = tenantId };
        open.Start();

        var closed = new MaintenanceOrder($"OS-{now.Year}-000002", serviced.Id, MaintenanceKind.Preventive,
            now.AddDays(-20), "Servicio mayor de 500 mil kilómetros.") { TenantId = tenantId };
        closed.Close(now.AddDays(-18), 28_450m, "Taller Diésel Norte", serviced.CurrentOdometer);

        return [open, closed];
    }

}
