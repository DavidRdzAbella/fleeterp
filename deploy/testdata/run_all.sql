-- =============================================================================
-- FleetERP · juego de datos de prueba
-- =============================================================================
-- Carga 210 registros repartidos en las doce tablas del sistema, colgados de dos
-- empresas propias ('qa' y 'qa-inactiva') para no tocar nada de lo que ya haya
-- en la base.
--
--   psql -h localhost -p 5432 -U postgres -d fleeterp -f deploy/testdata/run_all.sql
--
-- Para quitarlos:
--
--   psql -h localhost -p 5432 -U postgres -d fleeterp -f deploy/testdata/99_cleanup.sql
--
-- -----------------------------------------------------------------------------
-- Por que una empresa aparte
--
-- Los indices unicos del sistema estan acotados por TenantId, asi que estos
-- datos pueden repetir placas, folios y codigos sin chocar con la empresa de
-- demostracion. Y como todo cuelga de un TenantId, el borrado es exacto: no
-- hace falta adivinar que registro era de prueba y cual no.
--
-- Acceso al portal: empresa 'qa', cualquiera de los correos de 01_users.sql,
-- contrasena 'Prueba123$'.
--
-- -----------------------------------------------------------------------------
-- Que se puede probar con esto
--
--   * Los cuatro esquemas de pago al operador conviviendo, con la nomina ya
--     calculada como la calcularia el dominio.
--   * Puntualidad distinta de 100 %: cuatro viajes llegan tarde a proposito.
--   * Alertas del tablero: una licencia vencida, dos por vencer y dos seguros
--     proximos a expirar.
--   * Bajas logicas: clientes, operadores, unidades y conceptos desactivados
--     que siguen citados por movimientos historicos.
--   * Los cinco tipos de campo a la medida y sus valores en columnas jsonb.
--   * Rechazo de acceso a una empresa dada de baja con credenciales validas.
--
-- -----------------------------------------------------------------------------
-- Fechas relativas
--
-- Todo se calcula con now() y current_date, no con fechas fijas. El juego de
-- datos no envejece: cargado hoy o dentro de un anio, los tableros siempre
-- muestran actividad reciente y las alertas siguen disparando.
-- =============================================================================

\set ON_ERROR_STOP on

BEGIN;

-- Se limpia primero para que volver a correr el script deje siempre el mismo
-- estado, sin depender de lo que hubiera quedado de una corrida anterior.
\ir 99_cleanup.sql

\ir 00_tenants.sql
\ir 01_users.sql
\ir 02_vehicle_types.sql
\ir 03_expense_categories.sql
\ir 04_custom_field_definitions.sql
\ir 05_customers.sql
\ir 06_drivers.sql
\ir 07_vehicles.sql
\ir 08_trips.sql
\ir 09_fuel_logs.sql
\ir 10_expenses.sql
\ir 11_maintenance_orders.sql

COMMIT;

-- =============================================================================
-- Comprobaciones
-- -----------------------------------------------------------------------------
-- El juego de datos se valida a si mismo. Son invariantes que el sistema
-- garantiza en tiempo de ejecucion pero que un INSERT directo puede romper sin
-- que la base se queje, y que dejarian los tableros contradiciendose.
-- =============================================================================

DO $$
DECLARE
    qa   uuid := '11111111-1111-4111-8111-111111111111';
    mal  integer;
