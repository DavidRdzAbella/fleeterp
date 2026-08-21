-- =============================================================================
-- Campos a la medida
-- -----------------------------------------------------------------------------
-- Target: 0 = viaje, 1 = unidad, 2 = conductor, 3 = cliente.
-- Type:   0 = texto, 1 = numero, 2 = fecha, 3 = si/no, 4 = lista.
--
-- Estas definiciones son las que hacen aparecer campos extra en los formularios
-- sin tocar codigo; su valor se guarda en la columna jsonb custom_fields de la
-- entidad correspondiente. Se cubren los cinco tipos de dato, obligatorios y
-- opcionales, y uno inactivo que debe dejar de pintarse sin perder lo capturado.
-- =============================================================================

INSERT INTO custom_field_definitions ("Id", "Target", "Key", "Label", "Type", "IsRequired", "Options", "DisplayOrder", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy")
VALUES
('88888888-0000-4000-8000-000000000001', 0, 'permiso_sct',      'Permiso SCT',           0, false, NULL,                                   1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('88888888-0000-4000-8000-000000000002', 0, 'tipo_carga',       'Tipo de carga',         4, true,  'General|Refrigerada|Peligrosa|Granel', 2, true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('88888888-0000-4000-8000-000000000003', 0, 'carta_porte',      'Folio de carta porte',  0, false, NULL,                                   3, true,  '11111111-1111-4111-8111-111111111111', now() - interval '90 days',  'datos-de-prueba'),
('88888888-0000-4000-8000-000000000004', 0, 'requiere_custodia','Requiere custodia',     3, false, NULL,                                   4, true,  '11111111-1111-4111-8111-111111111111', now() - interval '75 days',  'datos-de-prueba'),
('88888888-0000-4000-8000-000000000005', 1, 'gps_id',           'Identificador GPS',     0, false, NULL,                                   1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '119 days', 'datos-de-prueba'),
('88888888-0000-4000-8000-000000000006', 1, 'ejes',             'Numero de ejes',        1, false, NULL,                                   2, true,  '11111111-1111-4111-8111-111111111111', now() - interval '85 days',  'datos-de-prueba'),
('88888888-0000-4000-8000-000000000007', 2, 'tipo_sangre',      'Tipo de sangre',        4, false, 'O+|O-|A+|A-|B+|B-|AB+|AB-',            1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '70 days',  'datos-de-prueba'),
('88888888-0000-4000-8000-000000000008', 2, 'examen_medico',    'Vence examen medico',   2, false, NULL,                                   2, true,  '11111111-1111-4111-8111-111111111111', now() - interval '70 days',  'datos-de-prueba'),
('88888888-0000-4000-8000-000000000009', 3, 'dias_credito',     'Dias de credito',       1, false, NULL,                                   1, true,  '11111111-1111-4111-8111-111111111111', now() - interval '65 days',  'datos-de-prueba'),
('88888888-0000-4000-8000-000000000010', 3, 'ejecutivo',        'Ejecutivo de cuenta',   0, false, NULL,                                   2, true,  '11111111-1111-4111-8111-111111111111', now() - interval '65 days',  'datos-de-prueba'),
('88888888-0000-4000-8000-000000000011', 0, 'campo_retirado',   'Campo ya no usado',     0, false, NULL,                                   9, false, '11111111-1111-4111-8111-111111111111', now() - interval '200 days', 'datos-de-prueba')

ON CONFLICT ("Id") DO NOTHING;
