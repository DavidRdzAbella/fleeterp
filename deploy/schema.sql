CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE custom_field_definitions (
        "Id" uuid NOT NULL,
        "Target" integer NOT NULL,
        "Key" character varying(40) NOT NULL,
        "Label" character varying(80) NOT NULL,
        "Type" integer NOT NULL,
        "IsRequired" boolean NOT NULL,
        "Options" character varying(1000),
        "DisplayOrder" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_custom_field_definitions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE customers (
        "Id" uuid NOT NULL,
        "Name" character varying(150) NOT NULL,
        "TaxId" character varying(30),
        "ContactName" character varying(120),
        "Phone" character varying(30),
        "Email" character varying(150),
        "Address" character varying(300),
        custom_fields jsonb NOT NULL,
        "IsActive" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_customers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE drivers (
        "Id" uuid NOT NULL,
        "FirstName" character varying(60) NOT NULL,
        "LastName" character varying(60) NOT NULL,
        "EmployeeNumber" character varying(30),
        "LicenseNumber" character varying(40) NOT NULL,
        "LicenseType" character varying(30),
        "LicenseExpiry" date,
        "Phone" character varying(30),
        "Email" character varying(150),
        "HireDate" date,
        "PayScheme" integer NOT NULL,
        "PayRate" numeric(18,2) NOT NULL,
        "Status" integer NOT NULL,
        custom_fields jsonb NOT NULL,
        "IsActive" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_drivers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE expense_categories (
        "Id" uuid NOT NULL,
        "Code" character varying(20) NOT NULL,
        "Name" character varying(80) NOT NULL,
        "IsTripRelated" boolean NOT NULL,
        "IsActive" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_expense_categories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE tenants (
        "Id" uuid NOT NULL,
        "Name" character varying(150) NOT NULL,
        "Slug" character varying(60) NOT NULL,
        "TaxId" character varying(30),
        "ContactEmail" character varying(150),
        "Phone" character varying(30),
        settings jsonb NOT NULL,
        "IsActive" boolean NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_tenants" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE users (
        "Id" uuid NOT NULL,
        "Email" character varying(150) NOT NULL,
        "FullName" character varying(120) NOT NULL,
        "PasswordHash" character varying(300) NOT NULL,
        "Role" integer NOT NULL,
        "LastLoginUtc" timestamp with time zone,
        "IsActive" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE vehicle_types (
        "Id" uuid NOT NULL,
        "Code" character varying(20) NOT NULL,
        "Name" character varying(80) NOT NULL,
        "Category" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_vehicle_types" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE vehicles (
        "Id" uuid NOT NULL,
        "EconomicNumber" character varying(30) NOT NULL,
        "PlateNumber" character varying(20) NOT NULL,
        "VehicleTypeId" uuid NOT NULL,
        "Brand" character varying(60),
        "Model" character varying(60),
        "Year" integer,
        "Vin" character varying(40),
        "CargoCapacity" numeric(18,3),
        "TankCapacity" numeric(18,3),
        "CurrentOdometer" numeric(18,2) NOT NULL,
        "Status" integer NOT NULL,
        "InsuranceExpiry" date,
        "CirculationCardExpiry" date,
        custom_fields jsonb NOT NULL,
        "IsActive" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_vehicles" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_vehicles_vehicle_types_VehicleTypeId" FOREIGN KEY ("VehicleTypeId") REFERENCES vehicle_types ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE maintenance_orders (
        "Id" uuid NOT NULL,
        "Folio" character varying(30) NOT NULL,
        "VehicleId" uuid NOT NULL,
        "Kind" integer NOT NULL,
        "Status" integer NOT NULL,
        "OpenedAtUtc" timestamp with time zone NOT NULL,
        "ClosedAtUtc" timestamp with time zone,
        "Description" character varying(500) NOT NULL,
        "Workshop" character varying(150),
        "Cost" numeric(18,2) NOT NULL,
        "OdometerAtService" numeric(18,2),
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_maintenance_orders" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_maintenance_orders_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES vehicles ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE trips (
        "Id" uuid NOT NULL,
        "Folio" character varying(30) NOT NULL,
        "DriverId" uuid NOT NULL,
        "VehicleId" uuid NOT NULL,
        "TrailerId" uuid,
        "CustomerId" uuid,
        "Origin" character varying(150) NOT NULL,
        "Destination" character varying(150) NOT NULL,
        "PlannedDistance" numeric(18,2) NOT NULL,
        "ScheduledDepartureUtc" timestamp with time zone NOT NULL,
        "ScheduledArrivalUtc" timestamp with time zone,
        "ActualDepartureUtc" timestamp with time zone,
        "ActualArrivalUtc" timestamp with time zone,
        "OdometerStart" numeric(18,2),
        "OdometerEnd" numeric(18,2),
        "InitialFuel" numeric(18,3) NOT NULL,
        "FinalFuel" numeric(18,3),
        "RefuelPlanned" boolean NOT NULL,
        "CargoWeight" numeric(18,3) NOT NULL,
        "CargoWeightUnit" integer NOT NULL,
        "CargoDescription" character varying(300),
        "FreightRevenue" numeric(18,2) NOT NULL,
        "DriverPayScheme" integer NOT NULL,
        "DriverPayRate" numeric(18,2) NOT NULL,
        "DriverHours" numeric(10,2),
        "DriverPayAmount" numeric(18,2) NOT NULL,
        "Status" integer NOT NULL,
        "Notes" character varying(1000),
        "CancellationReason" character varying(300),
        custom_fields jsonb NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_trips" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_trips_customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES customers ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_trips_drivers_DriverId" FOREIGN KEY ("DriverId") REFERENCES drivers ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_trips_vehicles_TrailerId" FOREIGN KEY ("TrailerId") REFERENCES vehicles ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_trips_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES vehicles ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE expenses (
        "Id" uuid NOT NULL,
        "CategoryId" uuid NOT NULL,
        "TripId" uuid,
        "VehicleId" uuid,
        "DriverId" uuid,
        "IncurredAtUtc" timestamp with time zone NOT NULL,
        "Amount" numeric(18,2) NOT NULL,
        "Description" character varying(250) NOT NULL,
        "ReferenceNumber" character varying(60),
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_expenses" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_expenses_drivers_DriverId" FOREIGN KEY ("DriverId") REFERENCES drivers ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_expenses_expense_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES expense_categories ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_expenses_trips_TripId" FOREIGN KEY ("TripId") REFERENCES trips ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_expenses_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES vehicles ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE TABLE fuel_logs (
        "Id" uuid NOT NULL,
        "VehicleId" uuid NOT NULL,
        "TripId" uuid,
        "DriverId" uuid,
        "LoadedAtUtc" timestamp with time zone NOT NULL,
        "Quantity" numeric(18,3) NOT NULL,
        "PricePerUnit" numeric(18,4) NOT NULL,
        "TotalCost" numeric(18,2) NOT NULL,
        "OdometerReading" numeric(18,2),
        "Station" character varying(120),
        "ReferenceNumber" character varying(60),
        "TenantId" uuid NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "CreatedBy" text,
        "UpdatedAtUtc" timestamp with time zone,
        "UpdatedBy" text,
        CONSTRAINT "PK_fuel_logs" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_fuel_logs_drivers_DriverId" FOREIGN KEY ("DriverId") REFERENCES drivers ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_fuel_logs_trips_TripId" FOREIGN KEY ("TripId") REFERENCES trips ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_fuel_logs_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES vehicles ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_custom_field_definitions_TenantId_Target_Key" ON custom_field_definitions ("TenantId", "Target", "Key");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_customers_TenantId_Name" ON customers ("TenantId", "Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_drivers_TenantId_LicenseNumber" ON drivers ("TenantId", "LicenseNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_drivers_TenantId_Status" ON drivers ("TenantId", "Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_expense_categories_TenantId_Code" ON expense_categories ("TenantId", "Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_expenses_CategoryId" ON expenses ("CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_expenses_DriverId" ON expenses ("DriverId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_expenses_TenantId_CategoryId" ON expenses ("TenantId", "CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_expenses_TenantId_IncurredAtUtc" ON expenses ("TenantId", "IncurredAtUtc");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_expenses_TripId" ON expenses ("TripId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_expenses_VehicleId" ON expenses ("VehicleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_fuel_logs_DriverId" ON fuel_logs ("DriverId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_fuel_logs_TenantId_LoadedAtUtc" ON fuel_logs ("TenantId", "LoadedAtUtc");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_fuel_logs_TenantId_VehicleId" ON fuel_logs ("TenantId", "VehicleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_fuel_logs_TripId" ON fuel_logs ("TripId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_fuel_logs_VehicleId" ON fuel_logs ("VehicleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_maintenance_orders_TenantId_Folio" ON maintenance_orders ("TenantId", "Folio");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_maintenance_orders_TenantId_Status" ON maintenance_orders ("TenantId", "Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_maintenance_orders_VehicleId" ON maintenance_orders ("VehicleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_tenants_Slug" ON tenants ("Slug");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_trips_CustomerId" ON trips ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_trips_DriverId" ON trips ("DriverId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_trips_TenantId_DriverId" ON trips ("TenantId", "DriverId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_trips_TenantId_Folio" ON trips ("TenantId", "Folio");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_trips_TenantId_ScheduledDepartureUtc" ON trips ("TenantId", "ScheduledDepartureUtc");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_trips_TenantId_Status" ON trips ("TenantId", "Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_trips_TrailerId" ON trips ("TrailerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_trips_VehicleId" ON trips ("VehicleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_users_TenantId_Email" ON users ("TenantId", "Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_vehicle_types_TenantId_Code" ON vehicle_types ("TenantId", "Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_vehicles_TenantId_EconomicNumber" ON vehicles ("TenantId", "EconomicNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_vehicles_TenantId_PlateNumber" ON vehicles ("TenantId", "PlateNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_vehicles_TenantId_Status" ON vehicles ("TenantId", "Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    CREATE INDEX "IX_vehicles_VehicleTypeId" ON vehicles ("VehicleTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806081455_InitialSchema') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260806081455_InitialSchema', '9.0.0');
    END IF;
END $EF$;
COMMIT;

