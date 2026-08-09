namespace FleetErp.Web.Models;

public sealed record TripListItemModel(
    Guid Id, string Folio, TripStatus Status,
    string DriverName, string VehicleLabel, string? CustomerName,
    string Origin, string Destination,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ActualDepartureUtc, DateTimeOffset? ActualArrivalUtc,
    decimal PlannedDistance, decimal ActualDistance,
    decimal CargoWeight, WeightUnit CargoWeightUnit,
    decimal FreightRevenue, decimal TotalCost, decimal Profit, bool IsLate);

public sealed record TripDetailModel(
    Guid Id, string Folio, TripStatus Status,
    Guid DriverId, string DriverName,
    Guid VehicleId, string VehicleLabel,
    Guid? TrailerId, string? TrailerLabel,
    Guid? CustomerId, string? CustomerName,
    string Origin, string Destination, decimal PlannedDistance,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ScheduledArrivalUtc,
    DateTimeOffset? ActualDepartureUtc, DateTimeOffset? ActualArrivalUtc,
    decimal? OdometerStart, decimal? OdometerEnd, decimal ActualDistance,
    decimal InitialFuel, decimal? FinalFuel, bool RefuelPlanned,
    decimal FuelPurchased, decimal? FuelConsumed, decimal? FuelEfficiency,
    decimal CargoWeight, WeightUnit CargoWeightUnit, string? CargoDescription,
    decimal FreightRevenue, DriverPayScheme DriverPayScheme, decimal DriverPayRate,
    decimal? DriverHours, decimal DriverPayAmount,
    decimal FuelCost, decimal OtherExpensesCost, decimal TotalCost, decimal Profit, decimal ProfitMargin,
    bool IsLate, string? Notes, string? CancellationReason,
    IReadOnlyDictionary<string, string?> CustomFields,
    IReadOnlyList<FuelLogModel> FuelLogs,
    IReadOnlyList<ExpenseModel> Expenses);

public sealed record CreateTripRequest(
    Guid DriverId, Guid VehicleId, Guid? TrailerId, Guid? CustomerId,
    string Origin, string Destination, decimal PlannedDistance,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ScheduledArrivalUtc,
    decimal InitialFuel, bool RefuelPlanned,
    decimal CargoWeight, WeightUnit CargoWeightUnit, string? CargoDescription,
    decimal FreightRevenue, DriverPayScheme? DriverPayScheme, decimal? DriverPayRate,
    string? Notes, Dictionary<string, string?>? CustomFields);

public sealed record UpdateTripRequest(
    Guid DriverId, Guid VehicleId, Guid? TrailerId, Guid? CustomerId,
    string Origin, string Destination, decimal PlannedDistance,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ScheduledArrivalUtc,
    decimal InitialFuel, bool RefuelPlanned,
    decimal CargoWeight, WeightUnit CargoWeightUnit, string? CargoDescription,
    decimal FreightRevenue, DriverPayScheme DriverPayScheme, decimal DriverPayRate,
    string? Notes, Dictionary<string, string?>? CustomFields);

public sealed record DispatchTripRequest(DateTimeOffset DepartureUtc, decimal OdometerStart, decimal? InitialFuel);

public sealed record CompleteTripRequest(DateTimeOffset ArrivalUtc, decimal OdometerEnd, decimal? FinalFuel, decimal? DriverHours);

public sealed record CancelTripRequest(string Reason);

public sealed record FuelLogModel(
    Guid Id, Guid VehicleId, string VehicleLabel, Guid? TripId, string? TripFolio,
    Guid? DriverId, string? DriverName,
    DateTimeOffset LoadedAtUtc, decimal Quantity, decimal PricePerUnit, decimal TotalCost,
    decimal? OdometerReading, string? Station, string? ReferenceNumber);

public sealed record CreateFuelLogRequest(
    Guid VehicleId, Guid? TripId, Guid? DriverId,
    DateTimeOffset LoadedAtUtc, decimal Quantity, decimal PricePerUnit,
    decimal? OdometerReading, string? Station, string? ReferenceNumber);

public sealed record ExpenseModel(
    Guid Id, Guid CategoryId, string CategoryName,
    Guid? TripId, string? TripFolio, Guid? VehicleId, string? VehicleLabel,
    Guid? DriverId, string? DriverName,
    DateTimeOffset IncurredAtUtc, decimal Amount, string Description, string? ReferenceNumber);

