-- =============================================================================
-- Usuarios de acceso al portal
-- -----------------------------------------------------------------------------
-- Los 8 comparten la contrasena 'Prueba123$'. El hash es PBKDF2-SHA256 con
-- 120 000 iteraciones, el mismo formato que produce Pbkdf2PasswordHasher, de
-- modo que estas cuentas entran de verdad al portal y no solo llenan la tabla.
--
-- La mezcla cubre los tres perfiles y los dos estados: administrador,
-- despachador y consulta, activos e inactivos, con y sin ultimo acceso
-- registrado, para poder probar permisos y el filtro de cuentas dadas de baja.
-- =============================================================================

INSERT INTO users ("Id", "Email", "FullName", "PasswordHash", "Role", "LastLoginUtc", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
('22222222-0000-4000-8000-000000000001', 'admin@qa.mx',      'Beatriz Ordonez',   'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 0, now() - interval '2 hours',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('22222222-0000-4000-8000-000000000002', 'director@qa.mx',   'Ramon Villalobos',  'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 0, now() - interval '3 days',   true,  '11111111-1111-4111-8111-111111111111', now() - interval '118 days', 'datos-de-prueba'),
('22222222-0000-4000-8000-000000000003', 'despacho1@qa.mx',  'Karla Fuentes',     'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 1, now() - interval '40 minutes', true, '11111111-1111-4111-8111-111111111111', now() - interval '110 days', 'datos-de-prueba'),
('22222222-0000-4000-8000-000000000004', 'despacho2@qa.mx',  'Hector Delgado',    'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 1, now() - interval '1 day',    true,  '11111111-1111-4111-8111-111111111111', now() - interval '95 days',  'datos-de-prueba'),
('22222222-0000-4000-8000-000000000005', 'despacho3@qa.mx',  'Norma Estrada',     'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 1, NULL,                        true,  '11111111-1111-4111-8111-111111111111', now() - interval '5 days',   'datos-de-prueba'),
('22222222-0000-4000-8000-000000000006', 'consulta1@qa.mx',  'Sergio Amaya',      'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 2, now() - interval '9 days',   true,  '11111111-1111-4111-8111-111111111111', now() - interval '80 days',  'datos-de-prueba'),
('22222222-0000-4000-8000-000000000007', 'consulta2@qa.mx',  'Leticia Barron',    'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 2, NULL,                        true,  '11111111-1111-4111-8111-111111111111', now() - interval '12 days',  'datos-de-prueba'),
('22222222-0000-4000-8000-000000000008', 'exempleado@qa.mx', 'Joaquin Trevino',   'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 1, now() - interval '210 days', false, '11111111-1111-4111-8111-111111111111', now() - interval '300 days', 'datos-de-prueba'),

-- Cuenta valida dentro de la empresa dada de baja: el acceso debe rechazarla.
('22222222-0000-4000-8000-000000000009', 'admin@qa-inactiva.mx', 'Olivia Cantu',  'pbkdf2.120000.LlYbWbFh85cZbmt8rbmB3g==.F6bdctJuxV+dguku2E0EsDzEgCyMk9izGu1tNQHxFOA=', 0, NULL,                        true,  '11111111-1111-4111-8111-111111111112', now() - interval '400 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
