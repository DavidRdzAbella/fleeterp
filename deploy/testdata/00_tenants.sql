-- =============================================================================
-- Empresas de prueba
-- -----------------------------------------------------------------------------
-- Todo el juego de datos cuelga de una empresa propia (slug 'qa') en lugar de
-- mezclarse con la de demostracion. Los indices unicos del sistema estan
-- acotados por TenantId, asi que una empresa aparte puede repetir placas,
-- folios y codigos sin chocar con nada, y basta un DELETE por TenantId para
-- dejar la base como estaba.
--
-- La segunda empresa se inserta inactiva y sin datos: sirve para comprobar que
-- el acceso rechaza a una empresa dada de baja aunque las credenciales existan.
-- =============================================================================

INSERT INTO tenants ("Id", "Name", "Slug", "TaxId", "ContactEmail", "Phone", settings, "IsActive", "CreatedAtUtc", "CreatedBy")
VALUES
('11111111-1111-4111-8111-111111111111', 'Autotransportes de Prueba', 'qa', 'ATP240101QA1',
 'operaciones@qa.mx', '81 8000 9000',
 '{"locale": "es-MX", "logoUrl": null, "timeZoneId": "America/Mexico_City", "volumeUnit": 0,
   "weightUnit": 0, "currencyCode": "MXN", "distanceUnit": 0, "currencySymbol": "$",
   "tripFolioPrefix": "QA", "brandPrimaryColor": "#1F5FA8", "defaultDriverPayRate": 105,
   "defaultDriverPayScheme": 0, "licenseExpiryAlertDays": 30, "defaultFuelPricePerUnit": 26.40,
   "minAcceptableFuelEfficiency": 2.2}'::jsonb,
 true, now() - interval '120 days', 'datos-de-prueba'),

('11111111-1111-4111-8111-111111111112', 'Fletes Suspendidos', 'qa-inactiva', 'FSU240101QA2',
 'contacto@qa-inactiva.mx', '81 8000 9001',
 '{"locale": "es-MX", "logoUrl": null, "timeZoneId": "America/Mexico_City", "volumeUnit": 0,
   "weightUnit": 0, "currencyCode": "MXN", "distanceUnit": 0, "currencySymbol": "$",
   "tripFolioPrefix": "FS", "brandPrimaryColor": "#0E7C66", "defaultDriverPayRate": 90,
   "defaultDriverPayScheme": 0, "licenseExpiryAlertDays": 30, "defaultFuelPricePerUnit": 25.90,
   "minAcceptableFuelEfficiency": 2.0}'::jsonb,
 false, now() - interval '400 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
