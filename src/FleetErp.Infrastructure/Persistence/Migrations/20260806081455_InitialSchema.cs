using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "custom_field_definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Label = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Options = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_field_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TaxId = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContactName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "drivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    EmployeeNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    LicenseNumber = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LicenseType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    LicenseExpiry = table.Column<DateOnly>(type: "date", nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PayScheme = table.Column<int>(type: "integer", nullable: false),
                    PayRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "expense_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    IsTripRelated = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    TaxId = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    settings = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FullName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    LastLoginUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EconomicNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PlateNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VehicleTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Brand = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Model = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Vin = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CargoCapacity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    TankCapacity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    CurrentOdometer = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    InsuranceExpiry = table.Column<DateOnly>(type: "date", nullable: true),
                    CirculationCardExpiry = table.Column<DateOnly>(type: "date", nullable: true),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicles_vehicle_types_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "vehicle_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Folio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OpenedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClosedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Workshop = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OdometerAtService = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenance_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_maintenance_orders_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Folio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrailerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Origin = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Destination = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PlannedDistance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ScheduledDepartureUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ScheduledArrivalUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualDepartureUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualArrivalUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OdometerStart = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    OdometerEnd = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    InitialFuel = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    FinalFuel = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    RefuelPlanned = table.Column<bool>(type: "boolean", nullable: false),
                    CargoWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    CargoWeightUnit = table.Column<int>(type: "integer", nullable: false),
                    CargoDescription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FreightRevenue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DriverPayScheme = table.Column<int>(type: "integer", nullable: false),
                    DriverPayRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DriverHours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    DriverPayAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trips_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trips_drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trips_vehicles_TrailerId",
                        column: x => x.TrailerId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trips_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_expenses_drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_expenses_expense_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "expense_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_expenses_trips_TripId",
                        column: x => x.TripId,
                        principalTable: "trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_expenses_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fuel_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: true),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: true),
                    LoadedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OdometerReading = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Station = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fuel_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fuel_logs_drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_logs_trips_TripId",
                        column: x => x.TripId,
                        principalTable: "trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_fuel_logs_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_custom_field_definitions_TenantId_Target_Key",
                table: "custom_field_definitions",
                columns: new[] { "TenantId", "Target", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_TenantId_Name",
                table: "customers",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_drivers_TenantId_LicenseNumber",
                table: "drivers",
                columns: new[] { "TenantId", "LicenseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_drivers_TenantId_Status",
                table: "drivers",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_expense_categories_TenantId_Code",
                table: "expense_categories",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_expenses_CategoryId",
                table: "expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_DriverId",
                table: "expenses",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_TenantId_CategoryId",
                table: "expenses",
                columns: new[] { "TenantId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_expenses_TenantId_IncurredAtUtc",
                table: "expenses",
                columns: new[] { "TenantId", "IncurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_expenses_TripId",
                table: "expenses",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_VehicleId",
                table: "expenses",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_logs_DriverId",
                table: "fuel_logs",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_logs_TenantId_LoadedAtUtc",
                table: "fuel_logs",
                columns: new[] { "TenantId", "LoadedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_fuel_logs_TenantId_VehicleId",
                table: "fuel_logs",
                columns: new[] { "TenantId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_fuel_logs_TripId",
                table: "fuel_logs",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_logs_VehicleId",
                table: "fuel_logs",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_orders_TenantId_Folio",
                table: "maintenance_orders",
                columns: new[] { "TenantId", "Folio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_orders_TenantId_Status",
                table: "maintenance_orders",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_orders_VehicleId",
                table: "maintenance_orders",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Slug",
                table: "tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_CustomerId",
                table: "trips",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_trips_DriverId",
                table: "trips",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_trips_TenantId_DriverId",
                table: "trips",
                columns: new[] { "TenantId", "DriverId" });

            migrationBuilder.CreateIndex(
                name: "IX_trips_TenantId_Folio",
                table: "trips",
                columns: new[] { "TenantId", "Folio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_TenantId_ScheduledDepartureUtc",
                table: "trips",
                columns: new[] { "TenantId", "ScheduledDepartureUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_trips_TenantId_Status",
                table: "trips",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_trips_TrailerId",
                table: "trips",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_trips_VehicleId",
                table: "trips",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_Email",
                table: "users",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_types_TenantId_Code",
                table: "vehicle_types",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_TenantId_EconomicNumber",
                table: "vehicles",
                columns: new[] { "TenantId", "EconomicNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_TenantId_PlateNumber",
                table: "vehicles",
                columns: new[] { "TenantId", "PlateNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_TenantId_Status",
                table: "vehicles",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_VehicleTypeId",
                table: "vehicles",
                column: "VehicleTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "custom_field_definitions");

            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.DropTable(
                name: "fuel_logs");

            migrationBuilder.DropTable(
                name: "maintenance_orders");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "expense_categories");

            migrationBuilder.DropTable(
                name: "trips");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "drivers");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "vehicle_types");
        }
    }
}
