-- =============================================================================
-- Conductores (20)
-- -----------------------------------------------------------------------------
-- PayScheme: 0 = por hora, 1 = por distancia, 2 = fijo por viaje,
--            3 = porcentaje del flete. PayRate se interpreta segun el esquema,
--            asi que 3.20 en el esquema 1 son pesos por kilometro y 12 en el
--            esquema 3 son doce por ciento del flete.
-- Status:    0 = disponible, 1 = en viaje, 2 = permiso, 3 = baja.
--
-- Los cuatro esquemas conviven a proposito, que es como ocurre en la practica y
-- lo que ejercita el calculo de nomina. LicenseExpiry incluye una ya vencida y
-- dos por vencer dentro del umbral de 30 dias, para que el tablero levante sus
-- alertas sin tener que esperar a que pase el tiempo.
--
-- Los que aparecen en viaje (Status = 1) son los mismos que llevan los viajes
-- en ruta de 08_trips.sql; si se cambia uno hay que cambiar el otro.
-- =============================================================================

INSERT INTO drivers ("Id", "FirstName", "LastName", "EmployeeNumber", "LicenseNumber", "LicenseType", "LicenseExpiry", "Phone", "Email", "HireDate", "PayScheme", "PayRate", "Status", custom_fields, "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
-- En viaje: amarrados a los viajes en ruta.
('55555555-0000-4000-8000-000000000001', 'Ulises',   'Mendoza',   'OP-001', 'QA-LIC-000001', 'Federal Tipo E', (current_date + 400), '81 1500 0001', 'ulises@qa.mx',   (current_date - 1200), 0, 110.00, 1, '{"tipo_sangre": "O+"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000002', 'Rogelio',  'Salinas',   'OP-002', 'QA-LIC-000002', 'Federal Tipo E', (current_date + 260), '81 1500 0002', 'rogelio@qa.mx',  (current_date - 900),  1, 3.20,   1, '{"tipo_sangre": "A+"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000003', 'Marisol',  'Aguilar',   'OP-003', 'QA-LIC-000003', 'Federal Tipo E', (current_date + 75),  '81 1500 0003', 'marisol@qa.mx',  (current_date - 700),  2, 2800.00, 1, '{"tipo_sangre": "B+"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '118 days', 'datos-de-prueba'),

-- Licencias que disparan alerta: una vencida y dos dentro del umbral de aviso.
('55555555-0000-4000-8000-000000000004', 'Juanito',  'Perez',     'OP-004', 'QA-LIC-000004', 'Federal Tipo E', (current_date + 12),  '81 1500 0004', 'juanito@qa.mx',  (current_date - 640),  0, 95.00,  0, '{"tipo_sangre": "O-"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '117 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000005', 'Efrain',   'Gonzalez',  'OP-005', 'QA-LIC-000005', 'Federal Tipo E', (current_date + 25),  '81 1500 0005', 'efrain@qa.mx',   (current_date - 610),  3, 12.00,  0, '{"tipo_sangre": "AB+"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '116 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000006', 'Bernardo', 'Quintana',  'OP-006', 'QA-LIC-000006', 'Federal Tipo E', (current_date - 8),   '81 1500 0006', 'bernardo@qa.mx', (current_date - 580),  0, 100.00, 0, '{"tipo_sangre": "A-"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '115 days', 'datos-de-prueba'),

-- Plantilla disponible.
('55555555-0000-4000-8000-000000000007', 'Carmen',   'Ibarra',    'OP-007', 'QA-LIC-000007', 'Federal Tipo E', (current_date + 540), '81 1500 0007', 'carmen@qa.mx',   (current_date - 540),  0, 105.00, 0, '{"tipo_sangre": "B-"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '112 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000008', 'Ismael',   'Carranza',  'OP-008', 'QA-LIC-000008', 'Federal Tipo E', (current_date + 380), '81 1500 0008', 'ismael@qa.mx',   (current_date - 500),  1, 3.05,   0, '{"tipo_sangre": "O+"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '110 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000009', 'Teresa',   'Valadez',   'OP-009', 'QA-LIC-000009', 'Federal Tipo C', (current_date + 300), '81 1500 0009', 'teresa@qa.mx',   (current_date - 470),  0, 98.00,  0, '{"tipo_sangre": "A+"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '108 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000010', 'Fidel',    'Rangel',    'OP-010', 'QA-LIC-000010', 'Federal Tipo E', (current_date + 210), '81 1500 0010', 'fidel@qa.mx',    (current_date - 430),  2, 3100.00, 0, '{"tipo_sangre": "O+"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '105 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000011', 'Adriana',  'Nunez',     'OP-011', 'QA-LIC-000011', 'Federal Tipo E', (current_date + 620), '81 1500 0011', 'adriana@qa.mx',  (current_date - 400),  0, 112.00, 0, '{"tipo_sangre": "AB-"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '100 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000012', 'Gonzalo',  'Herrera',   'OP-012', 'QA-LIC-000012', 'Federal Tipo E', (current_date + 190), '81 1500 0012', 'gonzalo@qa.mx',  (current_date - 360),  3, 10.50,  0, '{"tipo_sangre": "B+"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '95 days',  'datos-de-prueba'),
('55555555-0000-4000-8000-000000000013', 'Patricia', 'Moreno',    'OP-013', 'QA-LIC-000013', 'Federal Tipo C', (current_date + 480), '81 1500 0013', 'patricia@qa.mx', (current_date - 330),  0, 96.00,  0, '{}',                     true,  '11111111-1111-4111-8111-111111111111', now() - interval '90 days',  'datos-de-prueba'),
('55555555-0000-4000-8000-000000000014', 'Raul',     'Betancourt','OP-014', 'QA-LIC-000014', 'Federal Tipo E', (current_date + 350), '81 1500 0014', 'raul@qa.mx',     (current_date - 300),  1, 2.95,   0, '{"tipo_sangre": "O-"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '85 days',  'datos-de-prueba'),
('55555555-0000-4000-8000-000000000015', 'Lourdes',  'Sepulveda', 'OP-015', 'QA-LIC-000015', 'Federal Tipo E', (current_date + 270), '81 1500 0015', 'lourdes@qa.mx',  (current_date - 250),  0, 101.00, 0, '{"tipo_sangre": "A+"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '78 days',  'datos-de-prueba'),

-- Permiso o incapacidad: no deben aparecer al programar un viaje.
('55555555-0000-4000-8000-000000000016', 'Alfredo',  'Zavala',    'OP-016', 'QA-LIC-000016', 'Federal Tipo E', (current_date + 420), '81 1500 0016', 'alfredo@qa.mx',  (current_date - 220),  0, 99.00,  2, '{"tipo_sangre": "B+"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '70 days',  'datos-de-prueba'),
('55555555-0000-4000-8000-000000000017', 'Rosalba',  'Espinoza',  'OP-017', 'QA-LIC-000017', 'Federal Tipo C', (current_date + 160), '81 1500 0017', 'rosalba@qa.mx',  (current_date - 180),  0, 94.00,  2, '{}',                     true,  '11111111-1111-4111-8111-111111111111', now() - interval '60 days',  'datos-de-prueba'),

-- Alta reciente, aun sin viajes ni datos opcionales capturados.
('55555555-0000-4000-8000-000000000018', 'Emiliano', 'Cardenas',  NULL,     'QA-LIC-000018', NULL,             NULL,                 NULL,           NULL,             NULL,                  0, 105.00, 0, '{}',                     true,  '11111111-1111-4111-8111-111111111111', now() - interval '6 days',   'datos-de-prueba'),

-- Dados de baja: los viajes historicos los siguen citando.
('55555555-0000-4000-8000-000000000019', 'Salvador', 'Tapia',     'OP-019', 'QA-LIC-000019', 'Federal Tipo E', (current_date - 90),  '81 1500 0019', NULL,             (current_date - 1500), 0, 88.00,  3, '{}',                     false, '11111111-1111-4111-8111-111111111111', now() - interval '400 days', 'datos-de-prueba'),
('55555555-0000-4000-8000-000000000020', 'Yolanda',  'Prieto',    'OP-020', 'QA-LIC-000020', 'Federal Tipo C', (current_date - 200), '81 1500 0020', NULL,             (current_date - 1600), 1, 2.60,   3, '{}',                     false, '11111111-1111-4111-8111-111111111111', now() - interval '420 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