BEGIN
    -- El importe de una carga tiene que ser cantidad por precio.
    SELECT count(*) INTO mal FROM fuel_logs
    WHERE "TenantId" = qa AND round("Quantity" * "PricePerUnit", 2) <> "TotalCost";
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % carga(s) de combustible cuyo importe no es cantidad por precio', mal;
    END IF;

    -- El odometro de llegada nunca es menor al de salida.
    SELECT count(*) INTO mal FROM trips
    WHERE "TenantId" = qa AND "OdometerEnd" IS NOT NULL AND "OdometerEnd" < "OdometerStart";
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % viaje(s) con odometro de llegada menor al de salida', mal;
    END IF;

    -- Un viaje concluido trae salida, llegada y ambos odometros.
    SELECT count(*) INTO mal FROM trips
    WHERE "TenantId" = qa AND "Status" = 2
      AND ("ActualDepartureUtc" IS NULL OR "ActualArrivalUtc" IS NULL
           OR "OdometerStart" IS NULL OR "OdometerEnd" IS NULL);
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % viaje(s) concluido(s) sin sus datos de cierre', mal;
    END IF;

    -- La unidad de un viaje en ruta tiene que aparecer ocupada, y su operador
    -- tambien: si no, el tablero de flotilla se contradice con el de viajes.
    SELECT count(*) INTO mal FROM trips t
    JOIN vehicles v ON v."Id" = t."VehicleId"
    WHERE t."TenantId" = qa AND t."Status" = 1 AND v."Status" <> 1;
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % viaje(s) en ruta cuya unidad no figura en viaje', mal;
    END IF;

    SELECT count(*) INTO mal FROM trips t
    JOIN drivers d ON d."Id" = t."DriverId"
    WHERE t."TenantId" = qa AND t."Status" = 1 AND d."Status" <> 1;
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % viaje(s) en ruta cuyo operador no figura en viaje', mal;
    END IF;

    -- Una unidad en taller necesita una orden abierta que lo justifique, y al
    -- reves: una orden viva no puede dejar a su unidad como disponible.
    SELECT count(*) INTO mal FROM vehicles v
    WHERE v."TenantId" = qa AND v."Status" = 2
      AND NOT EXISTS (SELECT 1 FROM maintenance_orders m
                      WHERE m."VehicleId" = v."Id" AND m."Status" <> 2);
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % unidad(es) en taller sin orden abierta', mal;
    END IF;

    SELECT count(*) INTO mal FROM maintenance_orders m
    JOIN vehicles v ON v."Id" = m."VehicleId"
    WHERE m."TenantId" = qa AND m."Status" <> 2 AND v."Status" <> 2;
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % orden(es) abierta(s) cuya unidad no figura en taller', mal;
    END IF;

    -- El odometro de la unidad no puede quedar por debajo del ultimo viaje.
    SELECT count(*) INTO mal FROM vehicles v
    WHERE v."TenantId" = qa
      AND v."CurrentOdometer" < COALESCE((SELECT max(GREATEST(COALESCE(t."OdometerEnd", 0), COALESCE(t."OdometerStart", 0)))
                                          FROM trips t WHERE t."VehicleId" = v."Id"), 0);
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % unidad(es) con odometro por debajo de su ultimo viaje', mal;
    END IF;

    -- El pago al operador tiene que coincidir con su esquema.
    SELECT count(*) INTO mal FROM trips
    WHERE "TenantId" = qa AND "Status" IN (1, 2)
      AND "DriverPayAmount" <> round(
            CASE "DriverPayScheme"
                WHEN 0 THEN "DriverPayRate" * COALESCE("DriverHours", 0)
                WHEN 1 THEN "DriverPayRate" * CASE WHEN COALESCE("OdometerEnd", 0) - COALESCE("OdometerStart", 0) > 0
                                                   THEN "OdometerEnd" - "OdometerStart"
                                                   ELSE "PlannedDistance" END
                WHEN 2 THEN "DriverPayRate"
                ELSE "FreightRevenue" * "DriverPayRate" / 100
            END, 2);
    IF mal > 0 THEN
        RAISE EXCEPTION 'Hay % viaje(s) cuyo pago no corresponde a su esquema', mal;
    END IF;

    RAISE NOTICE 'Datos de prueba cargados y verificados.';
END $$;

SELECT 'tenants' AS tabla, count(*) FROM tenants WHERE "Id" IN ('11111111-1111-4111-8111-111111111111','11111111-1111-4111-8111-111111111112')
UNION ALL SELECT 'users',                    count(*) FROM users                    WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111','11111111-1111-4111-8111-111111111112')
UNION ALL SELECT 'vehicle_types',            count(*) FROM vehicle_types            WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'expense_categories',       count(*) FROM expense_categories       WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'custom_field_definitions', count(*) FROM custom_field_definitions WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'customers',                count(*) FROM customers                WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'drivers',                  count(*) FROM drivers                  WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'vehicles',                 count(*) FROM vehicles                 WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'trips',                    count(*) FROM trips                    WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'fuel_logs',                count(*) FROM fuel_logs                WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'expenses',                 count(*) FROM expenses                 WHERE "TenantId" = '11111111-1111-4111-8111-111111111111'
UNION ALL SELECT 'maintenance_orders',       count(*) FROM maintenance_orders       WHERE "TenantId" = '11111111-1111-4111-8111-111111111111';
