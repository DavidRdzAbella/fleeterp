-- =============================================================================
-- Borrado de los datos de prueba
-- -----------------------------------------------------------------------------
-- Deja la base como estaba antes de cargar el juego de pruebas. Borra solo por
-- TenantId, asi que la empresa de demostracion y cualquier dato real quedan
-- intactos: es la ventaja de haber metido todo bajo empresas propias.
--
-- El orden es el inverso al de carga, porque las llaves foraneas son RESTRICT:
-- no se puede borrar una unidad mientras un viaje la cite.
--
-- No abre transaccion propia a proposito: run_all.sql lo incluye dentro de la
-- suya, y un BEGIN anidado seria ignorado con una advertencia. Para correrlo
-- solo y de forma atomica, psql lo permite con la opcion -1:
--   psql ... -1 -f deploy/testdata/99_cleanup.sql
-- =============================================================================

DELETE FROM expenses                 WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM fuel_logs                WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM maintenance_orders       WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM trips                    WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM vehicles                 WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM drivers                  WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM customers                WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM vehicle_types            WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM expense_categories       WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM custom_field_definitions WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM users                    WHERE "TenantId" IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
DELETE FROM tenants                  WHERE "Id"       IN ('11111111-1111-4111-8111-111111111111', '11111111-1111-4111-8111-111111111112');
