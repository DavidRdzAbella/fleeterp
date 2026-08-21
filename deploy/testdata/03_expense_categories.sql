-- =============================================================================
-- Catalogo de conceptos de gasto
-- -----------------------------------------------------------------------------
-- IsTripRelated distingue el costo directo del viaje —que entra a la utilidad
-- de cada flete— del gasto de estructura, que solo pesa en el resumen de la
-- empresa. El combustible no aparece aqui a proposito: se captura como carga
-- en fuel_logs, que es la unica fuente de litros y costo de diesel.
-- =============================================================================

INSERT INTO expense_categories ("Id", "Code", "Name", "IsTripRelated", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
('77777777-0000-4000-8000-000000000001', 'CASETAS',  'Casetas y peajes',               true,  true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000002', 'VIATICOS', 'Viaticos del operador',          true,  true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000003', 'MANIOBRA', 'Maniobras de carga y descarga',  true,  true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000004', 'REFACC',   'Refacciones y llantas',          true,  true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000005', 'MULTAS',   'Multas e infracciones',          true,  true,  '11111111-1111-4111-8111-111111111111', now() - interval '118 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000006', 'PENSION',  'Pension y resguardo de unidades',true,  true,  '11111111-1111-4111-8111-111111111111', now() - interval '100 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000007', 'ADMIN',    'Gastos administrativos',         false, true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000008', 'SEGUROS',  'Primas de seguro',               false, true,  '11111111-1111-4111-8111-111111111111', now() - interval '120 days', 'datos-de-prueba'),
('77777777-0000-4000-8000-000000000009', 'CAPACIT',  'Capacitacion de operadores',     false, true,  '11111111-1111-4111-8111-111111111111', now() - interval '60 days',  'datos-de-prueba'),
('77777777-0000-4000-8000-000000000010', 'OBSOLETO', 'Concepto retirado',              true,  false, '11111111-1111-4111-8111-111111111111', now() - interval '250 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
