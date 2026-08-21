-- =============================================================================
-- Catalogo de tipos de unidad
-- -----------------------------------------------------------------------------
-- Category: 0 = motriz (se conduce), 1 = de arrastre (se engancha). Es lo que
-- decide que unidades pueden ir como remolque en un viaje, asi que el catalogo
-- incluye ejemplos de las dos y uno inactivo para probar que desaparece de los
-- combos sin borrarse.
-- =============================================================================

INSERT INTO vehicle_types ("Id", "Code", "Name", "Category", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
('33333333-0000-4000-8000-000000000001', 'TRACTO',   'Tractocamion',               0, true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000002', 'RABON',    'Rabon 8 toneladas',          0, true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000003', 'TORTON',   'Torton 14 toneladas',        0, true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000004', 'CAMIONETA','Camioneta de reparto',       0, true,  '11111111-1111-4111-8111-111111111111', now() - interval '110 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000005', 'CAJA53',   'Caja seca 53 pies',          1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000006', 'REFRI',    'Caja refrigerada 48 pies',   1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000007', 'PLATAF',   'Plataforma',                 1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '115 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000008', 'PIPA',     'Pipa de combustible',        1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '100 days', 'datos-de-prueba'),
('33333333-0000-4000-8000-000000000009', 'DOLLY',    'Dolly convertidor',          1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '90 days',  'datos-de-prueba'),
('33333333-0000-4000-8000-000000000010', 'GRUA',     'Grua de arrastre (retirada)',0, false, '11111111-1111-4111-8111-111111111111', now() - interval '300 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
