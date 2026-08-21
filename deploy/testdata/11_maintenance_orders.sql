-- =============================================================================
-- Ordenes de taller (20)
-- -----------------------------------------------------------------------------
-- Kind:   0 = preventivo, 1 = correctivo.
-- Status: 0 = abierta, 1 = en proceso, 2 = cerrada.
--
-- Las dos ordenes que NO estan cerradas son las que justifican que T-105 y
-- R-202 aparezcan en taller en 07_vehicles.sql. Es la misma regla que aplica el
-- sistema: una unidad vuelve a estar disponible cuando ya no le queda ninguna
-- orden abierta, asi que dejar aqui una orden viva y la unidad en disponible
-- haria que el tablero se contradijera.
--
-- Una orden abierta no tiene fecha de cierre ni costo: el gasto se conoce al
-- cerrarla. Por eso las dos vivas van con ClosedAtUtc en NULL y Cost en cero.
--
-- El folio usa el anio en curso por la misma razon que los viajes: el
-- consecutivo del sistema busca 'OS-<anio>-' y continua desde el mayor.
-- =============================================================================

INSERT INTO maintenance_orders ("Id", "Folio", "VehicleId", "Kind", "Status", "OpenedAtUtc", "ClosedAtUtc", "Description", "Workshop", "Cost", "OdometerAtService", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
-- ---- Vivas: mantienen a su unidad en taller ---------------------------------
('cccccccc-0000-4000-8000-000000000001', 'OS-' || to_char(now(),'YYYY') || '-000001', '44444444-0000-4000-8000-000000000005', 1, 0, now() - interval '4 days', NULL, 'Fuga en el sistema hidraulico de frenos traseros.',           NULL, 0.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '4 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000002', 'OS-' || to_char(now(),'YYYY') || '-000002', '44444444-0000-4000-8000-000000000008', 1, 1, now() - interval '9 days', NULL, 'Ruido en la suspension trasera; se desarmo para diagnostico.', 'Taller Diesel Norte', 0.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '9 days', 'datos-de-prueba'),

-- ---- Cerradas ---------------------------------------------------------------
('cccccccc-0000-4000-8000-000000000003', 'OS-' || to_char(now(),'YYYY') || '-000003', '44444444-0000-4000-8000-000000000001', 0, 2, now() - interval '95 days', now() - interval '93 days', 'Servicio de 380 mil kilometros: aceite, filtros y banda.', 'Taller Diesel Norte',      18400.00, 379200, '11111111-1111-4111-8111-111111111111', now() - interval '95 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000004', 'OS-' || to_char(now(),'YYYY') || '-000004', '44444444-0000-4000-8000-000000000002', 0, 2, now() - interval '88 days', now() - interval '86 days', 'Servicio mayor de 510 mil kilometros.',                    'Taller Diesel Norte',      31500.00, 511000, '11111111-1111-4111-8111-111111111111', now() - interval '88 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000005', 'OS-' || to_char(now(),'YYYY') || '-000005', '44444444-0000-4000-8000-000000000003', 1, 2, now() - interval '80 days', now() - interval '79 days', 'Cambio de alternador.',                                    'Electrico Regio',           9800.00, 197400, '11111111-1111-4111-8111-111111111111', now() - interval '80 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000006', 'OS-' || to_char(now(),'YYYY') || '-000006', '44444444-0000-4000-8000-000000000013', 0, 2, now() - interval '76 days', now() - interval '75 days', 'Revision de suspension y luces de la caja.',                'Taller de Remolques Apodaca', 4200.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '76 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000007', 'OS-' || to_char(now(),'YYYY') || '-000007', '44444444-0000-4000-8000-000000000004', 0, 2, now() - interval '70 days', now() - interval '69 days', 'Servicio preventivo de 140 mil kilometros.',               'Taller Diesel Norte',      14900.00, 143800, '11111111-1111-4111-8111-111111111111', now() - interval '70 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000008', 'OS-' || to_char(now(),'YYYY') || '-000008', '44444444-0000-4000-8000-000000000007', 1, 2, now() - interval '66 days', now() - interval '64 days', 'Reparacion de caja de velocidades.',                       'Transmisiones del Norte',  26700.00,  75200, '11111111-1111-4111-8111-111111111111', now() - interval '66 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000009', 'OS-' || to_char(now(),'YYYY') || '-000009', '44444444-0000-4000-8000-000000000009', 0, 2, now() - interval '60 days', now() - interval '59 days', 'Servicio preventivo y alineacion.',                        'Taller Diesel Norte',      11200.00, 140100, '11111111-1111-4111-8111-111111111111', now() - interval '60 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000010', 'OS-' || to_char(now(),'YYYY') || '-000010', '44444444-0000-4000-8000-000000000014', 0, 2, now() - interval '55 days', now() - interval '54 days', 'Cambio de llantas del eje trasero de la caja.',            'Llantera Industrial',      16800.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '55 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000011', 'OS-' || to_char(now(),'YYYY') || '-000011', '44444444-0000-4000-8000-000000000010', 1, 2, now() - interval '50 days', now() - interval '48 days', 'Reparacion de sistema electrico de arranque.',             'Electrico Regio',           7300.00,  87200, '11111111-1111-4111-8111-111111111111', now() - interval '50 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000012', 'OS-' || to_char(now(),'YYYY') || '-000012', '44444444-0000-4000-8000-000000000018', 0, 2, now() - interval '46 days', now() - interval '45 days', 'Mantenimiento del equipo de refrigeracion.',               'Servicio Termico MTY',     22400.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '46 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000013', 'OS-' || to_char(now(),'YYYY') || '-000013', '44444444-0000-4000-8000-000000000006', 0, 2, now() - interval '42 days', now() - interval '41 days', 'Servicio preventivo de 95 mil kilometros.',                'Taller Diesel Norte',      13100.00,  95400, '11111111-1111-4111-8111-111111111111', now() - interval '42 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000014', 'OS-' || to_char(now(),'YYYY') || '-000014', '44444444-0000-4000-8000-000000000015', 1, 2, now() - interval '38 days', now() - interval '37 days', 'Reparacion de puerta trasera de la caja.',                 'Taller de Remolques Apodaca', 5600.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '38 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000015', 'OS-' || to_char(now(),'YYYY') || '-000015', '44444444-0000-4000-8000-000000000011', 0, 2, now() - interval '33 days', now() - interval '33 days', 'Afinacion mayor.',                                         'Servicio Automotriz Sur',   4800.00,  41200, '11111111-1111-4111-8111-111111111111', now() - interval '33 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000016', 'OS-' || to_char(now(),'YYYY') || '-000016', '44444444-0000-4000-8000-000000000020', 0, 2, now() - interval '28 days', now() - interval '27 days', 'Revision de amarres y piso de la plataforma.',             'Taller de Remolques Apodaca', 3900.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '28 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000017', 'OS-' || to_char(now(),'YYYY') || '-000017', '44444444-0000-4000-8000-000000000012', 1, 2, now() - interval '24 days', now() - interval '20 days', 'Diagnostico de motor; se dictamino fuera de servicio.',    'Servicio Automotriz Sur',  12600.00,  68000, '11111111-1111-4111-8111-111111111111', now() - interval '24 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000018', 'OS-' || to_char(now(),'YYYY') || '-000018', '44444444-0000-4000-8000-000000000022', 0, 2, now() - interval '19 days', now() - interval '18 days', 'Prueba hidrostatica y valvulas de la pipa.',               'Servicios Industriales MX',28900.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '19 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000019', 'OS-' || to_char(now(),'YYYY') || '-000019', '44444444-0000-4000-8000-000000000016', 0, 2, now() - interval '13 days', now() - interval '12 days', 'Engrasado y revision de quinta rueda.',                    'Taller de Remolques Apodaca', 2100.00, NULL, '11111111-1111-4111-8111-111111111111', now() - interval '13 days', 'datos-de-prueba'),
('cccccccc-0000-4000-8000-000000000020', 'OS-' || to_char(now(),'YYYY') || '-000020', '44444444-0000-4000-8000-000000000001', 1, 2, now() - interval '7 days',  now() - interval '6 days',  'Cambio de faro y espejo lateral.',                        'Servicio Automotriz Sur',   3400.00, 384500, '11111111-1111-4111-8111-111111111111', now() - interval '7 days',  'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
