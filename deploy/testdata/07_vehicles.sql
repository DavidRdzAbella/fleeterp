-- =============================================================================
-- Unidades (24)
-- -----------------------------------------------------------------------------
-- Status: 0 = disponible, 1 = en viaje, 2 = en taller, 3 = fuera de servicio.
--
-- Los estados NO son decorativos: tienen que cuadrar con el resto del juego de
-- datos o el tablero se contradice a si mismo.
--   * Las que estan en viaje (01, 02, 03 y sus cajas 13, 14, 15) son justo las
--     que llevan los viajes en ruta de 08_trips.sql.
--   * Las que estan en taller (05 y 08) son las que tienen orden abierta en
--     11_maintenance_orders.sql.
-- Si se cambia una, hay que cambiar la otra.
--
-- CurrentOdometer queda por encima del odometro de llegada del ultimo viaje de
-- cada unidad, porque el sistema no permite que retroceda.
--
-- Las de arrastre llevan odometro y tanque en cero: no se conducen.
-- =============================================================================

INSERT INTO vehicles ("Id", "EconomicNumber", "PlateNumber", "VehicleTypeId", "Brand", "Model", "Year", "Vin", "CargoCapacity", "TankCapacity", "CurrentOdometer", "Status", "InsuranceExpiry", "CirculationCardExpiry", custom_fields, "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
-- ---- Unidades motrices ------------------------------------------------------
('44444444-0000-4000-8000-000000000001', 'T-101', 'QAA-101-A', '33333333-0000-4000-8000-000000000001', 'Kenworth',     'T680',       2021, 'QA3AKJGLD1MSA0001', 30000, 700, 385000, 1, (current_date + 210), (current_date + 255), '{"gps_id": "GPS-0101", "ejes": "5"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000002', 'T-102', 'QAA-102-A', '33333333-0000-4000-8000-000000000001', 'Freightliner', 'Cascadia',   2020, 'QA3AKJGLD1MSA0002', 30000, 680, 516500, 1, (current_date + 95),  (current_date + 140), '{"gps_id": "GPS-0102", "ejes": "5"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000003', 'T-103', 'QAA-103-A', '33333333-0000-4000-8000-000000000001', 'International','LT625',      2022, 'QA3AKJGLD1MSA0003', 30000, 720, 202500, 1, (current_date + 18),  (current_date + 63),  '{"gps_id": "GPS-0103", "ejes": "5"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '118 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000004', 'T-104', 'QAA-104-A', '33333333-0000-4000-8000-000000000001', 'Kenworth',     'T880',       2023, 'QA3AKJGLD1MSA0004', 30000, 700, 148290, 0, (current_date + 320), (current_date + 365), '{"gps_id": "GPS-0104", "ejes": "5"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '117 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000005', 'T-105', 'QAA-105-A', '33333333-0000-4000-8000-000000000001', 'Volvo',        'VNL 760',    2019, 'QA3AKJGLD1MSA0005', 30000, 660, 260000, 2, (current_date + 150), (current_date + 195), '{"gps_id": "GPS-0105", "ejes": "5"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '116 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000006', 'T-106', 'QAA-106-A', '33333333-0000-4000-8000-000000000001', 'Scania',       'R450',       2022, 'QA3AKJGLD1MSA0006', 30000, 690,  97648, 0, (current_date - 5),   (current_date + 40),  '{"gps_id": "GPS-0106", "ejes": "5"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '115 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000007', 'R-201', 'QAB-201-B', '33333333-0000-4000-8000-000000000002', 'Isuzu',        'Forward',    2023, 'QA3AKJGLD1MSB0007',  8000, 200,  77405, 0, (current_date + 300), (current_date + 345), '{"ejes": "2"}',                      true,  '11111111-1111-4111-8111-111111111111', now() - interval '112 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000008', 'R-202', 'QAB-202-B', '33333333-0000-4000-8000-000000000002', 'Hino',         '500 Series', 2021, 'QA3AKJGLD1MSB0008',  8000, 210, 175000, 2, (current_date + 240), (current_date + 285), '{"ejes": "2"}',                      true,  '11111111-1111-4111-8111-111111111111', now() - interval '110 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000009', 'O-301', 'QAC-301-C', '33333333-0000-4000-8000-000000000003', 'Hino',         'FM 500',     2022, 'QA3AKJGLD1MSC0009', 14000, 300, 143019, 0, (current_date + 190), (current_date + 235), '{"ejes": "3"}',                      true,  '11111111-1111-4111-8111-111111111111', now() - interval '108 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000010', 'O-302', 'QAC-302-C', '33333333-0000-4000-8000-000000000003', 'Mercedes-Benz','Atego 1726', 2020, 'QA3AKJGLD1MSC0010', 14000, 290,  89728, 0, (current_date + 22),  (current_date + 67),  '{"ejes": "3"}',                      true,  '11111111-1111-4111-8111-111111111111', now() - interval '105 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000011', 'C-401', 'QAD-401-D', '33333333-0000-4000-8000-000000000004', 'Nissan',       'NP300',      2023, 'QA3AKJGLD1MSD0011',  1200,  80,  42000, 0, (current_date + 275), (current_date + 320), '{"ejes": "2"}',                      true,  '11111111-1111-4111-8111-111111111111', now() - interval '100 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000012', 'C-402', 'QAD-402-D', '33333333-0000-4000-8000-000000000004', 'Chevrolet',    'Silverado',  2018, 'QA3AKJGLD1MSD0012',  1200,  85,  68000, 3, (current_date - 45),  (current_date - 10),  '{}',                                 true,  '11111111-1111-4111-8111-111111111111', now() - interval '95 days',  'datos-de-prueba'),

-- ---- Unidades de arrastre ---------------------------------------------------
('44444444-0000-4000-8000-000000000013', 'K-501', 'QAE-501-E', '33333333-0000-4000-8000-000000000005', 'Utility',      '3000R',      2019, 'QA1UYVS2531P0013',  28000, NULL, 0, 1, (current_date + 230), (current_date + 275), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000014', 'K-502', 'QAE-502-E', '33333333-0000-4000-8000-000000000005', 'Great Dane',   'Champion',   2020, 'QA1UYVS2531P0014',  28000, NULL, 0, 1, (current_date + 260), (current_date + 305), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000015', 'K-503', 'QAE-503-E', '33333333-0000-4000-8000-000000000005', 'Wabash',       'DuraPlate',  2021, 'QA1UYVS2531P0015',  28000, NULL, 0, 1, (current_date + 28),  (current_date + 73),  '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '118 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000016', 'K-504', 'QAE-504-E', '33333333-0000-4000-8000-000000000005', 'Utility',      '3000R',      2022, 'QA1UYVS2531P0016',  28000, NULL, 0, 0, (current_date + 340), (current_date + 385), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '112 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000017', 'K-505', 'QAE-505-E', '33333333-0000-4000-8000-000000000005', 'Hyundai',      'Translead',  2023, 'QA1UYVS2531P0017',  28000, NULL, 0, 0, (current_date + 400), (current_date + 445), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '105 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000018', 'F-601', 'QAF-601-F', '33333333-0000-4000-8000-000000000006', 'Thermo King',  'Precedent',  2021, 'QA1UYVS2531P0018',  24000, NULL, 0, 0, (current_date + 120), (current_date + 165), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '110 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000019', 'F-602', 'QAF-602-F', '33333333-0000-4000-8000-000000000006', 'Carrier',      'Vector',     2022, 'QA1UYVS2531P0019',  24000, NULL, 0, 0, (current_date + 200), (current_date + 245), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '100 days', 'datos-de-prueba'),
('44444444-0000-4000-8000-000000000020', 'P-701', 'QAG-701-G', '33333333-0000-4000-8000-000000000007', 'Lufkin',       'Flatbed 48', 2018, 'QA1UYVS2531P0020',  26000, NULL, 0, 0, (current_date + 60),  (current_date + 105), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '98 days',  'datos-de-prueba'),
('44444444-0000-4000-8000-000000000021', 'P-702', 'QAG-702-G', '33333333-0000-4000-8000-000000000007', 'Fontaine',     'Infinity',   2021, 'QA1UYVS2531P0021',  26000, NULL, 0, 0, (current_date + 310), (current_date + 355), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '90 days',  'datos-de-prueba'),
('44444444-0000-4000-8000-000000000022', 'Z-801', 'QAH-801-H', '33333333-0000-4000-8000-000000000008', 'Tremcar',      'Pipa 30000', 2020, 'QA1UYVS2531P0022',  30000, NULL, 0, 0, (current_date + 175), (current_date + 220), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '85 days',  'datos-de-prueba'),
('44444444-0000-4000-8000-000000000023', 'D-901', 'QAI-901-I', '33333333-0000-4000-8000-000000000009', 'Silver Eagle', 'Dolly',      2019, 'QA1UYVS2531P0023',   9000, NULL, 0, 0, (current_date + 145), (current_date + 190), '{}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '80 days',  'datos-de-prueba'),

-- Dada de baja: un viaje cancelado la sigue citando, por eso se desactiva.
('44444444-0000-4000-8000-000000000024', 'D-902', 'QAI-902-I', '33333333-0000-4000-8000-000000000009', 'Silver Eagle', 'Dolly',      2015, 'QA1UYVS2531P0024',   9000, NULL, 0, 0, (current_date - 120), (current_date - 75),  '{}', false, '11111111-1111-4111-8111-111111111111', now() - interval '350 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