public sealed record CreateExpenseRequest(
    Guid CategoryId, Guid? TripId, Guid? VehicleId, Guid? DriverId,
    DateTimeOffset IncurredAtUtc, decimal Amount, string Description, string? ReferenceNumber);

public sealed record UpdateExpenseRequest(
    Guid CategoryId, Guid? TripId, Guid? VehicleId, Guid? DriverId,
    DateTimeOffset IncurredAtUtc, decimal Amount, string Description, string? ReferenceNumber);

public sealed record UpdateFuelLogRequest(
    Guid VehicleId, Guid? TripId, Guid? DriverId,
    DateTimeOffset LoadedAtUtc, decimal Quantity, decimal PricePerUnit,
    decimal? OdometerReading, string? Station, string? ReferenceNumber);

// ---- Tableros -------------------------------------------------------------

public sealed record AnalyticsPeriodModel(DateTimeOffset FromUtc, DateTimeOffset ToUtc);

public sealed record TimeSeriesPointModel(DateOnly Date, decimal Value);

public sealed record NamedValueModel(string Label, decimal Value);

public sealed record FleetStatusModel(
    int TotalVehicles, int Available, int OnTrip, int InMaintenance, int OutOfService,
    int MotorizedUnits, int TowedUnits,
    int TotalDrivers, int DriversActive, int DriversOnTrip, decimal UtilizationRate);

public sealed record TripActivityModel(
    int Planned, int InProgress, int CompletedInPeriod, int Cancelled,
    int DeparturesToday, int ArrivalsToday, int LateArrivals,
    decimal OnTimeRate, decimal TotalDistance, decimal AverageDistancePerTrip);

public sealed record FinancialSummaryModel(
    decimal Revenue, decimal FuelCost, decimal FuelQuantity, decimal OtherExpenses,
    decimal DriverPay, decimal MaintenanceCost, decimal TotalCost,
    decimal Profit, decimal ProfitMargin,
    decimal RevenuePerDistanceUnit, decimal CostPerDistanceUnit, decimal AverageFuelEfficiency);

public sealed record AlertModel(string Severity, string Title, string Detail, string? Link);

public sealed record DriverRankingRowModel(
    int Rank, Guid DriverId, string DriverName,
    int Trips, decimal Distance, decimal Revenue, decimal FuelCost,
    decimal DriverPay, decimal Profit, decimal AverageFuelEfficiency, decimal OnTimeRate);

public sealed record FleetDashboardModel(
    AnalyticsPeriodModel Period,
    FleetStatusModel Fleet,
    TripActivityModel Activity,
    FinancialSummaryModel Financials,
    IReadOnlyList<TimeSeriesPointModel> DistanceByDay,
    IReadOnlyList<TimeSeriesPointModel> RevenueByDay,
    IReadOnlyList<TimeSeriesPointModel> ProfitByDay,
    IReadOnlyList<NamedValueModel> CostBreakdown,
    IReadOnlyList<DriverRankingRowModel> TopDrivers,
    IReadOnlyList<TripListItemModel> ActiveTrips,
    IReadOnlyList<AlertModel> Alerts);

public sealed record DriverPerformanceModel(
    Guid DriverId, string DriverName, AnalyticsPeriodModel Period,
    int Trips, int CompletedTrips, int LateTrips,
    decimal Distance, decimal FuelQuantity, decimal FuelCost,
    decimal Revenue, decimal DriverPay, decimal OtherExpenses,
    decimal TotalCost, decimal Profit, decimal ProfitMargin,
    decimal AverageFuelEfficiency, decimal OnTimeRate, decimal HoursWorked,
    IReadOnlyList<TimeSeriesPointModel> DistanceByDay,
    IReadOnlyList<TimeSeriesPointModel> RevenueByDay,
    IReadOnlyList<TripListItemModel> RecentTrips);

public sealed record DriverPayrollRowModel(
    Guid DriverId, string DriverName, int Trips,
    decimal Hours, decimal Distance, decimal Amount, string Scheme);

public sealed record FinanceReportModel(
    AnalyticsPeriodModel Period,
    FinancialSummaryModel Summary,
    IReadOnlyList<NamedValueModel> ExpensesByCategory,
    IReadOnlyList<NamedValueModel> CostByVehicle,
    IReadOnlyList<NamedValueModel> RevenueByCustomer,
    IReadOnlyList<TimeSeriesPointModel> RevenueByDay,
    IReadOnlyList<TimeSeriesPointModel> CostByDay,
    IReadOnlyList<DriverPayrollRowModel> Payroll);
