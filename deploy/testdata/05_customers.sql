-- =============================================================================
-- Clientes (20)
-- -----------------------------------------------------------------------------
-- Son a quien se le factura el flete, y de ahi sale el "cuanto vendio" por
-- cuenta. Se incluyen dos dados de baja y varios con campos incompletos, que es
-- como llegan los padrones reales: un alta rapida por telefono no trae RFC ni
-- direccion, y la pantalla tiene que soportarlo.
--
-- custom_fields usa las llaves declaradas en 04: dias_credito y ejecutivo.
-- =============================================================================

INSERT INTO customers ("Id", "Name", "TaxId", "ContactName", "Phone", "Email", "Address", custom_fields, "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
('66666666-0000-4000-8000-000000000001', 'Cementos del Bajio',              'CBA150320XY1', 'Rocio Mendez',    '81 8100 0001', 'compras@cementosbajio.mx',    'Parque Industrial Apodaca', '{"dias_credito": "30", "ejecutivo": "Karla Fuentes"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000002', 'Agroindustrias Sinaloa',          'ASI180712QQ2', 'Ivan Robles',     '667 715 0002', 'logistica@agrosinaloa.mx',    'Culiacan, Sinaloa',         '{"dias_credito": "45", "ejecutivo": "Hector Delgado"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '118 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000003', 'Distribuidora Monterrey',         'DMO200105LL3', 'Paola Guzman',    '81 8100 0003', 'compras@distmty.mx',          'Guadalupe, Nuevo Leon',     '{"dias_credito": "15", "ejecutivo": "Karla Fuentes"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '117 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000004', 'Refrigerados del Golfo',          'RGO170228MM4', 'Andres Lira',     '229 980 0004', 'trafico@refrigolfo.mx',       'Veracruz, Veracruz',        '{"dias_credito": "30", "ejecutivo": "Hector Delgado"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '116 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000005', 'Aceros del Norte',                'ANO190614PP5', 'Fabiola Cruz',    '81 8100 0005', 'embarques@acerosnorte.mx',    'Santa Catarina, NL',        '{"dias_credito": "60", "ejecutivo": "Karla Fuentes"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '115 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000006', 'Vidrios Industriales Saltillo',   'VIS210908RR6', 'Omar Zamora',     '844 411 0006', 'compras@vidriossaltillo.mx',  'Ramos Arizpe, Coahuila',    '{"dias_credito": "30"}',                                true,  '11111111-1111-4111-8111-111111111111', now() - interval '112 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000007', 'Alimentos La Huasteca',           'AHU160503TT7', 'Diana Palacios',  '833 260 0007', 'logistica@lahuasteca.mx',     'Tampico, Tamaulipas',       '{"dias_credito": "21", "ejecutivo": "Norma Estrada"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '110 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000008', 'Plasticos del Occidente',         'POC220117UU8', 'Ruben Tapia',     '33 3620 0008', 'compras@plasticosocc.mx',     'Zapopan, Jalisco',          '{"dias_credito": "30", "ejecutivo": "Norma Estrada"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '105 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000009', 'Muebles Queretaro',               'MQU200722VV9', 'Silvia Aguirre',  '442 215 0009', 'trafico@mueblesqro.mx',       'El Marques, Queretaro',     '{"dias_credito": "15"}',                                true,  '11111111-1111-4111-8111-111111111111', now() - interval '100 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000010', 'Quimicos del Pacifico',           'QPA180330WW1', 'Ernesto Bravo',   '669 985 0010', 'seguridad@quimpacifico.mx',   'Mazatlan, Sinaloa',         '{"dias_credito": "45", "ejecutivo": "Hector Delgado"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '96 days',  'datos-de-prueba'),
('66666666-0000-4000-8000-000000000011', 'Granos y Semillas del Norte',     'GSN150211XX2', 'Marisela Duran',  '81 8100 0011', 'compras@granosnorte.mx',      'Escobedo, Nuevo Leon',      '{"dias_credito": "30"}',                                true,  '11111111-1111-4111-8111-111111111111', now() - interval '90 days',  'datos-de-prueba'),
('66666666-0000-4000-8000-000000000012', 'Autopartes Bajio',                'ABA211105YY3', 'Gerardo Nieto',   '477 210 0012', 'logistica@autopartesbajio.mx','Leon, Guanajuato',          '{"dias_credito": "30", "ejecutivo": "Karla Fuentes"}',  true,  '11111111-1111-4111-8111-111111111111', now() - interval '85 days',  'datos-de-prueba'),
('66666666-0000-4000-8000-000000000013', 'Bebidas Regiomontanas',           'BRE190918ZZ4', 'Claudia Pena',    '81 8100 0013', 'embarques@bebidasreg.mx',     'Apodaca, Nuevo Leon',       '{"dias_credito": "7", "ejecutivo": "Norma Estrada"}',   true,  '11111111-1111-4111-8111-111111111111', now() - interval '80 days',  'datos-de-prueba'),
('66666666-0000-4000-8000-000000000014', 'Papel y Carton del Centro',       'PCC170426AA5', 'Alfonso Rivas',   '55 5520 0014', 'compras@papelcentro.mx',      'Tlalnepantla, Edomex',      '{"dias_credito": "30"}',                                true,  '11111111-1111-4111-8111-111111111111', now() - interval '72 days',  'datos-de-prueba'),
('66666666-0000-4000-8000-000000000015', 'Textiles de la Laguna',           'TLA160809BB6', 'Veronica Salas',  '871 750 0015', 'trafico@textileslaguna.mx',   'Torreon, Coahuila',         '{"dias_credito": "21", "ejecutivo": "Hector Delgado"}', true,  '11111111-1111-4111-8111-111111111111', now() - interval '65 days',  'datos-de-prueba'),

-- Altas rapidas por telefono: llegan sin RFC ni direccion, y el sistema debe
-- aceptarlas igual. Sirven para probar que las fichas no se rompen con huecos.
('66666666-0000-4000-8000-000000000016', 'Mudanzas Express',                NULL,           'Jose Luis Marin', '81 8100 0016', NULL,                          NULL,                        '{}',                                                    true,  '11111111-1111-4111-8111-111111111111', now() - interval '40 days',  'datos-de-prueba'),
('66666666-0000-4000-8000-000000000017', 'Ferreteria Industrial Chihuahua', NULL,           NULL,              '614 420 0017', NULL,                          NULL,                        '{}',                                                    true,  '11111111-1111-4111-8111-111111111111', now() - interval '30 days',  'datos-de-prueba'),
('66666666-0000-4000-8000-000000000018', 'Cliente de Contado Ocasional',    NULL,           NULL,              NULL,           NULL,                          NULL,                        '{}',                                                    true,  '11111111-1111-4111-8111-111111111111', now() - interval '12 days',  'datos-de-prueba'),

-- Dados de baja: los viajes historicos los siguen citando, por eso el sistema
-- los desactiva en lugar de borrarlos.
('66666666-0000-4000-8000-000000000019', 'Constructora Peninsular',         'CPE140215CC7', 'Hugo Sandoval',   '999 940 0019', 'compras@constpeninsular.mx',  'Merida, Yucatan',           '{"dias_credito": "60"}',                                false, '11111111-1111-4111-8111-111111111111', now() - interval '300 days', 'datos-de-prueba'),
('66666666-0000-4000-8000-000000000020', 'Maderas del Sureste',             'MSU130704DD8', 'Elena Ochoa',     '993 310 0020', 'logistica@maderassureste.mx', 'Villahermosa, Tabasco',     '{}',                                                    false, '11111111-1111-4111-8111-111111111111', now() - interval '320 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
