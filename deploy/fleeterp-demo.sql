--
-- PostgreSQL database dump
--

\restrict 8nn3x1UJbSJ1gd9kgzkxm52RfdvfjVplftv88mUhMKIhmI4pg0GmvNzkGZQZh3B

-- Dumped from database version 18.4
-- Dumped by pg_dump version 18.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- Name: custom_field_definitions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.custom_field_definitions (
    "Id" uuid NOT NULL,
    "Target" integer NOT NULL,
    "Key" character varying(40) NOT NULL,
    "Label" character varying(80) NOT NULL,
    "Type" integer NOT NULL,
    "IsRequired" boolean NOT NULL,
    "Options" character varying(1000),
    "DisplayOrder" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: customers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.customers (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "TaxId" character varying(30),
    "ContactName" character varying(120),
    "Phone" character varying(30),
    "Email" character varying(150),
    "Address" character varying(300),
    custom_fields jsonb NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: drivers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.drivers (
    "Id" uuid NOT NULL,
    "FirstName" character varying(60) NOT NULL,
    "LastName" character varying(60) NOT NULL,
    "EmployeeNumber" character varying(30),
    "LicenseNumber" character varying(40) NOT NULL,
    "LicenseType" character varying(30),
    "LicenseExpiry" date,
    "Phone" character varying(30),
    "Email" character varying(150),
    "HireDate" date,
    "PayScheme" integer NOT NULL,
    "PayRate" numeric(18,2) NOT NULL,
    "Status" integer NOT NULL,
    custom_fields jsonb NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: expense_categories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.expense_categories (
    "Id" uuid NOT NULL,
    "Code" character varying(20) NOT NULL,
    "Name" character varying(80) NOT NULL,
    "IsTripRelated" boolean NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: expenses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.expenses (
    "Id" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    "TripId" uuid,
    "VehicleId" uuid,
    "DriverId" uuid,
    "IncurredAtUtc" timestamp with time zone NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Description" character varying(250) NOT NULL,
    "ReferenceNumber" character varying(60),
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: fuel_logs; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.fuel_logs (
    "Id" uuid NOT NULL,
    "VehicleId" uuid NOT NULL,
    "TripId" uuid,
    "DriverId" uuid,
    "LoadedAtUtc" timestamp with time zone NOT NULL,
    "Quantity" numeric(18,3) NOT NULL,
    "PricePerUnit" numeric(18,4) NOT NULL,
    "TotalCost" numeric(18,2) NOT NULL,
    "OdometerReading" numeric(18,2),
    "Station" character varying(120),
    "ReferenceNumber" character varying(60),
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: maintenance_orders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.maintenance_orders (
    "Id" uuid NOT NULL,
    "Folio" character varying(30) NOT NULL,
    "VehicleId" uuid NOT NULL,
    "Kind" integer NOT NULL,
    "Status" integer NOT NULL,
    "OpenedAtUtc" timestamp with time zone NOT NULL,
    "ClosedAtUtc" timestamp with time zone,
    "Description" character varying(500) NOT NULL,
    "Workshop" character varying(150),
    "Cost" numeric(18,2) NOT NULL,
    "OdometerAtService" numeric(18,2),
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: tenants; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tenants (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Slug" character varying(60) NOT NULL,
    "TaxId" character varying(30),
    "ContactEmail" character varying(150),
    "Phone" character varying(30),
    settings jsonb NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: trips; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.trips (
    "Id" uuid NOT NULL,
    "Folio" character varying(30) NOT NULL,
    "DriverId" uuid NOT NULL,
    "VehicleId" uuid NOT NULL,
    "TrailerId" uuid,
    "CustomerId" uuid,
    "Origin" character varying(150) NOT NULL,
    "Destination" character varying(150) NOT NULL,
    "PlannedDistance" numeric(18,2) NOT NULL,
    "ScheduledDepartureUtc" timestamp with time zone NOT NULL,
    "ScheduledArrivalUtc" timestamp with time zone,
    "ActualDepartureUtc" timestamp with time zone,
    "ActualArrivalUtc" timestamp with time zone,
    "OdometerStart" numeric(18,2),
    "OdometerEnd" numeric(18,2),
    "InitialFuel" numeric(18,3) NOT NULL,
    "FinalFuel" numeric(18,3),
    "RefuelPlanned" boolean NOT NULL,
    "CargoWeight" numeric(18,3) NOT NULL,
    "CargoWeightUnit" integer NOT NULL,
    "CargoDescription" character varying(300),
    "FreightRevenue" numeric(18,2) NOT NULL,
    "DriverPayScheme" integer NOT NULL,
    "DriverPayRate" numeric(18,2) NOT NULL,
    "DriverHours" numeric(10,2),
    "DriverPayAmount" numeric(18,2) NOT NULL,
    "Status" integer NOT NULL,
    "Notes" character varying(1000),
    "CancellationReason" character varying(300),
    custom_fields jsonb NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    "Id" uuid NOT NULL,
    "Email" character varying(150) NOT NULL,
    "FullName" character varying(120) NOT NULL,
    "PasswordHash" character varying(300) NOT NULL,
    "Role" integer NOT NULL,
    "LastLoginUtc" timestamp with time zone,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: vehicle_types; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.vehicle_types (
    "Id" uuid NOT NULL,
    "Code" character varying(20) NOT NULL,
    "Name" character varying(80) NOT NULL,
    "Category" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Name: vehicles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.vehicles (
    "Id" uuid NOT NULL,
    "EconomicNumber" character varying(30) NOT NULL,
    "PlateNumber" character varying(20) NOT NULL,
    "VehicleTypeId" uuid NOT NULL,
    "Brand" character varying(60),
    "Model" character varying(60),
    "Year" integer,
    "Vin" character varying(40),
    "CargoCapacity" numeric(18,3),
    "TankCapacity" numeric(18,3),
    "CurrentOdometer" numeric(18,2) NOT NULL,
    "Status" integer NOT NULL,
    "InsuranceExpiry" date,
    "CirculationCardExpiry" date,
    custom_fields jsonb NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" uuid NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAtUtc" timestamp with time zone,
    "UpdatedBy" text
);


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260806081455_InitialSchema	9.0.0
\.


--
-- Data for Name: custom_field_definitions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.custom_field_definitions ("Id", "Target", "Key", "Label", "Type", "IsRequired", "Options", "DisplayOrder", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
7413a79f-4b18-4407-b822-da2f46e6a75f	0	tipo_carga	Tipo de carga	4	f	General|Refrigerada|Peligrosa|Granel	2	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
7de90cf3-0fc8-476c-890d-7179657f2539	0	permiso_sct	Permiso SCT	0	f	\N	1	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
fe03ddac-ebf8-4013-800d-8a4cb5e79b26	1	gps_id	Identificador GPS	0	f	\N	1	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
\.


--
-- Data for Name: customers; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.customers ("Id", "Name", "TaxId", "ContactName", "Phone", "Email", "Address", custom_fields, "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Cementos del Bajío	CBA150320XY1	Compras	81 8100 0000	compras@cliente.mx	Parque Industrial	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Refrigerados del Golfo	RGO170228MM4	Compras	81 8100 0000	compras@cliente.mx	Parque Industrial	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
cec94ae7-fadf-4db8-981d-80df95ba3774	Distribuidora Monterrey	DMO200105LL3	Compras	81 8100 0000	compras@cliente.mx	Parque Industrial	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
25f2f001-f2e9-427f-8f03-2fbf49847c6b	Agroindustrias Sinaloa	ASI180712QQ2	Compras	81 8100 0000	compras@cliente.mx	Parque Industrial	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 08:06:26.333281-06	admin@demo.com
\.


--
-- Data for Name: drivers; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.drivers ("Id", "FirstName", "LastName", "EmployeeNumber", "LicenseNumber", "LicenseType", "LicenseExpiry", "Phone", "Email", "HireDate", "PayScheme", "PayRate", "Status", custom_fields, "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
0a6009aa-244a-426b-85e6-5590d7baaf5a	Marisol	Aguilar	OP-006	LIC-889006	Federal Tipo E	2026-10-20	81 1532 4936	marisol@transportesdelnorte.mx	2024-10-19	2	2800.00	0	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
8222ade5-65e9-4ef5-b185-0c71996cf02c	Efraín	González	OP-005	LIC-889005	Federal Tipo E	2026-12-14	81 1562 4435	efraín@transportesdelnorte.mx	2024-07-19	3	12.00	0	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
86e72142-66cc-461b-94e0-f5daff605e4f	Rogelio	Salinas	OP-003	LIC-889003	Federal Tipo E	2027-04-23	81 1554 6328	rogelio@transportesdelnorte.mx	2023-01-21	1	3.20	0	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
9efa0c7e-03cf-4013-b0e1-36df51f3b84a	Carmen	Ibarra	OP-004	LIC-889004	Federal Tipo E	2028-01-28	81 1551 5902	carmen@transportesdelnorte.mx	2023-05-21	0	105.00	0	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
2bced3f7-432c-4465-9b96-c0322c375bc2	Juanito	Pérez	OP-002	LIC-889002	Federal Tipo E	2026-08-28	81 1580 5588	juanito@transportesdelnorte.mx	2023-05-08	0	95.00	1	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
705280e1-81dd-4793-b352-fbf5593d35b8	Ulises	Mendoza	OP-001	LIC-889001	Federal Tipo E	2027-09-10	81 1597 2073	ulises@transportesdelnorte.mx	2023-02-16	0	110.00	1	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
\.


--
-- Data for Name: expense_categories; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.expense_categories ("Id", "Code", "Name", "IsTripRelated", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
5bdecc19-6d6a-4e4b-9dcd-161897a07302	ADMIN	Gastos administrativos	f	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	VIATICOS	Viáticos del operador	t	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
6b7f3660-df40-49fb-894e-31187ae75007	MANIOBRA	Maniobras de carga y descarga	t	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
7e81e63e-f6f7-4737-958c-7476884bcca7	MULTAS	Multas e infracciones	t	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
c1ee2b83-1a2d-405e-9530-669fac3cf02a	REFACC	Refacciones y llantas	t	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
c2396d46-e3e7-4690-912b-a9d2b1768562	CASETAS	Casetas y peajes	t	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
\.


--
-- Data for Name: expenses; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.expenses ("Id", "CategoryId", "TripId", "VehicleId", "DriverId", "IncurredAtUtc", "Amount", "Description", "ReferenceNumber", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
00221758-cb80-4605-b21a-90abdff1a8dd	6b7f3660-df40-49fb-894e-31187ae75007	66effb93-a98b-4d83-a73e-dbc6713eced6	8f3ca495-c031-43be-88f9-403c2813e1d7	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-07 04:00:00-06	725.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
003b5a44-948b-4ff6-b5eb-553b9da85b9d	c2396d46-e3e7-4690-912b-a9d2b1768562	e0ceaa50-1da7-4544-8212-86f6fd3a35c8	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-24 05:00:00-06	1559.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0042a97a-ebdf-4ac7-afbf-79082073a027	c1ee2b83-1a2d-405e-9530-669fac3cf02a	b0c91b64-30c3-476f-b6d9-5231727c1e55	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-16 03:00:00-06	777.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
00a5cbf0-4081-450f-bc9a-0f4a8974870f	c1ee2b83-1a2d-405e-9530-669fac3cf02a	23e05f3e-f76d-43fd-8406-0f1fd143f59c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 08:00:00-06	1669.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
01abb88a-2af7-4f23-99fb-8239eb316dab	c2396d46-e3e7-4690-912b-a9d2b1768562	a28f3acc-667c-4b32-913a-992935f08655	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-13 02:00:00-06	608.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0332f604-252e-4468-9e47-f0551ef8d0c6	6b7f3660-df40-49fb-894e-31187ae75007	1a0bca72-0d5e-4e73-b1dd-04efff6b70e7	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-23 04:00:00-06	579.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
06ecc0b3-a0da-4749-8776-e71d593549cf	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	6f5eb4f8-d810-4feb-ba87-50ff97277a8a	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-29 11:00:00-06	938.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
07cc1e5a-8fb9-4e36-a610-e264d4479518	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	65111110-c9a7-4096-a902-4c3d1ce62319	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-14 05:00:00-06	780.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
085c764b-182c-40a6-9f5d-ccacb38f939a	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	61959438-3cc4-46a4-8ed1-e8589e24d4a2	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-25 08:00:00-06	786.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0887d68a-8df0-4f34-9cbd-e87c284ea0dc	c1ee2b83-1a2d-405e-9530-669fac3cf02a	6dbfefa7-101c-46ca-9786-28a3adb235f6	8f3ca495-c031-43be-88f9-403c2813e1d7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-11 03:00:00-06	3296.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
08adf6a1-c1e2-46cb-a19c-b7fd4b3ab105	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	a4a9db81-97c6-47f4-a683-10e1fa576dd4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-26 08:00:00-06	435.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
08e54e53-8b64-421e-b3b1-d894bbe379bf	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	f31876fa-328e-4f8e-a998-b48296296a55	aa074c2c-fe5a-442a-a6ed-669d6739062f	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-18 05:00:00-06	1149.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
09400bac-d636-4e88-af6e-762d2882b877	6b7f3660-df40-49fb-894e-31187ae75007	1bdeb0d1-f0f6-4770-baa9-7e0ca417336f	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-27 05:00:00-06	761.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
097d6a9c-1334-4cb9-ae32-c520ac8a599e	6b7f3660-df40-49fb-894e-31187ae75007	3c972500-11d1-48df-bb21-8ddfff247b11	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-02 03:00:00-06	898.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
09bba780-73ba-4107-b6aa-c36442a28706	c2396d46-e3e7-4690-912b-a9d2b1768562	ea6385c1-81ea-43a1-979a-c8753287d742	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-23 05:00:00-06	1965.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0a2fc09b-74c4-4a10-846f-2fefa372d1e7	c1ee2b83-1a2d-405e-9530-669fac3cf02a	7546fdce-398a-4265-b9a7-92db4ea57cf8	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-21 09:00:00-06	2252.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0b25a54b-88b7-4385-8e3c-6fca5e338da6	c1ee2b83-1a2d-405e-9530-669fac3cf02a	aa9c84e1-b07f-46cd-bfcb-014dbc1964f7	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-13 09:00:00-06	2574.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0d2237bc-a651-4a73-a2ce-9780737d8f54	c1ee2b83-1a2d-405e-9530-669fac3cf02a	eedc9484-c795-499d-ad23-4be0a563a8ee	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-25 08:00:00-06	2756.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0ed98f40-d0ad-4874-a162-33d8e45e6bcd	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	3ed14117-4757-4269-8790-b7b2c982052f	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-03 10:00:00-06	970.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0f21b9dc-b0f7-496c-ab2b-36c7121397b1	c1ee2b83-1a2d-405e-9530-669fac3cf02a	61959438-3cc4-46a4-8ed1-e8589e24d4a2	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-25 08:00:00-06	2158.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0f45b135-db23-4607-901e-e3c8610b7f49	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	7d03c213-67ac-4b36-ae23-3043682f983a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-27 02:00:00-06	414.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0fcb62b5-75a1-45be-aae1-bc5e1d79a407	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	54f2ed83-df52-4d5e-9ed6-1cda3cd14d89	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-29 03:00:00-06	609.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1087055b-0b3f-4191-ada3-4f6aa5b89e19	c1ee2b83-1a2d-405e-9530-669fac3cf02a	cc642552-a331-439c-8eae-1e7691304214	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-27 09:00:00-06	2822.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1135f905-f243-4315-b258-27c7425ee112	7e81e63e-f6f7-4737-958c-7476884bcca7	5ccd65ec-1fda-42cf-84f0-73edee47802e	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-24 05:00:00-06	521.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
13413ed8-95ca-4562-8422-e194cf20e901	c2396d46-e3e7-4690-912b-a9d2b1768562	f4a8b079-d294-4931-be1c-1f7ca3bc0cda	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-05 05:00:00-06	868.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
13d3b2ee-4b6e-48ef-88c5-562c0dde0fd8	c2396d46-e3e7-4690-912b-a9d2b1768562	3c972500-11d1-48df-bb21-8ddfff247b11	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-02 03:00:00-06	1044.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
17912e8d-dcb5-43d7-be49-0acfaf96cc83	c2396d46-e3e7-4690-912b-a9d2b1768562	aa9c84e1-b07f-46cd-bfcb-014dbc1964f7	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-13 09:00:00-06	2587.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
19ad486b-d3e0-4623-8092-df7e8624142a	c2396d46-e3e7-4690-912b-a9d2b1768562	e11feb81-c505-40ce-8411-ad49e353cba9	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-22 06:00:00-06	1411.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
19f063ef-49be-477b-a202-52ded29eb294	c2396d46-e3e7-4690-912b-a9d2b1768562	e525281b-8d5b-4551-8c53-4816c5099a08	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-16 03:00:00-06	1300.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1bbafbb6-5265-46b0-bec8-cd2c15924fb3	c1ee2b83-1a2d-405e-9530-669fac3cf02a	a739ea38-9105-4b31-abae-107484078d4a	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 11:00:00-06	2411.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1bdfe12e-8c4f-41b1-9ee4-4e7cb4dd1877	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	18870d3c-5de0-478a-a60b-8c2da843ee33	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-09 07:00:00-06	457.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1c0769af-3f40-47a5-9174-7c5ba8b002a5	c1ee2b83-1a2d-405e-9530-669fac3cf02a	12021d76-8cba-4893-ad95-086812a7a00b	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-06-23 06:00:00-06	1344.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1d9e115b-26dd-45a4-882b-f95bdcf5e35e	c1ee2b83-1a2d-405e-9530-669fac3cf02a	7d03c213-67ac-4b36-ae23-3043682f983a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-27 02:00:00-06	1878.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1dd2ed35-42a7-4620-bd83-0d6d3b2157db	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	b678f560-3801-4679-bc9f-0cdd1a8e3c41	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-01 04:00:00-06	404.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1e6dca4a-9449-4732-9cb8-5683146a1769	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	919d551c-9c3d-452f-9b96-d70076cef288	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-14 05:00:00-06	992.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1e7154c1-7f4f-44f2-a450-ce859c75c101	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	1957d298-0fa7-4a01-ae83-294864f8fe40	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-30 05:00:00-06	920.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
20f62f03-59b9-4f8b-a0b4-133144803d65	7e81e63e-f6f7-4737-958c-7476884bcca7	98b69f0f-f059-40aa-b1b0-ce51678ae08c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-24 07:00:00-06	474.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
21296a07-8ff8-4cc3-a3ec-943744fc8788	6b7f3660-df40-49fb-894e-31187ae75007	1f0496f2-8b3d-40ba-8ff5-5301c453693a	1709cadd-2a88-48cd-bc22-344c9b2949cf	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-17 03:00:00-06	468.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2765cd80-7849-444f-8f4f-cc9cbde31055	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	9e76936b-bb57-45c0-9b19-c0f5ab16cfe4	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-19 02:00:00-06	997.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
27db8f31-6b12-42e4-85ad-6a83475021d3	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	7546fdce-398a-4265-b9a7-92db4ea57cf8	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-21 09:00:00-06	801.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
27e08f34-e823-45fd-9845-714873875a10	7e81e63e-f6f7-4737-958c-7476884bcca7	1957d298-0fa7-4a01-ae83-294864f8fe40	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-30 05:00:00-06	383.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
29770d39-9c6f-4eab-8f4a-815e75519460	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	fa62a42d-22de-439b-9c01-7a3f3a67886b	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-30 08:00:00-06	1127.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2aeb5074-c711-47b5-980f-e4cd186ddaa8	6b7f3660-df40-49fb-894e-31187ae75007	919d551c-9c3d-452f-9b96-d70076cef288	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-14 05:00:00-06	672.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2b1a49ab-2ee4-42a8-a90f-984eaa50613b	6b7f3660-df40-49fb-894e-31187ae75007	99e79f0d-bee9-4643-8a1a-77ba58adcfa0	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-19 03:00:00-06	700.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2e001b74-a1bb-415a-b6a9-6950d832f5b8	7e81e63e-f6f7-4737-958c-7476884bcca7	9a65ff54-2df7-412f-b306-1eb8565d392f	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-04 04:00:00-06	589.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2eb3a023-fb02-488a-8cb8-e3db62ed6ee6	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	ca0904ab-3166-43df-bb4a-1793791426f5	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-08-04 03:00:00-06	1017.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2ef1969f-b230-4744-8168-bbb3fa6ef3df	c2396d46-e3e7-4690-912b-a9d2b1768562	5819ad28-5367-4907-86f9-1c323d6dfef7	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-01 08:00:00-06	694.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2f415905-e713-47c9-8700-5b4c433ed0cf	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	f8b75f3f-f8c4-4ee6-ba82-82ef98204dbb	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-06 11:00:00-06	1171.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2fd1c261-8387-47ea-a687-a7f833e7c34c	c2396d46-e3e7-4690-912b-a9d2b1768562	839924a4-7be3-4512-a079-4bc2db5cc4ed	1709cadd-2a88-48cd-bc22-344c9b2949cf	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-07 09:00:00-06	1794.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3037ba39-a471-456a-9d73-ddb606a9a667	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	9042d857-6baa-4313-a620-99c58939b5d3	8f3ca495-c031-43be-88f9-403c2813e1d7	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-24 02:00:00-06	635.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
303dedb0-6f69-4f6e-817f-93ba38bc0ff4	6b7f3660-df40-49fb-894e-31187ae75007	0a84b59a-f3fe-4a5d-b34d-9b400748921f	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-28 09:00:00-06	439.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3093accd-f426-4233-83bb-2f8e97a1c080	7e81e63e-f6f7-4737-958c-7476884bcca7	d983083a-fdb5-4fc2-acfb-85ea6549f4e7	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-23 07:00:00-06	218.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
30c5e307-6f06-4708-b9c4-823eaa1bbb42	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	1bdeb0d1-f0f6-4770-baa9-7e0ca417336f	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-27 05:00:00-06	460.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
33d3dce7-09db-486b-a6f8-5f9cfc8c674c	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	16cc60f9-9a3f-41df-aa60-f62a45a23905	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-22 05:00:00-06	459.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
346ee07f-ff13-4dd3-b24a-8e7e0777737e	c1ee2b83-1a2d-405e-9530-669fac3cf02a	8ce37e6d-ab47-4eba-a3e1-150bd6ceefa2	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-26 02:00:00-06	686.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
34bb2e22-0080-47f9-a4d1-eb5a99c7d624	c2396d46-e3e7-4690-912b-a9d2b1768562	8ce37e6d-ab47-4eba-a3e1-150bd6ceefa2	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-26 02:00:00-06	2161.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3682e484-5a11-4694-9ec6-952b23b2aecc	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	98b69f0f-f059-40aa-b1b0-ce51678ae08c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-24 07:00:00-06	1150.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
375e8651-7682-42f4-b41c-aee1da102740	6b7f3660-df40-49fb-894e-31187ae75007	12021d76-8cba-4893-ad95-086812a7a00b	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-06-23 06:00:00-06	654.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
37a677fd-f480-4f33-92e0-5276252f856e	7e81e63e-f6f7-4737-958c-7476884bcca7	f4a8b079-d294-4931-be1c-1f7ca3bc0cda	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-05 05:00:00-06	781.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
381eb47a-bb26-4daf-95fa-fa2016881545	7e81e63e-f6f7-4737-958c-7476884bcca7	7400c6aa-8a44-4a34-b76b-4824038a790d	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-20 05:00:00-06	689.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3a1878b1-5c15-421f-82db-5dee7badbd10	6b7f3660-df40-49fb-894e-31187ae75007	245d4afd-7294-4492-9757-c2948b7ec3b0	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-30 03:00:00-06	883.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3b48e22d-a186-47b6-9be0-9f5294cd3902	6b7f3660-df40-49fb-894e-31187ae75007	65111110-c9a7-4096-a902-4c3d1ce62319	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-14 05:00:00-06	555.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3c7b35d9-c561-494e-85d5-d66b02d5b2a9	6b7f3660-df40-49fb-894e-31187ae75007	9f0202ef-2f4d-4504-87bf-b301512a6763	aa074c2c-fe5a-442a-a6ed-669d6739062f	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 10:00:00-06	740.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3c9ef389-8523-429d-a1f7-fb553f267df7	6b7f3660-df40-49fb-894e-31187ae75007	5819ad28-5367-4907-86f9-1c323d6dfef7	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-01 08:00:00-06	440.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3e92162f-b4e8-4d11-8386-05f7d2530f86	6b7f3660-df40-49fb-894e-31187ae75007	e11feb81-c505-40ce-8411-ad49e353cba9	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-22 06:00:00-06	565.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3eb8cf75-6cfe-4923-af0f-b1acb8a44423	c2396d46-e3e7-4690-912b-a9d2b1768562	919d551c-9c3d-452f-9b96-d70076cef288	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-14 05:00:00-06	1781.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3edf590a-b4d6-4d3f-bd25-c8c83f50e177	6b7f3660-df40-49fb-894e-31187ae75007	a0eed33f-16af-4cf7-a332-4a7335323c03	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-08 07:00:00-06	601.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3fb859a4-0299-4757-94d8-add3158d2264	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	4a76544d-2b35-47ca-85bf-afbbe82a7552	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-02 06:00:00-06	1045.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
40be3748-da82-4752-b4b8-0b24c0f55da3	6b7f3660-df40-49fb-894e-31187ae75007	23974bf1-2f32-465d-93c9-168327cf7779	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-06 11:00:00-06	645.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
425eadde-1e77-4ddf-a37b-95c15a0d33c5	7e81e63e-f6f7-4737-958c-7476884bcca7	99e79f0d-bee9-4643-8a1a-77ba58adcfa0	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-19 03:00:00-06	224.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
442575cf-ff3f-456a-a0ee-03e0c1c01379	6b7f3660-df40-49fb-894e-31187ae75007	868be146-c911-4415-94c5-faf6ec1fa3ab	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-15 07:00:00-06	754.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
46a065c4-bafa-496e-a278-f3de23492c07	6b7f3660-df40-49fb-894e-31187ae75007	5c115bbe-ec90-40d9-8512-13696bdc6a22	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-05 05:00:00-06	547.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
46df4152-2e08-442e-ba76-ae163d43ce0c	c1ee2b83-1a2d-405e-9530-669fac3cf02a	55ffc6b7-8949-4403-85fc-cc113dd908cb	8f3ca495-c031-43be-88f9-403c2813e1d7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-22 09:00:00-06	2433.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
49b3bce4-c8e6-4286-b078-e18c03faebe9	c2396d46-e3e7-4690-912b-a9d2b1768562	31c2aee8-9f27-47a4-a98d-4fe01c3eef05	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-28 08:00:00-06	1664.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4a29e051-44a6-45a3-a0bb-80f175ca07b6	c2396d46-e3e7-4690-912b-a9d2b1768562	f31876fa-328e-4f8e-a998-b48296296a55	aa074c2c-fe5a-442a-a6ed-669d6739062f	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-18 05:00:00-06	1727.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4abb9733-e7fd-4434-808e-d0e9b24162c8	c2396d46-e3e7-4690-912b-a9d2b1768562	9a65ff54-2df7-412f-b306-1eb8565d392f	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-04 04:00:00-06	1421.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4af9d691-1086-41b3-8580-fb78fbd8bce9	7e81e63e-f6f7-4737-958c-7476884bcca7	f8ddeedb-3b8a-41df-bf00-1560c4864863	aa074c2c-fe5a-442a-a6ed-669d6739062f	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-12 03:00:00-06	685.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4c7ab099-144f-46ff-84c3-3e5809c0cb1f	c1ee2b83-1a2d-405e-9530-669fac3cf02a	4a1c287c-7be1-49b7-84b0-bd0cd4f84037	8f3ca495-c031-43be-88f9-403c2813e1d7	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-28 05:00:00-06	1772.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4dc8296b-8630-4a28-9bc7-00a99234676c	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	2b95e099-6abf-43c4-ae92-9dd41351dde7	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-07 08:00:00-06	1141.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4ebe9396-9d2c-42d1-a978-858775464813	c2396d46-e3e7-4690-912b-a9d2b1768562	b0c91b64-30c3-476f-b6d9-5231727c1e55	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-16 03:00:00-06	1153.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4fc2b5a9-57ad-4184-8ae3-02b399327f66	c2396d46-e3e7-4690-912b-a9d2b1768562	1a0bca72-0d5e-4e73-b1dd-04efff6b70e7	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-23 04:00:00-06	1433.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
50194f4e-2b87-4cb5-822a-fbd7c0020db4	6b7f3660-df40-49fb-894e-31187ae75007	3ed14117-4757-4269-8790-b7b2c982052f	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-03 10:00:00-06	848.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
52f91183-f165-4444-83d1-1f27154926c7	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	8031560b-18d7-4f4b-a6b5-8db0e0a44e2c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-06 06:00:00-06	419.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
53666796-a7ef-4de4-9465-056163ee1b20	c2396d46-e3e7-4690-912b-a9d2b1768562	305c36be-2652-4f7d-b3d2-e798afe40c17	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-15 04:00:00-06	1843.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
54120c37-60dc-411a-ad60-98ea88d5aed6	c2396d46-e3e7-4690-912b-a9d2b1768562	23974bf1-2f32-465d-93c9-168327cf7779	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-06 11:00:00-06	958.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
55dd0946-70ca-41df-b41a-68361ee80973	c2396d46-e3e7-4690-912b-a9d2b1768562	6e0b3337-4dd1-4973-9a4b-bf9752ac7422	1709cadd-2a88-48cd-bc22-344c9b2949cf	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-02 03:00:00-06	989.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
568e3016-d552-415a-b7d4-48a32554c157	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	d17b5514-b2b8-4600-89b5-db0c7589fcda	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-01 02:00:00-06	753.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5a20b966-66d5-480e-92b4-e942dd9d2a69	6b7f3660-df40-49fb-894e-31187ae75007	55ffc6b7-8949-4403-85fc-cc113dd908cb	8f3ca495-c031-43be-88f9-403c2813e1d7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-22 09:00:00-06	674.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5ac7288c-c762-4002-9ec0-917bde8e8772	c2396d46-e3e7-4690-912b-a9d2b1768562	54f2ed83-df52-4d5e-9ed6-1cda3cd14d89	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-29 03:00:00-06	742.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5afab83c-6c63-4d1c-aeae-325db853f253	6b7f3660-df40-49fb-894e-31187ae75007	cc642552-a331-439c-8eae-1e7691304214	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-27 09:00:00-06	759.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5bcd6bac-57ba-4661-85c9-815986250b59	c2396d46-e3e7-4690-912b-a9d2b1768562	4a1c287c-7be1-49b7-84b0-bd0cd4f84037	8f3ca495-c031-43be-88f9-403c2813e1d7	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-28 05:00:00-06	1477.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5c0da96b-b615-437a-be5d-589447ee9cec	c1ee2b83-1a2d-405e-9530-669fac3cf02a	9f0202ef-2f4d-4504-87bf-b301512a6763	aa074c2c-fe5a-442a-a6ed-669d6739062f	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 10:00:00-06	786.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6096e4d7-5deb-4a7c-87e7-6e73b68129a0	6b7f3660-df40-49fb-894e-31187ae75007	cf783a12-df74-4a45-96d9-ac1c812bb7d3	aa074c2c-fe5a-442a-a6ed-669d6739062f	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-22 06:00:00-06	885.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
611529b8-c6dd-4a45-84c0-a65901761b5f	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	5c115bbe-ec90-40d9-8512-13696bdc6a22	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-05 05:00:00-06	1087.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
620fcdd9-3bd6-47d8-92f0-b3a8e2d30720	c2396d46-e3e7-4690-912b-a9d2b1768562	d17b5514-b2b8-4600-89b5-db0c7589fcda	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-01 02:00:00-06	686.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
62870925-579b-478f-8a12-1d635a911cb1	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	6b93721a-c050-4375-b083-987cfd95a4eb	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 06:00:00-06	1152.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
62e8225b-966f-4bb0-8b1f-dda74c80e527	c2396d46-e3e7-4690-912b-a9d2b1768562	8031560b-18d7-4f4b-a6b5-8db0e0a44e2c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-06 06:00:00-06	1739.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6353ee6c-3a11-42ef-ad15-17d97f6f4629	c1ee2b83-1a2d-405e-9530-669fac3cf02a	0a770914-95e7-4d16-8aa0-30f4984c2df6	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-02 02:00:00-06	1508.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
67ee6484-4d9a-4043-b982-a6e519271493	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	4d1fc37d-7b1f-4b37-9230-b52209285833	1709cadd-2a88-48cd-bc22-344c9b2949cf	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-31 05:00:00-06	529.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
685ead5c-9ea7-4e74-8e64-253cf8162e33	c2396d46-e3e7-4690-912b-a9d2b1768562	61959438-3cc4-46a4-8ed1-e8589e24d4a2	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-25 08:00:00-06	1658.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6951d553-8ddb-4f57-a915-0bb9bda34b27	6b7f3660-df40-49fb-894e-31187ae75007	b678f560-3801-4679-bc9f-0cdd1a8e3c41	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-01 04:00:00-06	789.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6abe55df-992a-489a-81ad-300925cadc8c	c2396d46-e3e7-4690-912b-a9d2b1768562	a739ea38-9105-4b31-abae-107484078d4a	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 11:00:00-06	1851.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6b8bbc79-821e-4b38-9a1c-db80ee574b8c	c2396d46-e3e7-4690-912b-a9d2b1768562	b678f560-3801-4679-bc9f-0cdd1a8e3c41	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-01 04:00:00-06	893.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6bcfb408-5868-4bd5-a2a9-9aca0a23638d	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	305c36be-2652-4f7d-b3d2-e798afe40c17	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-15 04:00:00-06	860.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6c68c5ed-347e-4e10-950c-c2ce29fcf568	c1ee2b83-1a2d-405e-9530-669fac3cf02a	4d1fc37d-7b1f-4b37-9230-b52209285833	1709cadd-2a88-48cd-bc22-344c9b2949cf	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-31 05:00:00-06	2816.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6d1660ba-b2fa-433b-97d4-182d9e422279	c1ee2b83-1a2d-405e-9530-669fac3cf02a	ea6385c1-81ea-43a1-979a-c8753287d742	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-23 05:00:00-06	1944.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6f308a76-9e82-48e0-b0a3-c2a13ff68a59	c2396d46-e3e7-4690-912b-a9d2b1768562	5ccd65ec-1fda-42cf-84f0-73edee47802e	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-24 05:00:00-06	1576.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
71df75e6-1f51-4f2a-ae66-1ed64ed62699	c2396d46-e3e7-4690-912b-a9d2b1768562	ca0904ab-3166-43df-bb4a-1793791426f5	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-08-04 03:00:00-06	1452.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
737525a4-e92b-41c9-afdd-2d60dbf34d80	c1ee2b83-1a2d-405e-9530-669fac3cf02a	bb3defaa-2e0c-42a1-af26-ae085813c47b	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-28 04:00:00-06	2582.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
74416f05-6852-4aea-af47-a1c0a6b905d6	6b7f3660-df40-49fb-894e-31187ae75007	31c2aee8-9f27-47a4-a98d-4fe01c3eef05	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-28 08:00:00-06	836.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
749b4f5a-f4b9-4d9f-abef-c90388503fae	6b7f3660-df40-49fb-894e-31187ae75007	4d1fc37d-7b1f-4b37-9230-b52209285833	1709cadd-2a88-48cd-bc22-344c9b2949cf	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-31 05:00:00-06	487.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7578a47b-3280-40f1-b7a0-ef529bd6c574	7e81e63e-f6f7-4737-958c-7476884bcca7	18870d3c-5de0-478a-a60b-8c2da843ee33	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-09 07:00:00-06	670.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
762872db-d1ce-4d2b-92a2-d577720e501d	6b7f3660-df40-49fb-894e-31187ae75007	35dcc603-9902-4d26-acf6-5d7fab7c81ba	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-19 03:00:00-06	715.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
76e41f2e-7ad1-46a9-a37d-024307353c99	c2396d46-e3e7-4690-912b-a9d2b1768562	2b95e099-6abf-43c4-ae92-9dd41351dde7	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-07 08:00:00-06	615.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7758ca50-6466-4c55-9414-24018b72fd3b	c1ee2b83-1a2d-405e-9530-669fac3cf02a	a0eed33f-16af-4cf7-a332-4a7335323c03	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-08 07:00:00-06	2057.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
77a072b5-da65-4933-9486-20fb3519af21	6b7f3660-df40-49fb-894e-31187ae75007	2b95e099-6abf-43c4-ae92-9dd41351dde7	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-07 08:00:00-06	463.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
77c06416-9cf5-465f-bcbf-501cca9ba697	6b7f3660-df40-49fb-894e-31187ae75007	839924a4-7be3-4512-a079-4bc2db5cc4ed	1709cadd-2a88-48cd-bc22-344c9b2949cf	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-07 09:00:00-06	737.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
787b2ff6-cad1-4775-9902-0702bc82ab24	6b7f3660-df40-49fb-894e-31187ae75007	23e05f3e-f76d-43fd-8406-0f1fd143f59c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 08:00:00-06	314.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
78e16d84-b110-4f7d-9b0b-16524aa40313	c2396d46-e3e7-4690-912b-a9d2b1768562	6f5eb4f8-d810-4feb-ba87-50ff97277a8a	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-29 11:00:00-06	1249.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
79ab5937-42c6-4fb5-9f73-fa442713ccd6	6b7f3660-df40-49fb-894e-31187ae75007	c1151e10-65f5-4bbe-b6eb-818d0528444d	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-04 09:00:00-06	570.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7c2e336d-7130-4319-b348-539df148951a	c2396d46-e3e7-4690-912b-a9d2b1768562	0a770914-95e7-4d16-8aa0-30f4984c2df6	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-02 02:00:00-06	2355.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7d524e39-c013-47a0-a988-ecf99adcca65	6b7f3660-df40-49fb-894e-31187ae75007	9a65ff54-2df7-412f-b306-1eb8565d392f	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-04 04:00:00-06	531.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7f35f08b-23a0-4f93-a89a-6e19469526d2	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	23e05f3e-f76d-43fd-8406-0f1fd143f59c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 08:00:00-06	983.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
80fbeb5a-e7fd-42e3-90d4-bf2a68198cdd	c2396d46-e3e7-4690-912b-a9d2b1768562	d983083a-fdb5-4fc2-acfb-85ea6549f4e7	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-23 07:00:00-06	2072.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
813a9fe0-00e5-4595-a278-0fbf4fcda9ae	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	35dcc603-9902-4d26-acf6-5d7fab7c81ba	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-19 03:00:00-06	469.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
83bd7fa1-370f-45e1-959b-3f2ccc6f2590	6b7f3660-df40-49fb-894e-31187ae75007	4a1c287c-7be1-49b7-84b0-bd0cd4f84037	8f3ca495-c031-43be-88f9-403c2813e1d7	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-28 05:00:00-06	364.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
83ed888f-8565-4a57-baf9-81f4f3e98ce0	c1ee2b83-1a2d-405e-9530-669fac3cf02a	66effb93-a98b-4d83-a73e-dbc6713eced6	8f3ca495-c031-43be-88f9-403c2813e1d7	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-07 04:00:00-06	1402.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
859529d8-6138-4d39-8592-679ca0643ff7	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	b0c91b64-30c3-476f-b6d9-5231727c1e55	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-16 03:00:00-06	621.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8625c941-83aa-4637-ad1b-06583be1dc2d	c1ee2b83-1a2d-405e-9530-669fac3cf02a	8031560b-18d7-4f4b-a6b5-8db0e0a44e2c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-06 06:00:00-06	2786.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
86543703-610e-433c-b9b4-c42ec6ed5be6	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	a0eed33f-16af-4cf7-a332-4a7335323c03	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-08 07:00:00-06	913.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8867ac92-e235-4d0c-a233-901fdacc3f3e	6b7f3660-df40-49fb-894e-31187ae75007	f31876fa-328e-4f8e-a998-b48296296a55	aa074c2c-fe5a-442a-a6ed-669d6739062f	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-18 05:00:00-06	715.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8a93b4c9-baa8-49fc-aeed-9f4acc047c87	c2396d46-e3e7-4690-912b-a9d2b1768562	1f0496f2-8b3d-40ba-8ff5-5301c453693a	1709cadd-2a88-48cd-bc22-344c9b2949cf	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-17 03:00:00-06	1984.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8b64a5aa-607f-4664-8b14-fe2cabc21dcf	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	9c687700-3226-424c-b7e5-a41b38d407f6	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-08 11:00:00-06	424.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8bcec582-9b1a-4d1a-ab74-ec2585b131bb	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	ea6385c1-81ea-43a1-979a-c8753287d742	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-23 05:00:00-06	1104.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8d6c1d41-9b34-44e8-ae13-5965be81b5a5	c1ee2b83-1a2d-405e-9530-669fac3cf02a	839924a4-7be3-4512-a079-4bc2db5cc4ed	1709cadd-2a88-48cd-bc22-344c9b2949cf	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-07 09:00:00-06	3191.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8e49b681-8cfc-4f4a-b59a-24d382fb6708	c2396d46-e3e7-4690-912b-a9d2b1768562	5c115bbe-ec90-40d9-8512-13696bdc6a22	8f3ca495-c031-43be-88f9-403c2813e1d7	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-05 05:00:00-06	905.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
90f182c9-7aac-4194-941c-32fb8e147b96	6b7f3660-df40-49fb-894e-31187ae75007	9e76936b-bb57-45c0-9b19-c0f5ab16cfe4	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-19 02:00:00-06	343.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9282acfc-839b-40db-8928-e7e6e92c55b0	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	7400c6aa-8a44-4a34-b76b-4824038a790d	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-20 05:00:00-06	1097.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
950c0001-c1ea-4e25-8c38-bde49f2ec6d6	c1ee2b83-1a2d-405e-9530-669fac3cf02a	ee7916ec-1c38-4869-8329-87906bcac270	1709cadd-2a88-48cd-bc22-344c9b2949cf	705280e1-81dd-4793-b352-fbf5593d35b8	2026-06-22 10:00:00-06	2859.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
95cf1b43-cfbd-47eb-a765-9d61075a41c2	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	c1151e10-65f5-4bbe-b6eb-818d0528444d	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-04 09:00:00-06	626.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
968fa6ad-44c9-4d7d-b084-0de7d8c2c547	c2396d46-e3e7-4690-912b-a9d2b1768562	f8b75f3f-f8c4-4ee6-ba82-82ef98204dbb	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-06 11:00:00-06	1802.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9709f8b1-7fc9-44a1-9708-398e22afe224	6b7f3660-df40-49fb-894e-31187ae75007	eedc9484-c795-499d-ad23-4be0a563a8ee	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-25 08:00:00-06	493.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9796a944-cd86-420d-8f89-d471f25e9fe9	c1ee2b83-1a2d-405e-9530-669fac3cf02a	35dcc603-9902-4d26-acf6-5d7fab7c81ba	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-19 03:00:00-06	1547.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
99c04717-1903-4885-85db-51ced5379619	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	5819ad28-5367-4907-86f9-1c323d6dfef7	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-01 08:00:00-06	920.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9bf946c6-e0dd-4b12-87cd-acb129e144ce	6b7f3660-df40-49fb-894e-31187ae75007	1bfe8a75-c931-40be-8108-0bf80ceb02a4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-06-26 10:00:00-06	844.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9c990bc9-8d24-47d6-8409-5105e6abd608	c1ee2b83-1a2d-405e-9530-669fac3cf02a	a28f3acc-667c-4b32-913a-992935f08655	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-13 02:00:00-06	2058.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9ef4b4b4-6eb5-4ee0-8ab2-e348b090282b	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	b09bf7b8-3d49-42ef-9f3a-c0b63015ff79	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-11 11:00:00-06	1126.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9f7a6b5f-486b-4081-be3e-c7cdabb4e505	c1ee2b83-1a2d-405e-9530-669fac3cf02a	72a70e91-36f1-466a-8ba4-32606cc0d679	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-03 05:00:00-06	3232.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a18c0e0a-b8a8-4153-b96e-61e2dbbf6518	6b7f3660-df40-49fb-894e-31187ae75007	16cc60f9-9a3f-41df-aa60-f62a45a23905	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-22 05:00:00-06	874.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a25e21e6-3972-4d87-bcaf-fd8c408c364e	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	1a0bca72-0d5e-4e73-b1dd-04efff6b70e7	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-23 04:00:00-06	967.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a2adfc80-2552-4f01-b539-6bcc5535f823	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	cc642552-a331-439c-8eae-1e7691304214	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-27 09:00:00-06	797.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a428b4a8-e336-480b-9f2f-05057e5db018	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	1bfe8a75-c931-40be-8108-0bf80ceb02a4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-06-26 10:00:00-06	491.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a64cf207-0565-42a9-8f3f-bc5c4dfcfca6	c1ee2b83-1a2d-405e-9530-669fac3cf02a	6e0b3337-4dd1-4973-9a4b-bf9752ac7422	1709cadd-2a88-48cd-bc22-344c9b2949cf	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-02 03:00:00-06	3373.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a721f15a-91b7-4dd9-9bc4-0fbafa619a41	c2396d46-e3e7-4690-912b-a9d2b1768562	9c687700-3226-424c-b7e5-a41b38d407f6	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-08 11:00:00-06	1403.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a84fe005-d053-422d-9df8-7a0f4f05c591	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	e525281b-8d5b-4551-8c53-4816c5099a08	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-16 03:00:00-06	690.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a8c00921-2507-4007-b9e5-42708c478589	c1ee2b83-1a2d-405e-9530-669fac3cf02a	5e586a5b-cae0-4c15-a223-84a4530423e8	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-14 09:00:00-06	1481.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a920d0c1-461c-4019-9729-92974b1d1898	c2396d46-e3e7-4690-912b-a9d2b1768562	eedc9484-c795-499d-ad23-4be0a563a8ee	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-25 08:00:00-06	1788.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a96e6e93-ee70-4e41-b3f3-fb69ecc8a656	c1ee2b83-1a2d-405e-9530-669fac3cf02a	b09bf7b8-3d49-42ef-9f3a-c0b63015ff79	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-11 11:00:00-06	2279.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a9ee570c-1208-4f67-9daf-115879eba680	c1ee2b83-1a2d-405e-9530-669fac3cf02a	305c36be-2652-4f7d-b3d2-e798afe40c17	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-15 04:00:00-06	3011.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
aa46dd75-aba0-4672-9937-f0e40e97460b	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	5e586a5b-cae0-4c15-a223-84a4530423e8	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-14 09:00:00-06	401.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ad8d0328-abb5-4ec2-8c20-75562cf45579	c2396d46-e3e7-4690-912b-a9d2b1768562	1bfe8a75-c931-40be-8108-0bf80ceb02a4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-06-26 10:00:00-06	2579.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
af483804-69bb-4a73-b869-d9a95dbd1547	6b7f3660-df40-49fb-894e-31187ae75007	6315e558-0e5d-4f28-88f4-2de23f9584e4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-21 05:00:00-06	479.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b0454659-1f2a-4c55-b7f8-e38059d10b71	c2396d46-e3e7-4690-912b-a9d2b1768562	cf783a12-df74-4a45-96d9-ac1c812bb7d3	aa074c2c-fe5a-442a-a6ed-669d6739062f	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-22 06:00:00-06	2338.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b21a375a-aef7-4f62-adb9-c71b7872f24e	7e81e63e-f6f7-4737-958c-7476884bcca7	bb3defaa-2e0c-42a1-af26-ae085813c47b	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-28 04:00:00-06	504.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b2e162dd-c2c0-47df-807c-6f2774dc893b	6b7f3660-df40-49fb-894e-31187ae75007	a739ea38-9105-4b31-abae-107484078d4a	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 11:00:00-06	437.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b63638d4-de2f-49d3-baba-43a22e71bcc2	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	245d4afd-7294-4492-9757-c2948b7ec3b0	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-30 03:00:00-06	435.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b6a96802-e960-4a67-afc0-3454f7a904c5	c2396d46-e3e7-4690-912b-a9d2b1768562	5a6f40e7-b3f1-4d7f-acf2-d3a6b3bdf748	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-05 23:41:37.744096-06	921.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b702bbff-c07a-4776-be81-7922aaf3cc32	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	fbbf011f-bb9d-4147-9fd9-a34ac87d48ba	1709cadd-2a88-48cd-bc22-344c9b2949cf	705280e1-81dd-4793-b352-fbf5593d35b8	2026-06-22 03:00:00-06	830.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b9479471-d87b-4eb9-82bb-7fba8434e3c7	c2396d46-e3e7-4690-912b-a9d2b1768562	b09bf7b8-3d49-42ef-9f3a-c0b63015ff79	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-11 11:00:00-06	1681.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ba37902b-0f7f-489d-8167-5d1b92d74e8e	c2396d46-e3e7-4690-912b-a9d2b1768562	9e76936b-bb57-45c0-9b19-c0f5ab16cfe4	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-19 02:00:00-06	833.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bc5a0a31-da7a-4a2c-95cd-24764eb4236d	c1ee2b83-1a2d-405e-9530-669fac3cf02a	9042d857-6baa-4313-a620-99c58939b5d3	8f3ca495-c031-43be-88f9-403c2813e1d7	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-24 02:00:00-06	567.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bd3882a0-755f-4682-b3bc-eabea5949903	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	0a84b59a-f3fe-4a5d-b34d-9b400748921f	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-28 09:00:00-06	509.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bd624585-9acb-4cea-8053-3529c7830c52	c1ee2b83-1a2d-405e-9530-669fac3cf02a	23974bf1-2f32-465d-93c9-168327cf7779	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-06 11:00:00-06	2914.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
be05b245-2211-4443-b01c-571f5009dab9	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	6dbfefa7-101c-46ca-9786-28a3adb235f6	8f3ca495-c031-43be-88f9-403c2813e1d7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-11 03:00:00-06	960.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bfb1a772-de72-4767-8c79-5e32a3b1eb29	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	a28f3acc-667c-4b32-913a-992935f08655	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-13 02:00:00-06	710.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c08652ab-b545-45a9-9dcd-72cf964c7b69	c2396d46-e3e7-4690-912b-a9d2b1768562	97c2ff14-f188-4b2a-9155-87c99fa6c70b	8f3ca495-c031-43be-88f9-403c2813e1d7	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-20 11:00:00-06	740.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c12cb15c-20cb-4ac9-99ee-bac347d6b5d0	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	5ccd65ec-1fda-42cf-84f0-73edee47802e	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-24 05:00:00-06	622.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c3cf4f74-6888-450d-a3e4-f9a0d22b2161	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	aa9c84e1-b07f-46cd-bfcb-014dbc1964f7	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-13 09:00:00-06	924.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c42fd1cc-8f9c-4f58-9523-c234c7e4b99c	c1ee2b83-1a2d-405e-9530-669fac3cf02a	4a76544d-2b35-47ca-85bf-afbbe82a7552	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-02 06:00:00-06	1945.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c55ea2c6-acda-45d7-a614-c460ec5fadcd	6b7f3660-df40-49fb-894e-31187ae75007	54f2ed83-df52-4d5e-9ed6-1cda3cd14d89	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-29 03:00:00-06	505.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c69566ed-38cc-488c-a56d-a60acd14134d	7e81e63e-f6f7-4737-958c-7476884bcca7	121872a6-0a62-42e5-bcdf-fda1bcf97dda	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-18 07:00:00-06	620.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c7730b6c-5bc6-4c5b-9330-f1001b63209c	c2396d46-e3e7-4690-912b-a9d2b1768562	65111110-c9a7-4096-a902-4c3d1ce62319	1709cadd-2a88-48cd-bc22-344c9b2949cf	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-14 05:00:00-06	1723.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c83b8191-b9a6-4178-88b0-f93b941071ee	6b7f3660-df40-49fb-894e-31187ae75007	ca0904ab-3166-43df-bb4a-1793791426f5	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-08-04 03:00:00-06	678.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c8d54cdc-fb15-4d23-af03-da9b33fc522a	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	97c2ff14-f188-4b2a-9155-87c99fa6c70b	8f3ca495-c031-43be-88f9-403c2813e1d7	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-20 11:00:00-06	502.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
cb46b843-f0e2-49c4-8dff-df167e0281ac	7e81e63e-f6f7-4737-958c-7476884bcca7	fa62a42d-22de-439b-9c01-7a3f3a67886b	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-30 08:00:00-06	393.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
cb772318-4070-4e5e-b9cb-df98ae1a3c6a	c1ee2b83-1a2d-405e-9530-669fac3cf02a	d17b5514-b2b8-4600-89b5-db0c7589fcda	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-01 02:00:00-06	3200.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
cd656fff-5a8f-4cf7-8805-69ab5d110f77	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	3d525c86-2765-4b0e-930e-e9beeee1abd7	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-05 21:41:37.744096-06	557.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
cdc0ccbc-bdb3-4782-97a1-079f6b431fca	6b7f3660-df40-49fb-894e-31187ae75007	bb3defaa-2e0c-42a1-af26-ae085813c47b	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-28 04:00:00-06	300.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d0241a4c-d6aa-480b-bd32-5b4e84d11ee2	c1ee2b83-1a2d-405e-9530-669fac3cf02a	5a6f40e7-b3f1-4d7f-acf2-d3a6b3bdf748	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-05 23:41:37.744096-06	657.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d2140089-3cbe-418b-8586-b7b717ee08f4	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	0a770914-95e7-4d16-8aa0-30f4984c2df6	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-02 02:00:00-06	1137.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d2449c2a-4293-424a-8b0f-73a5c0a7b4f0	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	e0ceaa50-1da7-4544-8212-86f6fd3a35c8	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-24 05:00:00-06	799.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d54dd849-62f9-4ad5-b6b8-e41afaa2b834	7e81e63e-f6f7-4737-958c-7476884bcca7	6b93721a-c050-4375-b083-987cfd95a4eb	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 06:00:00-06	408.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d6fd948e-6d71-48f8-8fe9-a1faa04b4bd9	6b7f3660-df40-49fb-894e-31187ae75007	e0ceaa50-1da7-4544-8212-86f6fd3a35c8	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-24 05:00:00-06	688.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
dae337c4-c2ac-4bb6-b93f-15a6d18eca7c	7e81e63e-f6f7-4737-958c-7476884bcca7	6dbfefa7-101c-46ca-9786-28a3adb235f6	8f3ca495-c031-43be-88f9-403c2813e1d7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-11 03:00:00-06	774.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
db5287da-490f-4d88-ac97-d67255afe634	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	8ce37e6d-ab47-4eba-a3e1-150bd6ceefa2	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-26 02:00:00-06	810.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
dbafc6c1-76cb-46ec-a9ae-5bcd4b9e67a8	6b7f3660-df40-49fb-894e-31187ae75007	7d03c213-67ac-4b36-ae23-3043682f983a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-27 02:00:00-06	866.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
dedede89-d163-4954-9f87-9fe2d8ff26d3	c2396d46-e3e7-4690-912b-a9d2b1768562	c1151e10-65f5-4bbe-b6eb-818d0528444d	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-04 09:00:00-06	2293.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
dff90753-af2d-4ad3-8026-cfeaddc78e9d	c2396d46-e3e7-4690-912b-a9d2b1768562	a4a9db81-97c6-47f4-a683-10e1fa576dd4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-26 08:00:00-06	1590.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e032d1e1-5bd3-40f2-8c7a-b811496a4057	7e81e63e-f6f7-4737-958c-7476884bcca7	16cc60f9-9a3f-41df-aa60-f62a45a23905	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-22 05:00:00-06	384.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e03e38f4-3377-4f4a-9702-87dc520f1294	c2396d46-e3e7-4690-912b-a9d2b1768562	589a9654-8a1d-43d7-8d25-614161e2e099	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-27 06:00:00-06	776.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e0c3d313-ba9b-4e4f-9922-d3b3e54840d2	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	589a9654-8a1d-43d7-8d25-614161e2e099	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-27 06:00:00-06	956.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e12f56c9-7f20-4d51-813e-fae4bbece237	6b7f3660-df40-49fb-894e-31187ae75007	f8ddeedb-3b8a-41df-bf00-1560c4864863	aa074c2c-fe5a-442a-a6ed-669d6739062f	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-12 03:00:00-06	567.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e1809f91-a9eb-430c-94f0-71454c182c56	6b7f3660-df40-49fb-894e-31187ae75007	72a70e91-36f1-466a-8ba4-32606cc0d679	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-03 05:00:00-06	578.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e220c60b-4a68-4622-9010-99392842c569	c2396d46-e3e7-4690-912b-a9d2b1768562	868be146-c911-4415-94c5-faf6ec1fa3ab	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-15 07:00:00-06	703.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e234363d-2a1a-42ed-8fda-0bdd95496e41	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	5a6f40e7-b3f1-4d7f-acf2-d3a6b3bdf748	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-05 23:41:37.744096-06	657.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e286a533-bc75-4d38-9391-a4ba6630baba	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	12021d76-8cba-4893-ad95-086812a7a00b	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-06-23 06:00:00-06	1159.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e42aa63f-c46f-49a3-9279-1a4be6bfae92	c1ee2b83-1a2d-405e-9530-669fac3cf02a	f8b75f3f-f8c4-4ee6-ba82-82ef98204dbb	aa074c2c-fe5a-442a-a6ed-669d6739062f	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-06 11:00:00-06	2973.00	Refacciones y llantas	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e5ca35dd-fa46-4375-a4e3-f7c24effd2d5	6b7f3660-df40-49fb-894e-31187ae75007	6e0b3337-4dd1-4973-9a4b-bf9752ac7422	1709cadd-2a88-48cd-bc22-344c9b2949cf	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-02 03:00:00-06	895.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e6629292-fb4f-4711-9c2f-6c3bf79e2e19	6b7f3660-df40-49fb-894e-31187ae75007	7546fdce-398a-4265-b9a7-92db4ea57cf8	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-21 09:00:00-06	647.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e6f6b15b-188e-4deb-be9e-8c6556110a2f	c2396d46-e3e7-4690-912b-a9d2b1768562	f1b104dd-94ee-4a42-b120-adbf0779abf2	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-11 05:00:00-06	1142.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e8ae303f-91fe-455b-a635-b60db7ba795a	c2396d46-e3e7-4690-912b-a9d2b1768562	66effb93-a98b-4d83-a73e-dbc6713eced6	8f3ca495-c031-43be-88f9-403c2813e1d7	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-07 04:00:00-06	1778.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
eaf4a58c-d087-494d-845d-a40865484c54	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	3c972500-11d1-48df-bb21-8ddfff247b11	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-02 03:00:00-06	1056.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
eb4f7434-f1d7-455a-b28e-c94c97de3c5f	7e81e63e-f6f7-4737-958c-7476884bcca7	e525281b-8d5b-4551-8c53-4816c5099a08	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-16 03:00:00-06	403.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ecccde41-5cc6-4afa-a916-bd0becbf5b3e	c2396d46-e3e7-4690-912b-a9d2b1768562	0a84b59a-f3fe-4a5d-b34d-9b400748921f	aa074c2c-fe5a-442a-a6ed-669d6739062f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-28 09:00:00-06	2108.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ef66d002-7d84-438b-b218-1809aa20de75	c2396d46-e3e7-4690-912b-a9d2b1768562	6315e558-0e5d-4f28-88f4-2de23f9584e4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-21 05:00:00-06	1288.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f12bfa0c-b9f6-4356-b17e-116b7409a1a6	c2396d46-e3e7-4690-912b-a9d2b1768562	1bdeb0d1-f0f6-4770-baa9-7e0ca417336f	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-27 05:00:00-06	1190.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f2dfe6e0-830d-49c1-8377-f5deb4c1d7fe	6b7f3660-df40-49fb-894e-31187ae75007	589a9654-8a1d-43d7-8d25-614161e2e099	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-27 06:00:00-06	611.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f2f14c32-31ed-4c81-b29e-1711a90a60a2	6b7f3660-df40-49fb-894e-31187ae75007	fa62a42d-22de-439b-9c01-7a3f3a67886b	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-30 08:00:00-06	405.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f3e59c57-2e7a-437e-b5ac-daad7d6ee880	c2396d46-e3e7-4690-912b-a9d2b1768562	6b93721a-c050-4375-b083-987cfd95a4eb	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 06:00:00-06	2544.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f6ef57d1-a54c-4f57-a027-57e09e0fb211	c2396d46-e3e7-4690-912b-a9d2b1768562	3ed14117-4757-4269-8790-b7b2c982052f	8f3ca495-c031-43be-88f9-403c2813e1d7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-03 10:00:00-06	1226.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f77e338b-0100-4447-b1b0-da492f19c2c3	c2396d46-e3e7-4690-912b-a9d2b1768562	245d4afd-7294-4492-9757-c2948b7ec3b0	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-30 03:00:00-06	2549.00	Casetas y peajes	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f89b44bf-b07a-4766-b7c7-38d920a0f945	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	f1b104dd-94ee-4a42-b120-adbf0779abf2	1709cadd-2a88-48cd-bc22-344c9b2949cf	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-11 05:00:00-06	888.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f95ae9c7-a00d-4c25-91f4-6bcd11f41fd4	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	1f0496f2-8b3d-40ba-8ff5-5301c453693a	1709cadd-2a88-48cd-bc22-344c9b2949cf	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-17 03:00:00-06	711.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
faa40bb9-c39c-4c91-8ca3-6ef78462c3e0	6b7f3660-df40-49fb-894e-31187ae75007	121872a6-0a62-42e5-bcdf-fda1bcf97dda	8f3ca495-c031-43be-88f9-403c2813e1d7	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-18 07:00:00-06	760.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fb5585de-88ea-4db2-8b62-78a93419b313	6b7f3660-df40-49fb-894e-31187ae75007	a4a9db81-97c6-47f4-a683-10e1fa576dd4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-26 08:00:00-06	708.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fc7b14c1-7f9f-4132-ba8b-3e8e8d4bc27c	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	6315e558-0e5d-4f28-88f4-2de23f9584e4	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-21 05:00:00-06	869.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fd064ff0-4537-4f30-a02c-f4895da8dfde	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	55ffc6b7-8949-4403-85fc-cc113dd908cb	8f3ca495-c031-43be-88f9-403c2813e1d7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-22 09:00:00-06	564.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fd23dc42-572a-4454-be33-4a2e847100d4	6b7f3660-df40-49fb-894e-31187ae75007	9c687700-3226-424c-b7e5-a41b38d407f6	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-08 11:00:00-06	555.00	Maniobras de carga y descarga	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fec744f3-c62c-4940-93f1-40cd88ee968d	7e81e63e-f6f7-4737-958c-7476884bcca7	fbbf011f-bb9d-4147-9fd9-a34ac87d48ba	1709cadd-2a88-48cd-bc22-344c9b2949cf	705280e1-81dd-4793-b352-fbf5593d35b8	2026-06-22 03:00:00-06	772.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ffc3ff7f-c5f7-4f3b-b9b0-ceebb9976085	7e81e63e-f6f7-4737-958c-7476884bcca7	9f0202ef-2f4d-4504-87bf-b301512a6763	aa074c2c-fe5a-442a-a6ed-669d6739062f	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 10:00:00-06	643.00	Multas e infracciones	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ffd57b51-996e-4011-8a15-0c6fee081a8d	6add2a11-8a09-4c1c-8edc-9d5d1c439d3b	72a70e91-36f1-466a-8ba4-32606cc0d679	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-03 05:00:00-06	684.00	Viáticos del operador	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
\.


--
-- Data for Name: fuel_logs; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.fuel_logs ("Id", "VehicleId", "TripId", "DriverId", "LoadedAtUtc", "Quantity", "PricePerUnit", "TotalCost", "OdometerReading", "Station", "ReferenceNumber", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
047519a9-3a7a-43f6-83be-d2c7b70eea1b	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	23974bf1-2f32-465d-93c9-168327cf7779	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-06 10:00:00-06	358.000	25.9600	9293.68	144575.05	Estación Pemex km 120	TICKET-18443	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0a3ddb17-28e3-45f0-bb33-46799bccea4b	aa074c2c-fe5a-442a-a6ed-669d6739062f	d17b5514-b2b8-4600-89b5-db0c7589fcda	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-08-01 01:00:00-06	379.000	24.5700	9312.03	523925.95	Estación Pemex km 120	TICKET-98337	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0ec1d7ae-8c62-46c2-b57e-57230bbbce1d	1709cadd-2a88-48cd-bc22-344c9b2949cf	1f0496f2-8b3d-40ba-8ff5-5301c453693a	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-17 02:00:00-06	204.000	25.6900	5240.76	202585.87	Estación Pemex km 120	TICKET-97019	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
11cc8fef-5e24-4e7d-8260-5a21cc7403cb	8f3ca495-c031-43be-88f9-403c2813e1d7	b678f560-3801-4679-bc9f-0cdd1a8e3c41	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-01 03:00:00-06	341.000	26.2300	8944.43	77596.65	Estación Pemex km 120	TICKET-49517	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
12829e3b-6d83-4cee-8b61-a606dbbac229	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	fa62a42d-22de-439b-9c01-7a3f3a67886b	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-30 07:00:00-06	232.000	27.3000	6333.60	143222.82	Estación Pemex km 120	TICKET-30525	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
13f4c4c8-583d-4181-95b6-ed2611575663	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	aa9c84e1-b07f-46cd-bfcb-014dbc1964f7	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-13 08:00:00-06	193.000	26.5000	5114.50	387867.53	Estación Pemex km 120	TICKET-27768	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
16c1e4cf-d134-468d-85ed-7bbbd071b407	8f3ca495-c031-43be-88f9-403c2813e1d7	a739ea38-9105-4b31-abae-107484078d4a	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 10:00:00-06	372.000	26.3900	9817.08	85621.40	Estación Pemex km 120	TICKET-85383	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
16f2079b-74a4-4bc7-9669-471230882ab2	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	f4a8b079-d294-4931-be1c-1f7ca3bc0cda	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-05 04:00:00-06	199.000	25.4800	5070.52	396300.73	Estación Pemex km 120	TICKET-99867	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1b86942d-d88f-4140-90db-0d82c9fc61a7	8f3ca495-c031-43be-88f9-403c2813e1d7	31c2aee8-9f27-47a4-a98d-4fe01c3eef05	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-28 07:00:00-06	396.000	24.4700	9690.12	86524.47	Estación Pemex km 120	TICKET-89165	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1c79a0dc-a2e1-418b-8016-10e72745589d	aa074c2c-fe5a-442a-a6ed-669d6739062f	cc642552-a331-439c-8eae-1e7691304214	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-27 08:00:00-06	345.000	25.6000	8832.00	522078.70	Estación Pemex km 120	TICKET-84065	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1f29c91b-ca0f-4047-9ecb-56642f0dd2d8	aa074c2c-fe5a-442a-a6ed-669d6739062f	f8b75f3f-f8c4-4ee6-ba82-82ef98204dbb	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-06 10:00:00-06	192.000	24.7200	4746.24	514913.15	Estación Pemex km 120	TICKET-61649	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
26ccd6c6-f5fa-4a9e-95ff-a776f89fdddf	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	245d4afd-7294-4492-9757-c2948b7ec3b0	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-30 02:00:00-06	397.000	24.6700	9793.99	386475.48	Estación Pemex km 120	TICKET-11928	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
297c1000-8812-42b6-aa0b-4f50cf86c28e	8f3ca495-c031-43be-88f9-403c2813e1d7	4a1c287c-7be1-49b7-84b0-bd0cd4f84037	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-28 04:00:00-06	329.000	25.7600	8475.04	76840.12	Estación Pemex km 120	TICKET-40943	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2a821b78-84b4-4240-973d-f689bd3ef959	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	ca0904ab-3166-43df-bb4a-1793791426f5	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-08-04 02:00:00-06	242.000	25.0500	6062.10	149725.28	Estación Pemex km 120	TICKET-93037	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
31c98edd-774f-477d-9d55-2c910a8cf2ca	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	e0ceaa50-1da7-4544-8212-86f6fd3a35c8	86e72142-66cc-461b-94e0-f5daff605e4f	2026-06-24 04:00:00-06	346.000	27.1300	9386.98	141380.25	Estación Pemex km 120	TICKET-61480	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
31e969d7-9af3-44bc-9e7e-ceb075cb695e	1709cadd-2a88-48cd-bc22-344c9b2949cf	6e0b3337-4dd1-4973-9a4b-bf9752ac7422	705280e1-81dd-4793-b352-fbf5593d35b8	2026-08-02 02:00:00-06	350.000	26.6200	9317.00	206996.40	Estación Pemex km 120	TICKET-47030	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3281bc1a-b986-44bb-a64f-49a6580ee9f8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	99e79f0d-bee9-4643-8a1a-77ba58adcfa0	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-19 02:00:00-06	388.000	24.4400	9482.72	389586.40	Estación Pemex km 120	TICKET-51898	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3840f786-7041-406c-aed9-83906850c2b0	1709cadd-2a88-48cd-bc22-344c9b2949cf	54f2ed83-df52-4d5e-9ed6-1cda3cd14d89	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-29 02:00:00-06	265.000	25.7200	6815.80	204529.05	Estación Pemex km 120	TICKET-62169	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3b9450be-1bbe-4bcc-87f8-f3379b5a8a45	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	6f5eb4f8-d810-4feb-ba87-50ff97277a8a	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-29 10:00:00-06	216.000	24.6500	5324.40	142723.27	Estación Pemex km 120	TICKET-16057	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3f25016e-0f2c-4eb0-a048-c6d7dc9ad468	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	9c687700-3226-424c-b7e5-a41b38d407f6	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-08 10:00:00-06	272.000	27.2600	7414.72	146171.12	Estación Pemex km 120	TICKET-51377	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
451eba07-67f6-4a9b-87ca-4ad7489424e0	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	12021d76-8cba-4893-ad95-086812a7a00b	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-06-23 05:00:00-06	216.000	26.7600	5780.16	384709.07	Estación Pemex km 120	TICKET-71624	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
47d92159-2749-432e-a522-c5071dfcda09	aa074c2c-fe5a-442a-a6ed-669d6739062f	35dcc603-9902-4d26-acf6-5d7fab7c81ba	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-19 02:00:00-06	377.000	25.7200	9696.44	518403.88	Estación Pemex km 120	TICKET-96889	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
48b290f0-e55d-4dae-8b46-f9a7494ba410	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	1bdeb0d1-f0f6-4770-baa9-7e0ca417336f	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-27 04:00:00-06	193.000	25.4600	4913.78	393951.80	Estación Pemex km 120	TICKET-47898	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
501a3014-7d71-4467-9f4f-8da24c6ab9dc	aa074c2c-fe5a-442a-a6ed-669d6739062f	1957d298-0fa7-4a01-ae83-294864f8fe40	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-30 04:00:00-06	249.000	24.5300	6107.97	522753.97	Estación Pemex km 120	TICKET-91577	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
51616da7-8e1b-464b-9072-845dc0d74d94	8f3ca495-c031-43be-88f9-403c2813e1d7	18870d3c-5de0-478a-a60b-8c2da843ee33	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-09 06:00:00-06	241.000	25.3200	6102.12	80552.52	Estación Pemex km 120	TICKET-23619	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5438211b-01d0-44e4-8124-b8e7c8de748b	aa074c2c-fe5a-442a-a6ed-669d6739062f	ea6385c1-81ea-43a1-979a-c8753287d742	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-23 04:00:00-06	357.000	26.6500	9514.05	520486.90	Estación Pemex km 120	TICKET-25096	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
54d36c3f-8859-4ea0-b03a-20c840115f3c	1709cadd-2a88-48cd-bc22-344c9b2949cf	65111110-c9a7-4096-a902-4c3d1ce62319	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-14 04:00:00-06	391.000	26.8100	10482.71	201899.40	Estación Pemex km 120	TICKET-99633	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5760a054-d9bd-48fe-90f9-c4c06bb07fe3	1709cadd-2a88-48cd-bc22-344c9b2949cf	4d1fc37d-7b1f-4b37-9230-b52209285833	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-31 04:00:00-06	250.000	25.5300	6382.50	205520.52	Estación Pemex km 120	TICKET-12462	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5bf8df4c-7a28-4e8c-bb73-56da11d801aa	aa074c2c-fe5a-442a-a6ed-669d6739062f	0a84b59a-f3fe-4a5d-b34d-9b400748921f	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-28 08:00:00-06	273.000	27.1600	7414.68	513560.92	Estación Pemex km 120	TICKET-10579	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5e46ab8a-ef5b-4b46-ab30-b199b37cbfac	1709cadd-2a88-48cd-bc22-344c9b2949cf	839924a4-7be3-4512-a079-4bc2db5cc4ed	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-07 08:00:00-06	268.000	26.0300	6976.04	200468.40	Estación Pemex km 120	TICKET-92673	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6211928e-fc0b-447e-8121-712a162330e9	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	b0c91b64-30c3-476f-b6d9-5231727c1e55	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-16 02:00:00-06	281.000	25.7600	7238.56	148033.33	Estación Pemex km 120	TICKET-47912	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
649bdb26-b413-4a28-af0d-f12b2963d8d4	aa074c2c-fe5a-442a-a6ed-669d6739062f	a28f3acc-667c-4b32-913a-992935f08655	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-13 01:00:00-06	263.000	26.0100	6840.63	516371.55	Estación Pemex km 120	TICKET-49748	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
79c8f136-a49b-4dd9-83ec-381555149155	8f3ca495-c031-43be-88f9-403c2813e1d7	121872a6-0a62-42e5-bcdf-fda1bcf97dda	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-18 06:00:00-06	348.000	27.3300	9510.84	82672.12	Estación Pemex km 120	TICKET-88151	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
811ad748-09d6-4d64-81bf-2eb38f5d00a9	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	b09bf7b8-3d49-42ef-9f3a-c0b63015ff79	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-11 10:00:00-06	274.000	26.5400	7271.96	146886.88	Estación Pemex km 120	TICKET-80601	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
866d6a8b-7945-46a5-b8ab-14022cfde728	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	6315e558-0e5d-4f28-88f4-2de23f9584e4	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-21 04:00:00-06	230.000	26.0300	5986.90	390379.40	Estación Pemex km 120	TICKET-38108	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8849b5f0-b002-43f1-bf07-18750ced4c35	8f3ca495-c031-43be-88f9-403c2813e1d7	61959438-3cc4-46a4-8ed1-e8589e24d4a2	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-25 07:00:00-06	190.000	24.5900	4672.10	84807.60	Estación Pemex km 120	TICKET-69027	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
93936f40-e77c-4530-b88c-57bd2a311455	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	0a770914-95e7-4d16-8aa0-30f4984c2df6	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-02 01:00:00-06	200.000	24.6200	4924.00	386980.18	Estación Pemex km 120	TICKET-39766	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
94f14813-bad7-43b1-b022-a8802deff971	aa074c2c-fe5a-442a-a6ed-669d6739062f	9f0202ef-2f4d-4504-87bf-b301512a6763	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 09:00:00-06	288.000	26.4900	7629.12	514103.02	Estación Pemex km 120	TICKET-22184	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a1961db5-92c6-4bc8-b7e5-d180c4245649	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	7d03c213-67ac-4b36-ae23-3043682f983a	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-06-27 01:00:00-06	341.000	25.0000	8525.00	385939.88	Estación Pemex km 120	TICKET-11976	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a7e955f4-d840-40b2-8d9b-8d69579572a0	1709cadd-2a88-48cd-bc22-344c9b2949cf	7400c6aa-8a44-4a34-b76b-4824038a790d	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-20 04:00:00-06	337.000	25.7400	8674.38	203106.02	Estación Pemex km 120	TICKET-34948	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
aff52ba5-6015-469d-92b1-5c1cacfa0fa1	aa074c2c-fe5a-442a-a6ed-669d6739062f	f31876fa-328e-4f8e-a998-b48296296a55	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-18 04:00:00-06	272.000	26.4400	7191.68	517459.35	Estación Pemex km 120	TICKET-77420	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b4ff1290-63b4-4882-9a69-36860e3b1713	aa074c2c-fe5a-442a-a6ed-669d6739062f	7546fdce-398a-4265-b9a7-92db4ea57cf8	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-21 08:00:00-06	384.000	26.5600	10199.04	519881.75	Estación Pemex km 120	TICKET-94803	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b76d2f8d-b79c-481c-8684-742141113d06	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	6b93721a-c050-4375-b083-987cfd95a4eb	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-26 05:00:00-06	212.000	24.4100	5174.92	392820.60	Estación Pemex km 120	TICKET-51349	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bb1cbb64-530a-4aea-b6fb-2bacf890593b	8f3ca495-c031-43be-88f9-403c2813e1d7	97c2ff14-f188-4b2a-9155-87c99fa6c70b	86e72142-66cc-461b-94e0-f5daff605e4f	2026-07-20 10:00:00-06	381.000	24.7000	9410.70	83532.12	Estación Pemex km 120	TICKET-33471	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bb65ba47-73ca-4d9f-ba49-f1121054affc	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	4a76544d-2b35-47ca-85bf-afbbe82a7552	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-02 05:00:00-06	370.000	24.4400	9042.80	395019.13	Estación Pemex km 120	TICKET-56602	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bba84ad0-59bb-4bea-bc07-37487511ab97	aa074c2c-fe5a-442a-a6ed-669d6739062f	9e76936b-bb57-45c0-9b19-c0f5ab16cfe4	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-19 01:00:00-06	367.000	26.8300	9846.61	519224.68	Estación Pemex km 120	TICKET-45292	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c17a199a-6d12-4d2c-8b79-cdfc03c70623	1709cadd-2a88-48cd-bc22-344c9b2949cf	3c972500-11d1-48df-bb21-8ddfff247b11	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-08-02 02:00:00-06	315.000	25.8500	8142.75	206425.63	Estación Pemex km 120	TICKET-45729	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
cb40e2ff-8deb-41d4-aa56-03c9e7ba97b9	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	bb3defaa-2e0c-42a1-af26-ae085813c47b	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-28 03:00:00-06	225.000	26.9200	6057.00	148865.22	Estación Pemex km 120	TICKET-59652	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d013429a-39c1-4044-b925-312ab290d9f3	8f3ca495-c031-43be-88f9-403c2813e1d7	55ffc6b7-8949-4403-85fc-cc113dd908cb	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-22 08:00:00-06	270.000	26.0600	7036.20	84168.90	Estación Pemex km 120	TICKET-56639	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d4dcb59b-c5b0-49f8-ad02-70ba72f8f2ec	aa074c2c-fe5a-442a-a6ed-669d6739062f	d983083a-fdb5-4fc2-acfb-85ea6549f4e7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-23 06:00:00-06	251.000	24.6000	6174.60	521055.07	Estación Pemex km 120	TICKET-96495	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d68d1bd6-7576-41a6-8ecb-2c91973831b8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	e11feb81-c505-40ce-8411-ad49e353cba9	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	2026-07-22 05:00:00-06	321.000	25.8200	8288.22	391284.60	Estación Pemex km 120	TICKET-51485	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d702fa65-99f4-4764-87c9-72049cc54d54	1709cadd-2a88-48cd-bc22-344c9b2949cf	1a0bca72-0d5e-4e73-b1dd-04efff6b70e7	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-23 03:00:00-06	277.000	25.8200	7152.14	203711.85	Estación Pemex km 120	TICKET-28995	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
da217c47-4ae0-4722-ad34-44be8a4bff00	aa074c2c-fe5a-442a-a6ed-669d6739062f	cf783a12-df74-4a45-96d9-ac1c812bb7d3	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-06-22 05:00:00-06	327.000	27.2300	8904.21	512526.13	Estación Pemex km 120	TICKET-86889	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e358efc4-711d-45bd-ab76-c2651b8a5245	8f3ca495-c031-43be-88f9-403c2813e1d7	3ed14117-4757-4269-8790-b7b2c982052f	705280e1-81dd-4793-b352-fbf5593d35b8	2026-07-03 09:00:00-06	357.000	27.2200	9717.54	78538.12	Estación Pemex km 120	TICKET-72770	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e886a684-0b19-4fa5-9f45-39a866c6aed1	8f3ca495-c031-43be-88f9-403c2813e1d7	6dbfefa7-101c-46ca-9786-28a3adb235f6	2bced3f7-432c-4465-9b96-c0322c375bc2	2026-07-11 02:00:00-06	205.000	26.8500	5504.25	81171.45	Estación Pemex km 120	TICKET-60595	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
eebefd7e-e8e5-4f7f-8f1d-c1c8560c4601	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	23e05f3e-f76d-43fd-8406-0f1fd143f59c	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-04 07:00:00-06	186.000	25.7200	4783.92	143764.92	Estación Pemex km 120	TICKET-99409	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f0577fe8-1256-4182-95f8-bcbd888bcede	8f3ca495-c031-43be-88f9-403c2813e1d7	66effb93-a98b-4d83-a73e-dbc6713eced6	0a6009aa-244a-426b-85e6-5590d7baaf5a	2026-07-07 03:00:00-06	257.000	26.0700	6699.99	79834.85	Estación Pemex km 120	TICKET-83523	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fe6e0c5d-5850-4ca0-87c9-d3fd269c8785	1709cadd-2a88-48cd-bc22-344c9b2949cf	ee7916ec-1c38-4869-8329-87906bcac270	705280e1-81dd-4793-b352-fbf5593d35b8	2026-06-22 09:00:00-06	336.000	27.1300	9115.68	199084.53	Estación Pemex km 120	TICKET-94398	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fefc041d-f359-4078-888b-ff616c6396df	8f3ca495-c031-43be-88f9-403c2813e1d7	5e586a5b-cae0-4c15-a223-84a4530423e8	8222ade5-65e9-4ef5-b185-0c71996cf02c	2026-07-14 08:00:00-06	310.000	25.2400	7824.40	81933.85	Estación Pemex km 120	TICKET-43702	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
\.


--
-- Data for Name: maintenance_orders; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.maintenance_orders ("Id", "Folio", "VehicleId", "Kind", "Status", "OpenedAtUtc", "ClosedAtUtc", "Description", "Workshop", "Cost", "OdometerAtService", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
793d2c21-3d0c-4b56-a89f-e1ea8d44cc10	OS-2026-000001	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	1	1	2026-08-03 07:41:37.750985-06	\N	Falla en sistema de frenos traseros.	\N	0.00	\N	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
dcbf5175-da47-4a03-8c6b-7b1c0cb9a3c1	OS-2026-000002	aa074c2c-fe5a-442a-a6ed-669d6739062f	0	2	2026-07-17 07:41:37.750985-06	2026-07-19 07:41:37.750985-06	Servicio mayor de 500 mil kilómetros.	Taller Diésel Norte	28450.00	524533.15	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
\.


--
-- Data for Name: tenants; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.tenants ("Id", "Name", "Slug", "TaxId", "ContactEmail", "Phone", settings, "IsActive", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
9a2d9571-c52c-40ed-85b2-f5d98581a1e8	Transportes del Norte	demo	TNO250101ABC	operaciones@transportesdelnorte.mx	81 8000 1234	{"locale": "es-MX", "logoUrl": null, "timeZoneId": "America/Mexico_City", "volumeUnit": 0, "weightUnit": 0, "currencyCode": "MXN", "distanceUnit": 0, "currencySymbol": "$", "tripFolioPrefix": "VJ", "brandPrimaryColor": "#0E7C66", "defaultDriverPayRate": 95, "defaultDriverPayScheme": 0, "licenseExpiryAlertDays": 30, "defaultFuelPricePerUnit": 25.90, "minAcceptableFuelEfficiency": 2.2}	t	2026-08-06 07:41:37.53405-06	sistema	\N	\N
\.


--
-- Data for Name: trips; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.trips ("Id", "Folio", "DriverId", "VehicleId", "TrailerId", "CustomerId", "Origin", "Destination", "PlannedDistance", "ScheduledDepartureUtc", "ScheduledArrivalUtc", "ActualDepartureUtc", "ActualArrivalUtc", "OdometerStart", "OdometerEnd", "InitialFuel", "FinalFuel", "RefuelPlanned", "CargoWeight", "CargoWeightUnit", "CargoDescription", "FreightRevenue", "DriverPayScheme", "DriverPayRate", "DriverHours", "DriverPayAmount", "Status", "Notes", "CancellationReason", custom_fields, "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
0a770914-95e7-4d16-8aa0-30f4984c2df6	VJ-2026-000019	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	1791a784-f779-465e-b5cb-d3c79bbcf836	25f2f001-f2e9-427f-8f03-2fbf49847c6b	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-01 23:00:00-06	2026-07-02 11:18:23.225806-06	2026-07-01 23:00:00-06	2026-07-02 09:18:23.225806-06	386811.95	387316.65	213.000	305.570	t	9000.000	0	Carga consolidada	18025.00	0	105.00	10.31	1082.55	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
0a84b59a-f3fe-4a5d-b34d-9b400748921f	VJ-2026-000014	2bced3f7-432c-4465-9b96-c0322c375bc2	aa074c2c-fe5a-442a-a6ed-669d6739062f	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	25f2f001-f2e9-427f-8f03-2fbf49847c6b	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-06-28 06:00:00-06	2026-06-28 18:18:23.225806-06	2026-06-28 06:00:00-06	2026-06-28 15:18:23.225806-06	513394.40	513893.95	224.000	289.430	t	20000.000	0	Carga consolidada	17510.00	0	95.00	9.31	884.45	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
12021d76-8cba-4893-ad95-086812a7a00b	VJ-2026-000004	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Saltillo, COAH	Querétaro, QRO	640.00	2026-06-23 03:00:00-06	2026-06-23 18:19:21.290322-06	2026-06-23 03:00:00-06	2026-06-23 19:19:21.290322-06	384500.00	385127.20	220.000	303.630	t	21000.000	0	Carga consolidada	20480.00	0	105.00	16.32	1713.60	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
121872a6-0a62-42e5-bcdf-fda1bcf97dda	VJ-2026-000049	8222ade5-65e9-4ef5-b185-0c71996cf02c	8f3ca495-c031-43be-88f9-403c2813e1d7	869d6498-3590-45d1-b372-258ac18a65e8	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Ciudad de México	920.00	2026-07-18 04:00:00-06	2026-07-18 22:50:19.354838-06	2026-07-18 04:00:00-06	2026-07-19 00:50:19.354838-06	82356.25	83303.85	393.000	352.180	t	20000.000	0	Carga consolidada	31280.00	3	12.00	20.84	3753.60	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
16cc60f9-9a3f-41df-aa60-f62a45a23905	VJ-2026-000060	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	869d6498-3590-45d1-b372-258ac18a65e8	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-07-22 02:00:00-06	2026-07-22 10:37:44.516129-06	2026-07-22 02:00:00-06	2026-07-22 12:37:44.516129-06	148400.70	148636.95	264.000	483.640	f	24000.000	0	Carga consolidada	8100.00	0	105.00	10.63	1116.15	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
18870d3c-5de0-478a-a60b-8c2da843ee33	VJ-2026-000032	705280e1-81dd-4793-b352-fbf5593d35b8	8f3ca495-c031-43be-88f9-403c2813e1d7	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-09 04:00:00-06	2026-07-09 16:18:23.225806-06	2026-07-09 04:00:00-06	2026-07-09 18:18:23.225806-06	80380.85	80895.85	309.000	388.200	t	8000.000	0	Carga consolidada	14420.00	0	110.00	14.31	1574.10	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1957d298-0fa7-4a01-ae83-294864f8fe40	VJ-2026-000074	86e72142-66cc-461b-94e0-f5daff605e4f	aa074c2c-fe5a-442a-a6ed-669d6739062f	869d6498-3590-45d1-b372-258ac18a65e8	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Ciudad de México	920.00	2026-07-30 02:00:00-06	2026-07-30 19:50:19.354838-06	2026-07-30 02:00:00-06	2026-07-30 20:50:19.354838-06	522428.90	523404.10	222.000	55.000	t	18000.000	0	Carga consolidada	32200.00	1	3.20	18.84	3120.64	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1a0bca72-0d5e-4e73-b1dd-04efff6b70e7	VJ-2026-000061	2bced3f7-432c-4465-9b96-c0322c375bc2	1709cadd-2a88-48cd-bc22-344c9b2949cf	1791a784-f779-465e-b5cb-d3c79bbcf836	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-23 01:00:00-06	2026-07-23 16:34:50.32258-06	2026-07-23 01:00:00-06	2026-07-23 18:34:50.32258-06	203459.65	204216.25	375.000	332.790	t	8000.000	0	Carga consolidada	22620.00	0	95.00	17.58	1670.10	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1bdeb0d1-f0f6-4770-baa9-7e0ca417336f	VJ-2026-000069	8222ade5-65e9-4ef5-b185-0c71996cf02c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	869d6498-3590-45d1-b372-258ac18a65e8	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-27 02:00:00-06	2026-07-27 23:03:52.258064-06	2026-07-27 02:00:00-06	2026-07-27 20:03:52.258064-06	393582.20	394691.00	254.000	119.930	t	8000.000	0	Carga consolidada	31360.00	3	12.00	18.06	3763.20	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1bfe8a75-c931-40be-8108-0bf80ceb02a4	VJ-2026-000010	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	30c62f14-5d2e-4676-972d-b06f190fc4e6	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Torreón, COAH	400.00	2026-06-26 07:00:00-06	2026-06-26 18:27:05.806451-06	2026-06-26 07:00:00-06	2026-06-26 18:27:05.806451-06	385352.20	385756.20	311.000	447.430	f	21000.000	0	Carga consolidada	11600.00	0	105.00	11.45	1202.25	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
1f0496f2-8b3d-40ba-8ff5-5301c453693a	VJ-2026-000048	86e72142-66cc-461b-94e0-f5daff605e4f	1709cadd-2a88-48cd-bc22-344c9b2949cf	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-17 00:00:00-06	2026-07-17 11:18:23.225806-06	2026-07-17 00:00:00-06	2026-07-17 07:18:23.225806-06	202414.20	202929.20	496.000	563.370	t	16000.000	0	Carga consolidada	20600.00	1	3.20	7.31	1648.00	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
23974bf1-2f32-465d-93c9-168327cf7779	VJ-2026-000025	2bced3f7-432c-4465-9b96-c0322c375bc2	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	869d6498-3590-45d1-b372-258ac18a65e8	cec94ae7-fadf-4db8-981d-80df95ba3774	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-06 08:00:00-06	2026-07-07 08:03:52.258064-06	2026-07-06 08:00:00-06	2026-07-07 05:03:52.258064-06	144183.05	145359.05	411.000	215.040	t	26000.000	0	Carga consolidada	42560.00	0	95.00	21.06	2000.70	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
23e05f3e-f76d-43fd-8406-0f1fd143f59c	VJ-2026-000021	8222ade5-65e9-4ef5-b185-0c71996cf02c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	30c62f14-5d2e-4676-972d-b06f190fc4e6	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-04 05:00:00-06	2026-07-04 21:19:21.290322-06	2026-07-04 05:00:00-06	2026-07-04 18:19:21.290322-06	143555.85	144183.05	324.000	386.620	t	26000.000	0	Carga consolidada	19840.00	3	12.00	13.32	2380.80	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
245d4afd-7294-4492-9757-c2948b7ec3b0	VJ-2026-000016	86e72142-66cc-461b-94e0-f5daff605e4f	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	869d6498-3590-45d1-b372-258ac18a65e8	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-06-30 00:00:00-06	2026-06-30 12:18:23.225806-06	2026-06-30 00:00:00-06	2026-06-30 09:18:23.225806-06	386307.25	386811.95	344.000	421.600	t	24000.000	0	Carga consolidada	20600.00	1	3.20	9.31	1615.04	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
2b95e099-6abf-43c4-ae92-9dd41351dde7	VJ-2026-000029	8222ade5-65e9-4ef5-b185-0c71996cf02c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	1791a784-f779-465e-b5cb-d3c79bbcf836	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Torreón, COAH	400.00	2026-07-07 05:00:00-06	2026-07-07 14:27:05.806451-06	2026-07-07 05:00:00-06	2026-07-07 16:27:05.806451-06	145586.30	145994.30	339.000	442.780	f	8000.000	0	Carga consolidada	14400.00	3	12.00	11.45	1728.00	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
305c36be-2652-4f7d-b3d2-e798afe40c17	VJ-2026-000043	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Torreón, COAH	400.00	2026-07-15 01:00:00-06	2026-07-15 12:27:05.806451-06	2026-07-15 01:00:00-06	2026-07-15 08:27:05.806451-06	388523.80	388935.80	238.000	365.720	f	20000.000	0	Carga consolidada	11200.00	0	105.00	7.45	782.25	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
31c2aee8-9f27-47a4-a98d-4fe01c3eef05	VJ-2026-000071	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	8f3ca495-c031-43be-88f9-403c2813e1d7	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-28 05:00:00-06	2026-07-29 05:03:52.258064-06	2026-07-28 05:00:00-06	2026-07-29 04:03:52.258064-06	86136.20	87301.00	280.000	133.680	t	17000.000	0	Carga consolidada	43680.00	0	105.00	23.06	2421.30	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
31f7d861-27d5-4c33-967b-824a4f8a66ac	VJ-2026-000047	0a6009aa-244a-426b-85e6-5590d7baaf5a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Torreón, COAH	400.00	2026-07-17 06:00:00-06	2026-07-17 18:27:05.806451-06	\N	\N	\N	\N	443.000	\N	f	10000.000	0	Carga consolidada	12800.00	2	2800.00	\N	2800.00	3	\N	El cliente reprogramó la carga.	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
338f6303-bff0-4253-a223-d1c65580e194	VJ-2026-000076	705280e1-81dd-4793-b352-fbf5593d35b8	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	30c62f14-5d2e-4676-972d-b06f190fc4e6	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-07-31 23:00:00-06	2026-08-01 08:37:44.516129-06	\N	\N	\N	\N	245.000	\N	f	19000.000	0	Carga consolidada	6975.00	0	110.00	\N	0.00	3	\N	El cliente reprogramó la carga.	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
35dcc603-9902-4d26-acf6-5d7fab7c81ba	VJ-2026-000051	2bced3f7-432c-4465-9b96-c0322c375bc2	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Ciudad de México	920.00	2026-07-19 00:00:00-06	2026-07-19 17:50:19.354838-06	2026-07-19 00:00:00-06	2026-07-19 18:50:19.354838-06	518103.35	519004.95	314.000	239.150	t	23000.000	0	Carga consolidada	25760.00	0	95.00	18.84	1789.80	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3c972500-11d1-48df-bb21-8ddfff247b11	VJ-2026-000079	2bced3f7-432c-4465-9b96-c0322c375bc2	1709cadd-2a88-48cd-bc22-344c9b2949cf	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-08-02 00:00:00-06	2026-08-02 12:18:23.225806-06	2026-08-02 00:00:00-06	2026-08-02 08:18:23.225806-06	206252.25	206772.40	421.000	540.580	t	21000.000	0	Carga consolidada	16995.00	0	95.00	8.31	789.45	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3d525c86-2765-4b0e-930e-e9beeee1abd7	VJ-2026-000088	705280e1-81dd-4793-b352-fbf5593d35b8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	30c62f14-5d2e-4676-972d-b06f190fc4e6	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Torreón, COAH	400.00	2026-08-05 18:41:37.744096-06	2026-08-06 06:08:43.550548-06	2026-08-05 18:41:37.744096-06	2026-08-06 00:41:37.744096-06	396968.15	397396.15	459.000	571.540	f	9000.000	0	Carga consolidada	13200.00	0	110.00	6.00	660.00	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
3ed14117-4757-4269-8790-b7b2c982052f	VJ-2026-000020	705280e1-81dd-4793-b352-fbf5593d35b8	8f3ca495-c031-43be-88f9-403c2813e1d7	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Ciudad de México	920.00	2026-07-03 07:00:00-06	2026-07-04 02:50:19.354838-06	2026-07-03 07:00:00-06	2026-07-04 04:50:19.354838-06	78222.25	79169.85	320.000	225.710	t	27000.000	0	Carga consolidada	26680.00	0	110.00	21.84	2402.40	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4a1c287c-7be1-49b7-84b0-bd0cd4f84037	VJ-2026-000013	86e72142-66cc-461b-94e0-f5daff605e4f	8f3ca495-c031-43be-88f9-403c2813e1d7	1791a784-f779-465e-b5cb-d3c79bbcf836	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Saltillo, COAH	Querétaro, QRO	640.00	2026-06-28 02:00:00-06	2026-06-28 18:19:21.290322-06	2026-06-28 02:00:00-06	2026-06-28 15:19:21.290322-06	76618.25	77283.85	273.000	306.550	t	11000.000	0	Carga consolidada	23040.00	1	3.20	13.32	2129.92	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4a76544d-2b35-47ca-85bf-afbbe82a7552	VJ-2026-000081	2bced3f7-432c-4465-9b96-c0322c375bc2	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Ciudad de México	920.00	2026-08-02 03:00:00-06	2026-08-02 20:50:19.354838-06	2026-08-02 03:00:00-06	2026-08-02 22:50:19.354838-06	394691.00	395675.40	271.000	139.970	t	23000.000	0	Carga consolidada	28520.00	0	95.00	19.84	1884.80	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
4d1fc37d-7b1f-4b37-9230-b52209285833	VJ-2026-000075	0a6009aa-244a-426b-85e6-5590d7baaf5a	1709cadd-2a88-48cd-bc22-344c9b2949cf	1791a784-f779-465e-b5cb-d3c79bbcf836	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-31 02:00:00-06	2026-08-01 00:03:52.258064-06	2026-07-31 02:00:00-06	2026-08-01 00:03:52.258064-06	205154.65	206252.25	276.000	205.100	t	9000.000	0	Carga consolidada	43680.00	2	2800.00	22.06	2800.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
54f2ed83-df52-4d5e-9ed6-1cda3cd14d89	VJ-2026-000073	8222ade5-65e9-4ef5-b185-0c71996cf02c	1709cadd-2a88-48cd-bc22-344c9b2949cf	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Ciudad de México	920.00	2026-07-29 00:00:00-06	2026-07-29 17:50:19.354838-06	2026-07-29 00:00:00-06	2026-07-29 16:50:19.354838-06	204216.25	205154.65	358.000	325.870	t	10000.000	0	Carga consolidada	29440.00	3	12.00	16.84	3532.80	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
55ffc6b7-8949-4403-85fc-cc113dd908cb	VJ-2026-000058	2bced3f7-432c-4465-9b96-c0322c375bc2	8f3ca495-c031-43be-88f9-403c2813e1d7	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-22 06:00:00-06	2026-07-22 17:18:23.225806-06	2026-07-22 06:00:00-06	2026-07-22 19:18:23.225806-06	83988.65	84529.40	491.000	528.370	t	20000.000	0	Carga consolidada	15965.00	0	95.00	13.31	1264.45	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5819ad28-5367-4907-86f9-1c323d6dfef7	VJ-2026-000077	0a6009aa-244a-426b-85e6-5590d7baaf5a	aa074c2c-fe5a-442a-a6ed-669d6739062f	869d6498-3590-45d1-b372-258ac18a65e8	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-08-01 05:00:00-06	2026-08-01 14:37:44.516129-06	2026-08-01 05:00:00-06	2026-08-01 13:37:44.516129-06	523404.10	523622.35	407.000	616.420	f	11000.000	0	Carga consolidada	8100.00	2	2800.00	8.63	2800.00	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
589a9654-8a1d-43d7-8d25-614161e2e099	VJ-2026-000012	86e72142-66cc-461b-94e0-f5daff605e4f	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	869d6498-3590-45d1-b372-258ac18a65e8	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Torreón, COAH	400.00	2026-06-27 03:00:00-06	2026-06-27 15:27:05.806451-06	2026-06-27 03:00:00-06	2026-06-27 16:27:05.806451-06	142156.75	142556.75	494.000	614.260	f	8000.000	0	Carga consolidada	11600.00	1	3.20	13.45	1280.00	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5a6f40e7-b3f1-4d7f-acf2-d3a6b3bdf748	VJ-2026-000087	705280e1-81dd-4793-b352-fbf5593d35b8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-08-05 20:41:37.744096-06	2026-08-06 04:19:22.260225-06	2026-08-05 20:41:37.744096-06	2026-08-06 05:19:22.260225-06	396727.40	396968.15	339.000	532.800	f	22000.000	0	Carga consolidada	7875.00	0	110.00	8.63	949.30	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5c115bbe-ec90-40d9-8512-13696bdc6a22	VJ-2026-000023	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	8f3ca495-c031-43be-88f9-403c2813e1d7	1791a784-f779-465e-b5cb-d3c79bbcf836	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Torreón, COAH	400.00	2026-07-05 02:00:00-06	2026-07-05 11:27:05.806451-06	2026-07-05 02:00:00-06	2026-07-05 12:27:05.806451-06	79169.85	79561.85	228.000	337.360	f	15000.000	0	Carga consolidada	11600.00	0	105.00	10.45	1097.25	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5ccd65ec-1fda-42cf-84f0-73edee47802e	VJ-2026-000006	2bced3f7-432c-4465-9b96-c0322c375bc2	aa074c2c-fe5a-442a-a6ed-669d6739062f	30c62f14-5d2e-4676-972d-b06f190fc4e6	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Torreón, COAH	400.00	2026-06-24 02:00:00-06	2026-06-24 13:27:05.806451-06	2026-06-24 02:00:00-06	2026-06-24 13:27:05.806451-06	512978.40	513394.40	459.000	601.290	f	27000.000	0	Carga consolidada	14800.00	0	95.00	11.45	1087.75	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
5e586a5b-cae0-4c15-a223-84a4530423e8	VJ-2026-000040	8222ade5-65e9-4ef5-b185-0c71996cf02c	8f3ca495-c031-43be-88f9-403c2813e1d7	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-14 06:00:00-06	2026-07-14 21:19:21.290322-06	2026-07-14 06:00:00-06	2026-07-14 17:19:21.290322-06	81722.65	82356.25	260.000	269.970	t	23000.000	0	Carga consolidada	19200.00	3	12.00	11.32	2304.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
61959438-3cc4-46a4-8ed1-e8589e24d4a2	VJ-2026-000065	8222ade5-65e9-4ef5-b185-0c71996cf02c	8f3ca495-c031-43be-88f9-403c2813e1d7	869d6498-3590-45d1-b372-258ac18a65e8	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-25 05:00:00-06	2026-07-25 21:34:50.32258-06	2026-07-25 05:00:00-06	2026-07-25 20:34:50.32258-06	84529.40	85364.00	473.000	482.830	t	13000.000	0	Carga consolidada	31200.00	3	12.00	15.58	3744.00	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6315e558-0e5d-4f28-88f4-2de23f9584e4	VJ-2026-000056	86e72142-66cc-461b-94e0-f5daff605e4f	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	869d6498-3590-45d1-b372-258ac18a65e8	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-21 02:00:00-06	2026-07-21 19:34:50.32258-06	2026-07-21 02:00:00-06	2026-07-21 18:34:50.32258-06	390111.60	390915.00	255.000	216.080	t	22000.000	0	Carga consolidada	22620.00	1	3.20	16.58	2570.88	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
65111110-c9a7-4096-a902-4c3d1ce62319	VJ-2026-000042	8222ade5-65e9-4ef5-b185-0c71996cf02c	1709cadd-2a88-48cd-bc22-344c9b2949cf	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-14 02:00:00-06	2026-07-14 17:34:50.32258-06	2026-07-14 02:00:00-06	2026-07-14 19:34:50.32258-06	201642.00	202414.20	226.000	195.910	t	10000.000	0	Carga consolidada	26520.00	3	12.00	17.58	3182.40	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
66effb93-a98b-4d83-a73e-dbc6713eced6	VJ-2026-000027	0a6009aa-244a-426b-85e6-5590d7baaf5a	8f3ca495-c031-43be-88f9-403c2813e1d7	1791a784-f779-465e-b5cb-d3c79bbcf836	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-07 01:00:00-06	2026-07-07 17:34:50.32258-06	2026-07-07 01:00:00-06	2026-07-07 18:34:50.32258-06	79561.85	80380.85	222.000	130.820	t	24000.000	0	Carga consolidada	26520.00	2	2800.00	17.58	2800.00	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6b93721a-c050-4375-b083-987cfd95a4eb	VJ-2026-000068	8222ade5-65e9-4ef5-b185-0c71996cf02c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	30c62f14-5d2e-4676-972d-b06f190fc4e6	cec94ae7-fadf-4db8-981d-80df95ba3774	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-26 03:00:00-06	2026-07-27 03:03:52.258064-06	2026-07-26 03:00:00-06	2026-07-27 00:03:52.258064-06	392439.80	393582.20	221.000	110.550	t	9000.000	0	Carga consolidada	35840.00	3	12.00	21.06	4300.80	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6dbfefa7-101c-46ca-9786-28a3adb235f6	VJ-2026-000035	2bced3f7-432c-4465-9b96-c0322c375bc2	8f3ca495-c031-43be-88f9-403c2813e1d7	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-11 00:00:00-06	2026-07-11 16:34:50.32258-06	2026-07-11 00:00:00-06	2026-07-11 18:34:50.32258-06	80895.85	81722.65	237.000	187.070	t	14000.000	0	Carga consolidada	31980.00	0	95.00	18.58	1765.10	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6e0b3337-4dd1-4973-9a4b-bf9752ac7422	VJ-2026-000080	705280e1-81dd-4793-b352-fbf5593d35b8	1709cadd-2a88-48cd-bc22-344c9b2949cf	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Saltillo, COAH	Querétaro, QRO	640.00	2026-08-02 00:00:00-06	2026-08-02 14:19:21.290322-06	2026-08-02 00:00:00-06	2026-08-02 12:19:21.290322-06	206772.40	207444.40	371.000	354.560	t	20000.000	0	Carga consolidada	22400.00	0	110.00	12.32	1355.20	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
6f5eb4f8-d810-4feb-ba87-50ff97277a8a	VJ-2026-000015	8222ade5-65e9-4ef5-b185-0c71996cf02c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	1791a784-f779-465e-b5cb-d3c79bbcf836	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-06-29 08:00:00-06	2026-06-29 21:18:23.225806-06	2026-06-29 08:00:00-06	2026-06-29 17:18:23.225806-06	142556.75	143056.30	245.000	360.660	t	22000.000	0	Carga consolidada	14935.00	3	12.00	9.31	1792.20	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
72a70e91-36f1-466a-8ba4-32606cc0d679	VJ-2026-000082	0a6009aa-244a-426b-85e6-5590d7baaf5a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	869d6498-3590-45d1-b372-258ac18a65e8	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Torreón, COAH	400.00	2026-08-03 02:00:00-06	2026-08-03 13:27:05.806451-06	2026-08-03 02:00:00-06	2026-08-03 12:27:05.806451-06	395675.40	396087.40	200.000	298.870	f	14000.000	0	Carga consolidada	13200.00	2	2800.00	10.45	2800.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7400c6aa-8a44-4a34-b76b-4824038a790d	VJ-2026-000054	8222ade5-65e9-4ef5-b185-0c71996cf02c	1709cadd-2a88-48cd-bc22-344c9b2949cf	30c62f14-5d2e-4676-972d-b06f190fc4e6	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-20 02:00:00-06	2026-07-20 13:18:23.225806-06	2026-07-20 02:00:00-06	2026-07-20 11:18:23.225806-06	202929.20	203459.65	227.000	340.140	t	17000.000	0	Carga consolidada	18025.00	3	12.00	9.31	2163.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7546fdce-398a-4265-b9a7-92db4ea57cf8	VJ-2026-000057	0a6009aa-244a-426b-85e6-5590d7baaf5a	aa074c2c-fe5a-442a-a6ed-669d6739062f	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-21 06:00:00-06	2026-07-21 20:19:21.290322-06	2026-07-21 06:00:00-06	2026-07-21 16:19:21.290322-06	519664.15	520316.95	205.000	255.330	t	22000.000	0	Carga consolidada	18560.00	2	2800.00	10.32	2800.00	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
7d03c213-67ac-4b36-ae23-3043682f983a	VJ-2026-000011	2bced3f7-432c-4465-9b96-c0322c375bc2	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	1791a784-f779-465e-b5cb-d3c79bbcf836	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-06-26 23:00:00-06	2026-06-27 13:18:23.225806-06	2026-06-26 23:00:00-06	2026-06-27 11:18:23.225806-06	385756.20	386307.25	292.000	337.650	t	22000.000	0	Carga consolidada	19055.00	0	95.00	12.31	1169.45	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8031560b-18d7-4f4b-a6b5-8db0e0a44e2c	VJ-2026-000026	705280e1-81dd-4793-b352-fbf5593d35b8	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-07-06 03:00:00-06	2026-07-06 11:37:44.516129-06	2026-07-06 03:00:00-06	2026-07-06 08:37:44.516129-06	145359.05	145586.30	278.000	499.040	f	27000.000	0	Carga consolidada	8550.00	0	110.00	5.63	619.30	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
839924a4-7be3-4512-a079-4bc2db5cc4ed	VJ-2026-000028	86e72142-66cc-461b-94e0-f5daff605e4f	1709cadd-2a88-48cd-bc22-344c9b2949cf	30c62f14-5d2e-4676-972d-b06f190fc4e6	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-07 06:00:00-06	2026-07-08 05:03:52.258064-06	2026-07-07 06:00:00-06	2026-07-08 07:03:52.258064-06	200087.60	201230.00	431.000	258.130	t	13000.000	0	Carga consolidada	38080.00	1	3.20	25.06	3655.68	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
868be146-c911-4415-94c5-faf6ec1fa3ab	VJ-2026-000044	8222ade5-65e9-4ef5-b185-0c71996cf02c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	869d6498-3590-45d1-b372-258ac18a65e8	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Torreón, COAH	400.00	2026-07-15 04:00:00-06	2026-07-15 14:27:05.806451-06	2026-07-15 04:00:00-06	2026-07-15 13:27:05.806451-06	388935.80	389323.80	291.000	457.800	f	17000.000	0	Carga consolidada	13600.00	3	12.00	9.45	1632.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
89fb3c67-4478-4a2c-a908-a09146da548e	VJ-2026-000089	705280e1-81dd-4793-b352-fbf5593d35b8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	869d6498-3590-45d1-b372-258ac18a65e8	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Saltillo, COAH	Querétaro, QRO	640.00	2026-08-06 05:41:37.744096-06	2026-08-06 20:00:59.034419-06	2026-08-06 05:41:37.744096-06	\N	397396.15	\N	306.000	\N	t	25000.000	0	Carga consolidada	19840.00	0	110.00	\N	0.00	1	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
8ce37e6d-ab47-4eba-a3e1-150bd6ceefa2	VJ-2026-000066	2bced3f7-432c-4465-9b96-c0322c375bc2	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Torreón, COAH	400.00	2026-07-25 23:00:00-06	2026-07-26 08:27:05.806451-06	2026-07-25 23:00:00-06	2026-07-26 10:27:05.806451-06	521511.60	521903.60	275.000	396.100	f	27000.000	0	Carga consolidada	16400.00	0	95.00	11.45	1087.75	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9042d857-6baa-4313-a620-99c58939b5d3	VJ-2026-000005	86e72142-66cc-461b-94e0-f5daff605e4f	8f3ca495-c031-43be-88f9-403c2813e1d7	869d6498-3590-45d1-b372-258ac18a65e8	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-06-23 23:00:00-06	2026-06-24 05:37:44.516129-06	2026-06-23 23:00:00-06	2026-06-24 03:37:44.516129-06	76400.00	76618.25	371.000	586.630	f	19000.000	0	Carga consolidada	7875.00	1	3.20	4.63	698.40	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
919d551c-9c3d-452f-9b96-d70076cef288	VJ-2026-000041	86e72142-66cc-461b-94e0-f5daff605e4f	aa074c2c-fe5a-442a-a6ed-669d6739062f	869d6498-3590-45d1-b372-258ac18a65e8	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-07-14 02:00:00-06	2026-07-14 09:37:44.516129-06	2026-07-14 02:00:00-06	2026-07-14 09:37:44.516129-06	516912.35	517137.35	342.000	559.330	f	21000.000	0	Carga consolidada	8325.00	1	3.20	7.63	720.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
97c2ff14-f188-4b2a-9155-87c99fa6c70b	VJ-2026-000055	86e72142-66cc-461b-94e0-f5daff605e4f	8f3ca495-c031-43be-88f9-403c2813e1d7	30c62f14-5d2e-4676-972d-b06f190fc4e6	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-20 08:00:00-06	2026-07-21 00:19:21.290322-06	2026-07-20 08:00:00-06	2026-07-21 00:19:21.290322-06	83303.85	83988.65	463.000	440.840	t	12000.000	0	Carga consolidada	23040.00	1	3.20	16.32	2191.36	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
98b69f0f-f059-40aa-b1b0-ce51678ae08c	VJ-2026-000064	8222ade5-65e9-4ef5-b185-0c71996cf02c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	30c62f14-5d2e-4676-972d-b06f190fc4e6	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Torreón, COAH	400.00	2026-07-24 04:00:00-06	2026-07-24 15:27:05.806451-06	2026-07-24 04:00:00-06	2026-07-24 17:27:05.806451-06	392023.80	392439.80	345.000	486.350	f	18000.000	0	Carga consolidada	14000.00	3	12.00	13.45	1680.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
99e79f0d-bee9-4643-8a1a-77ba58adcfa0	VJ-2026-000052	705280e1-81dd-4793-b352-fbf5593d35b8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	1791a784-f779-465e-b5cb-d3c79bbcf836	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-19 00:00:00-06	2026-07-19 16:34:50.32258-06	2026-07-19 00:00:00-06	2026-07-19 16:34:50.32258-06	389323.80	390111.60	498.000	438.100	t	14000.000	0	Carga consolidada	21840.00	0	110.00	16.58	1823.80	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9a65ff54-2df7-412f-b306-1eb8565d392f	VJ-2026-000085	705280e1-81dd-4793-b352-fbf5593d35b8	8f3ca495-c031-43be-88f9-403c2813e1d7	869d6498-3590-45d1-b372-258ac18a65e8	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Torreón, COAH	400.00	2026-08-04 01:00:00-06	2026-08-04 12:27:05.806451-06	2026-08-04 01:00:00-06	2026-08-04 12:27:05.806451-06	87301.00	87693.00	417.000	569.810	f	11000.000	0	Carga consolidada	11600.00	0	110.00	11.45	1259.50	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9c687700-3226-424c-b7e5-a41b38d407f6	VJ-2026-000030	8222ade5-65e9-4ef5-b185-0c71996cf02c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-08 08:00:00-06	2026-07-08 20:18:23.225806-06	2026-07-08 08:00:00-06	2026-07-08 19:18:23.225806-06	145994.30	146524.75	461.000	581.590	t	8000.000	0	Carga consolidada	19055.00	3	12.00	11.31	2286.60	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9e76936b-bb57-45c0-9b19-c0f5ab16cfe4	VJ-2026-000053	0a6009aa-244a-426b-85e6-5590d7baaf5a	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-18 23:00:00-06	2026-07-19 13:19:21.290322-06	2026-07-18 23:00:00-06	2026-07-19 09:19:21.290322-06	519004.95	519664.15	459.000	521.980	t	27000.000	0	Carga consolidada	25600.00	2	2800.00	10.32	2800.00	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
9f0202ef-2f4d-4504-87bf-b301512a6763	VJ-2026-000022	8222ade5-65e9-4ef5-b185-0c71996cf02c	aa074c2c-fe5a-442a-a6ed-669d6739062f	869d6498-3590-45d1-b372-258ac18a65e8	cec94ae7-fadf-4db8-981d-80df95ba3774	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-04 07:00:00-06	2026-07-04 20:19:21.290322-06	2026-07-04 07:00:00-06	2026-07-04 22:19:21.290322-06	513893.95	514521.15	253.000	286.850	t	14000.000	0	Carga consolidada	25600.00	3	12.00	15.32	3072.00	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a0eed33f-16af-4cf7-a332-4a7335323c03	VJ-2026-000031	8222ade5-65e9-4ef5-b185-0c71996cf02c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-07-08 04:00:00-06	2026-07-08 11:37:44.516129-06	2026-07-08 04:00:00-06	2026-07-08 11:37:44.516129-06	387316.65	387539.40	340.000	558.390	f	22000.000	0	Carga consolidada	6525.00	3	12.00	7.63	783.00	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a28f3acc-667c-4b32-913a-992935f08655	VJ-2026-000038	86e72142-66cc-461b-94e0-f5daff605e4f	aa074c2c-fe5a-442a-a6ed-669d6739062f	869d6498-3590-45d1-b372-258ac18a65e8	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-12 23:00:00-06	2026-07-13 17:34:50.32258-06	2026-07-12 23:00:00-06	2026-07-13 16:34:50.32258-06	516101.15	516912.35	300.000	270.030	t	14000.000	0	Carga consolidada	24180.00	1	3.20	17.58	2595.84	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a4a9db81-97c6-47f4-a683-10e1fa576dd4	VJ-2026-000009	8222ade5-65e9-4ef5-b185-0c71996cf02c	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	1791a784-f779-465e-b5cb-d3c79bbcf836	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-06-26 05:00:00-06	2026-06-26 11:37:44.516129-06	2026-06-26 05:00:00-06	2026-06-26 09:37:44.516129-06	385127.20	385352.20	485.000	679.380	f	23000.000	0	Carga consolidada	7200.00	3	12.00	4.63	864.00	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a739ea38-9105-4b31-abae-107484078d4a	VJ-2026-000067	8222ade5-65e9-4ef5-b185-0c71996cf02c	8f3ca495-c031-43be-88f9-403c2813e1d7	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Guadalajara, JAL	780.00	2026-07-26 08:00:00-06	2026-07-27 01:34:50.32258-06	2026-07-26 08:00:00-06	2026-07-27 01:34:50.32258-06	85364.00	86136.20	224.000	232.950	t	27000.000	0	Carga consolidada	31980.00	3	12.00	17.58	3837.60	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
a814cf35-bf5e-404e-8f29-4de263ba730e	VJ-2026-000091	86e72142-66cc-461b-94e0-f5daff605e4f	1709cadd-2a88-48cd-bc22-344c9b2949cf	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Ciudad de México	920.00	2026-08-06 11:41:37.744096-06	2026-08-07 05:31:57.098935-06	\N	\N	\N	\N	326.000	\N	t	9000.000	0	Carga consolidada	27600.00	1	3.20	\N	2944.00	0	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
aa9c84e1-b07f-46cd-bfcb-014dbc1964f7	VJ-2026-000039	705280e1-81dd-4793-b352-fbf5593d35b8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Ciudad de México	920.00	2026-07-13 06:00:00-06	2026-07-14 02:50:19.354838-06	2026-07-13 06:00:00-06	2026-07-14 01:50:19.354838-06	387539.40	388523.80	245.000	192.540	t	17000.000	0	Carga consolidada	36800.00	0	110.00	19.84	2182.40	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
adcd99f2-ef69-4662-ad36-a7e29e0b1b50	VJ-2026-000090	2bced3f7-432c-4465-9b96-c0322c375bc2	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-08-06 02:41:37.744096-06	2026-08-06 10:19:22.260225-06	2026-08-06 02:41:37.744096-06	\N	524533.15	\N	426.000	\N	f	27000.000	0	Carga consolidada	9000.00	0	95.00	\N	0.00	1	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b09bf7b8-3d49-42ef-9f3a-c0b63015ff79	VJ-2026-000036	86e72142-66cc-461b-94e0-f5daff605e4f	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	1791a784-f779-465e-b5cb-d3c79bbcf836	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-11 08:00:00-06	2026-07-12 08:03:52.258064-06	2026-07-11 08:00:00-06	2026-07-12 07:03:52.258064-06	146524.75	147611.15	280.000	204.290	t	8000.000	0	Carga consolidada	42560.00	1	3.20	23.06	3476.48	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b0c91b64-30c3-476f-b6d9-5231727c1e55	VJ-2026-000046	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	30c62f14-5d2e-4676-972d-b06f190fc4e6	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-16 00:00:00-06	2026-07-16 11:18:23.225806-06	2026-07-16 00:00:00-06	2026-07-16 13:18:23.225806-06	147849.65	148400.70	283.000	367.870	t	20000.000	0	Carga consolidada	16995.00	0	105.00	13.31	1397.55	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b1424588-3d51-42f0-a899-2ffece9c39ab	VJ-2026-000092	86e72142-66cc-461b-94e0-f5daff605e4f	1709cadd-2a88-48cd-bc22-344c9b2949cf	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Culiacán, SIN	Monterrey, NL	1120.00	2026-08-06 14:41:37.744096-06	2026-08-07 13:45:30.002161-06	\N	\N	\N	\N	432.000	\N	t	12000.000	0	Carga consolidada	32480.00	1	3.20	\N	3584.00	0	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
b678f560-3801-4679-bc9f-0cdd1a8e3c41	VJ-2026-000018	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	8f3ca495-c031-43be-88f9-403c2813e1d7	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Monterrey, NL	Ciudad de México	920.00	2026-07-01 01:00:00-06	2026-07-01 20:50:19.354838-06	2026-07-01 01:00:00-06	2026-07-01 22:50:19.354838-06	77283.85	78222.25	463.000	362.760	t	19000.000	0	Carga consolidada	36800.00	0	105.00	21.84	2293.20	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bb3defaa-2e0c-42a1-af26-ae085813c47b	VJ-2026-000072	0a6009aa-244a-426b-85e6-5590d7baaf5a	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-28 01:00:00-06	2026-07-28 16:19:21.290322-06	2026-07-28 01:00:00-06	2026-07-28 16:19:21.290322-06	148636.95	149321.75	382.000	366.950	t	11000.000	0	Carga consolidada	19200.00	2	2800.00	15.32	2800.00	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
bc7e20e3-0d2e-4a58-bab9-39eae9cbfa7c	VJ-2026-000033	0a6009aa-244a-426b-85e6-5590d7baaf5a	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	30c62f14-5d2e-4676-972d-b06f190fc4e6	cec94ae7-fadf-4db8-981d-80df95ba3774	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-09 23:00:00-06	2026-07-10 14:19:21.290322-06	\N	\N	\N	\N	209.000	\N	t	15000.000	0	Carga consolidada	19840.00	2	2800.00	\N	2800.00	3	\N	El cliente reprogramó la carga.	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
c1151e10-65f5-4bbe-b6eb-818d0528444d	VJ-2026-000083	2bced3f7-432c-4465-9b96-c0322c375bc2	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	869d6498-3590-45d1-b372-258ac18a65e8	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-08-04 06:00:00-06	2026-08-04 12:37:44.516129-06	2026-08-04 06:00:00-06	2026-08-04 08:37:44.516129-06	149321.75	149546.75	306.000	501.500	f	27000.000	0	Carga consolidada	8775.00	0	95.00	2.63	249.85	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ca0904ab-3166-43df-bb4a-1793791426f5	VJ-2026-000084	8222ade5-65e9-4ef5-b185-0c71996cf02c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	1791a784-f779-465e-b5cb-d3c79bbcf836	cec94ae7-fadf-4db8-981d-80df95ba3774	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-08-04 00:00:00-06	2026-08-04 11:18:23.225806-06	2026-08-04 00:00:00-06	2026-08-04 10:18:23.225806-06	149546.75	150082.35	363.000	429.660	t	26000.000	0	Carga consolidada	20600.00	3	12.00	10.31	2472.00	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
cc642552-a331-439c-8eae-1e7691304214	VJ-2026-000070	0a6009aa-244a-426b-85e6-5590d7baaf5a	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	cec94ae7-fadf-4db8-981d-80df95ba3774	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-27 06:00:00-06	2026-07-27 18:18:23.225806-06	2026-07-27 06:00:00-06	2026-07-27 16:18:23.225806-06	521903.60	522428.90	268.000	355.730	t	24000.000	0	Carga consolidada	15450.00	2	2800.00	10.31	2800.00	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
cf783a12-df74-4a45-96d9-ac1c812bb7d3	VJ-2026-000003	8222ade5-65e9-4ef5-b185-0c71996cf02c	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Saltillo, COAH	Querétaro, QRO	640.00	2026-06-22 03:00:00-06	2026-06-22 17:19:21.290322-06	2026-06-22 03:00:00-06	2026-06-22 14:19:21.290322-06	512300.00	512978.40	233.000	236.370	t	27000.000	0	Carga consolidada	24320.00	3	12.00	11.32	2918.40	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d17b5514-b2b8-4600-89b5-db0c7589fcda	VJ-2026-000078	0a6009aa-244a-426b-85e6-5590d7baaf5a	aa074c2c-fe5a-442a-a6ed-669d6739062f	869d6498-3590-45d1-b372-258ac18a65e8	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Ciudad de México	920.00	2026-07-31 23:00:00-06	2026-08-01 18:50:19.354838-06	2026-07-31 23:00:00-06	2026-08-01 17:50:19.354838-06	523622.35	524533.15	245.000	121.290	t	24000.000	0	Carga consolidada	28520.00	2	2800.00	18.84	2800.00	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
d983083a-fdb5-4fc2-acfb-85ea6549f4e7	VJ-2026-000063	2bced3f7-432c-4465-9b96-c0322c375bc2	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Saltillo, COAH	Querétaro, QRO	640.00	2026-07-23 04:00:00-06	2026-07-23 19:19:21.290322-06	2026-07-23 04:00:00-06	2026-07-23 16:19:21.290322-06	520826.80	521511.60	414.000	406.200	t	20000.000	0	Carga consolidada	23680.00	0	95.00	12.32	1170.40	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e0ceaa50-1da7-4544-8212-86f6fd3a35c8	VJ-2026-000007	86e72142-66cc-461b-94e0-f5daff605e4f	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	1791a784-f779-465e-b5cb-d3c79bbcf836	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-06-24 02:00:00-06	2026-06-24 16:18:23.225806-06	2026-06-24 02:00:00-06	2026-06-24 17:18:23.225806-06	141200.00	141740.75	287.000	369.110	t	9000.000	0	Carga consolidada	17510.00	1	3.20	15.31	1730.40	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e11feb81-c505-40ce-8411-ad49e353cba9	VJ-2026-000059	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-22 03:00:00-06	2026-07-23 02:03:52.258064-06	2026-07-22 03:00:00-06	2026-07-23 00:03:52.258064-06	390915.00	392023.80	482.000	320.440	t	18000.000	0	Carga consolidada	45920.00	0	105.00	21.06	2211.30	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
e525281b-8d5b-4551-8c53-4816c5099a08	VJ-2026-000045	2bced3f7-432c-4465-9b96-c0322c375bc2	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	30c62f14-5d2e-4676-972d-b06f190fc4e6	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-07-16 00:00:00-06	2026-07-16 09:37:44.516129-06	2026-07-16 00:00:00-06	2026-07-16 08:37:44.516129-06	147611.15	147849.65	469.000	650.670	f	26000.000	0	Carga consolidada	8550.00	0	95.00	8.63	819.85	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ea6385c1-81ea-43a1-979a-c8753287d742	VJ-2026-000062	86e72142-66cc-461b-94e0-f5daff605e4f	aa074c2c-fe5a-442a-a6ed-669d6739062f	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-07-23 02:00:00-06	2026-07-23 16:18:23.225806-06	2026-07-23 02:00:00-06	2026-07-23 18:18:23.225806-06	520316.95	520826.80	423.000	527.420	t	25000.000	0	Carga consolidada	21115.00	1	3.20	16.31	1631.52	2	\N	\N	{"tipo_carga": "Refrigerada"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
ee7916ec-1c38-4869-8329-87906bcac270	VJ-2026-000001	705280e1-81dd-4793-b352-fbf5593d35b8	1709cadd-2a88-48cd-bc22-344c9b2949cf	869d6498-3590-45d1-b372-258ac18a65e8	25f2f001-f2e9-427f-8f03-2fbf49847c6b	Culiacán, SIN	Monterrey, NL	1120.00	2026-06-22 07:00:00-06	2026-06-23 07:03:52.258064-06	2026-06-22 07:00:00-06	2026-06-23 03:03:52.258064-06	198700.00	199853.60	489.000	273.820	t	24000.000	0	Carga consolidada	36960.00	0	110.00	20.06	2206.60	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
eedc9484-c795-499d-ad23-4be0a563a8ee	VJ-2026-000008	8222ade5-65e9-4ef5-b185-0c71996cf02c	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	1791a784-f779-465e-b5cb-d3c79bbcf836	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Torreón, COAH	400.00	2026-06-25 05:00:00-06	2026-06-25 15:27:05.806451-06	2026-06-25 05:00:00-06	2026-06-25 17:27:05.806451-06	141740.75	142156.75	407.000	560.830	f	14000.000	0	Carga consolidada	14000.00	3	12.00	12.45	1680.00	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f1b104dd-94ee-4a42-b120-adbf0779abf2	VJ-2026-000034	2bced3f7-432c-4465-9b96-c0322c375bc2	1709cadd-2a88-48cd-bc22-344c9b2949cf	30c62f14-5d2e-4676-972d-b06f190fc4e6	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Torreón, COAH	400.00	2026-07-11 02:00:00-06	2026-07-11 14:27:05.806451-06	2026-07-11 02:00:00-06	2026-07-11 16:27:05.806451-06	201230.00	201642.00	366.000	497.340	f	14000.000	0	Carga consolidada	12400.00	0	95.00	14.45	1372.75	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f31876fa-328e-4f8e-a998-b48296296a55	VJ-2026-000050	705280e1-81dd-4793-b352-fbf5593d35b8	aa074c2c-fe5a-442a-a6ed-669d6739062f	1791a784-f779-465e-b5cb-d3c79bbcf836	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Monterrey, NL	Ciudad de México	920.00	2026-07-18 02:00:00-06	2026-07-18 19:50:19.354838-06	2026-07-18 02:00:00-06	2026-07-18 17:50:19.354838-06	517137.35	518103.35	220.000	133.020	t	11000.000	0	Carga consolidada	35880.00	0	110.00	15.84	1742.40	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f4a8b079-d294-4931-be1c-1f7ca3bc0cda	VJ-2026-000086	705280e1-81dd-4793-b352-fbf5593d35b8	a30fba63-bc02-4c42-b4e4-5c741f6cd72e	1791a784-f779-465e-b5cb-d3c79bbcf836	8e7dbcea-eba1-4a1d-9eb1-006be536cbe8	Saltillo, COAH	Querétaro, QRO	640.00	2026-08-05 02:00:00-06	2026-08-05 18:19:21.290322-06	2026-08-05 02:00:00-06	2026-08-05 16:19:21.290322-06	396087.40	396727.40	229.000	246.440	t	10000.000	0	Carga consolidada	23680.00	0	110.00	14.32	1575.20	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f8b75f3f-f8c4-4ee6-ba82-82ef98204dbb	VJ-2026-000024	86e72142-66cc-461b-94e0-f5daff605e4f	aa074c2c-fe5a-442a-a6ed-669d6739062f	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	Culiacán, SIN	Monterrey, NL	1120.00	2026-07-06 08:00:00-06	2026-07-07 05:03:52.258064-06	2026-07-06 08:00:00-06	2026-07-07 02:03:52.258064-06	514521.15	515697.15	307.000	150.530	t	20000.000	0	Carga consolidada	40320.00	1	3.20	18.06	3763.20	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
f8ddeedb-3b8a-41df-bf00-1560c4864863	VJ-2026-000037	9efa0c7e-03cf-4013-b0e1-36df51f3b84a	aa074c2c-fe5a-442a-a6ed-669d6739062f	30c62f14-5d2e-4676-972d-b06f190fc4e6	cec94ae7-fadf-4db8-981d-80df95ba3774	Monterrey, NL	Torreón, COAH	400.00	2026-07-12 00:00:00-06	2026-07-12 11:27:05.806451-06	2026-07-12 00:00:00-06	2026-07-12 07:27:05.806451-06	515697.15	516101.15	306.000	466.760	f	9000.000	0	Carga consolidada	14800.00	0	105.00	7.45	782.25	2	\N	\N	{"tipo_carga": "General"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fa62a42d-22de-439b-9c01-7a3f3a67886b	VJ-2026-000017	2bced3f7-432c-4465-9b96-c0322c375bc2	cd37e895-ee3c-4737-9cbc-e926b0f4aa53	7e6dee91-bbd5-4138-9bc5-72b54c3fe190	cec94ae7-fadf-4db8-981d-80df95ba3774	San Luis Potosí, SLP	Monterrey, NL	515.00	2026-06-30 05:00:00-06	2026-06-30 18:18:23.225806-06	2026-06-30 05:00:00-06	2026-06-30 20:18:23.225806-06	143056.30	143555.85	428.000	540.710	t	11000.000	0	Carga consolidada	18025.00	0	95.00	15.31	1454.45	2	\N	\N	{"tipo_carga": "Peligrosa"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
fbbf011f-bb9d-4147-9fd9-a34ac87d48ba	VJ-2026-000002	705280e1-81dd-4793-b352-fbf5593d35b8	1709cadd-2a88-48cd-bc22-344c9b2949cf	869d6498-3590-45d1-b372-258ac18a65e8	9bdbb1f6-16af-4427-8c90-7aa445d8f9e1	Monterrey, NL	Nuevo Laredo, TAMPS	225.00	2026-06-22 00:00:00-06	2026-06-22 08:37:44.516129-06	2026-06-22 00:00:00-06	2026-06-22 08:37:44.516129-06	199853.60	200087.60	416.000	632.970	f	15000.000	0	Carga consolidada	9000.00	0	110.00	8.63	949.30	2	\N	\N	{"tipo_carga": "Granel"}	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.889884-06	sistema	\N	\N
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.users ("Id", "Email", "FullName", "PasswordHash", "Role", "LastLoginUtc", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
2f12f881-3694-430b-b197-f15788b2a840	despacho@demo.com	Luis Cárdenas	pbkdf2.120000.3xevn9z/UdL75FJQzh4fvA==.cm8llYRiEGxV3LTzFmVMwTD2AS8xIa8V43GyFLQC03A=	1	\N	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
503b408a-0f66-4b98-896b-4c7a44811277	consulta@demo.com	Mónica Peña	pbkdf2.120000.fY9mSJlxhU3P8Q8+TNPrUQ==.HBC0L5a2AH1vOw/MX53tlt4UgS4/tcBgGIv0OoO8x6o=	2	2026-08-09 09:29:15.160753-06	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-09 09:29:15.160769-06	sistema
079224a1-3872-4572-b5b5-45670c54520e	admin@demo.com	Ana Ramírez	pbkdf2.120000.+tURyk/I74v1ykx74ppb8g==.jRDMifSCkVE7cdIz/a076O2RNUaU4r7NJcZInWVJp8k=	0	2026-08-09 09:29:53.03942-06	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-09 09:29:53.039433-06	sistema
\.


--
-- Data for Name: vehicle_types; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.vehicle_types ("Id", "Code", "Name", "Category", "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
2016c1a7-2488-4fd2-b004-17a96452479a	CAJA53	Caja seca 53 pies	1	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
38970fa1-7099-41e6-9724-98cea8ef2854	TRACTO	Tractocamión	0	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
79a391f1-2680-4720-866e-a0c89c72c38d	RABON	Rabón 8 toneladas	0	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
b034a93e-3403-408b-8f0f-2c5f53967f1f	TORTON	Torton 14 toneladas	0	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
e20da3e3-7ad3-430e-9a9e-6afbf61ed493	REFRI	Caja refrigerada 48 pies	1	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
e54e2b39-155b-4c42-a5de-8ede2a45f6a3	PLATAF	Plataforma	1	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	\N	\N
\.


--
-- Data for Name: vehicles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.vehicles ("Id", "EconomicNumber", "PlateNumber", "VehicleTypeId", "Brand", "Model", "Year", "Vin", "CargoCapacity", "TankCapacity", "CurrentOdometer", "Status", "InsuranceExpiry", "CirculationCardExpiry", custom_fields, "IsActive", "TenantId", "CreatedAtUtc", "CreatedBy", "UpdatedAtUtc", "UpdatedBy") FROM stdin;
1791a784-f779-465e-b5cb-d3c79bbcf836	C-302	EF-222-GH	2016c1a7-2488-4fd2-b004-17a96452479a	Great Dane	Champion	2020	3AKJG853077	28000.000	\N	0.00	1	2027-04-23	2027-06-07	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
869d6498-3590-45d1-b372-258ac18a65e8	C-301	EF-111-GH	2016c1a7-2488-4fd2-b004-17a96452479a	Utility	3000R	2019	3AKJG762498	28000.000	\N	0.00	1	2027-04-03	2027-05-18	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
8f3ca495-c031-43be-88f9-403c2813e1d7	R-201	CD-321-EF	79a391f1-2680-4720-866e-a0c89c72c38d	Isuzu	Forward	2023	3AKJG945943	8000.000	200.000	87693.00	0	2027-06-02	2027-07-17	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
a30fba63-bc02-4c42-b4e4-5c741f6cd72e	T-101	AB-123-CD	38970fa1-7099-41e6-9724-98cea8ef2854	Kenworth	T680	2021	3AKJG659500	30000.000	700.000	397396.15	1	2027-03-04	2027-04-18	{"gps_id": "GPS-0091"}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
aa074c2c-fe5a-442a-a6ed-669d6739062f	T-102	AB-456-CD	38970fa1-7099-41e6-9724-98cea8ef2854	Freightliner	Cascadia	2020	3AKJG926440	30000.000	680.000	524533.15	1	2026-11-09	2026-12-24	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
cd37e895-ee3c-4737-9cbc-e926b0f4aa53	R-202	CD-654-EF	b034a93e-3403-408b-8f0f-2c5f53967f1f	Hino	FM 500	2022	3AKJG359394	14000.000	300.000	150082.35	2	2027-01-03	2027-02-17	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 07:41:37.889884-06	sistema
30c62f14-5d2e-4676-972d-b06f190fc4e6	P-401	EF-444-GH	e54e2b39-155b-4c42-a5de-8ede2a45f6a3	Lufkin	Flatbed 48	2018	3AKJG816548	26000.000	\N	0.00	0	2026-10-05	2026-11-19	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-09 09:29:15.081832-06	admin@demo.com
1709cadd-2a88-48cd-bc22-344c9b2949cf	T-103	AB-789-CD	38970fa1-7099-41e6-9724-98cea8ef2854	International	LT625	2022	3AKJG497298	30000.000	720.000	207444.40	0	2026-08-24	2026-10-08	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 08:34:52.57592-06	admin@demo.com
7e6dee91-bbd5-4138-9bc5-72b54c3fe190	C-303	EF-333-GH	e20da3e3-7ad3-430e-9a9e-6afbf61ed493	Thermo King	Precedent	2021	3AKJG817720	24000.000	\N	0.00	0	2026-12-04	2027-01-18	{}	t	9a2d9571-c52c-40ed-85b2-f5d98581a1e8	2026-08-06 07:41:37.53405-06	sistema	2026-08-06 08:42:14.849871-06	admin@demo.com
\.


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: custom_field_definitions PK_custom_field_definitions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.custom_field_definitions
    ADD CONSTRAINT "PK_custom_field_definitions" PRIMARY KEY ("Id");


--
-- Name: customers PK_customers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customers
    ADD CONSTRAINT "PK_customers" PRIMARY KEY ("Id");


--
-- Name: drivers PK_drivers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.drivers
    ADD CONSTRAINT "PK_drivers" PRIMARY KEY ("Id");


--
-- Name: expense_categories PK_expense_categories; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.expense_categories
    ADD CONSTRAINT "PK_expense_categories" PRIMARY KEY ("Id");


--
-- Name: expenses PK_expenses; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.expenses
    ADD CONSTRAINT "PK_expenses" PRIMARY KEY ("Id");


--
-- Name: fuel_logs PK_fuel_logs; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.fuel_logs
    ADD CONSTRAINT "PK_fuel_logs" PRIMARY KEY ("Id");


--
-- Name: maintenance_orders PK_maintenance_orders; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.maintenance_orders
    ADD CONSTRAINT "PK_maintenance_orders" PRIMARY KEY ("Id");


--
-- Name: tenants PK_tenants; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tenants
    ADD CONSTRAINT "PK_tenants" PRIMARY KEY ("Id");


--
-- Name: trips PK_trips; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT "PK_trips" PRIMARY KEY ("Id");


--
-- Name: users PK_users; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT "PK_users" PRIMARY KEY ("Id");


--
-- Name: vehicle_types PK_vehicle_types; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.vehicle_types
    ADD CONSTRAINT "PK_vehicle_types" PRIMARY KEY ("Id");


--
-- Name: vehicles PK_vehicles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.vehicles
    ADD CONSTRAINT "PK_vehicles" PRIMARY KEY ("Id");


--
-- Name: IX_custom_field_definitions_TenantId_Target_Key; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_custom_field_definitions_TenantId_Target_Key" ON public.custom_field_definitions USING btree ("TenantId", "Target", "Key");


--
-- Name: IX_customers_TenantId_Name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_customers_TenantId_Name" ON public.customers USING btree ("TenantId", "Name");


--
-- Name: IX_drivers_TenantId_LicenseNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_drivers_TenantId_LicenseNumber" ON public.drivers USING btree ("TenantId", "LicenseNumber");


--
-- Name: IX_drivers_TenantId_Status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_drivers_TenantId_Status" ON public.drivers USING btree ("TenantId", "Status");


--
-- Name: IX_expense_categories_TenantId_Code; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_expense_categories_TenantId_Code" ON public.expense_categories USING btree ("TenantId", "Code");


--
-- Name: IX_expenses_CategoryId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_expenses_CategoryId" ON public.expenses USING btree ("CategoryId");


--
-- Name: IX_expenses_DriverId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_expenses_DriverId" ON public.expenses USING btree ("DriverId");


--
-- Name: IX_expenses_TenantId_CategoryId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_expenses_TenantId_CategoryId" ON public.expenses USING btree ("TenantId", "CategoryId");


--
-- Name: IX_expenses_TenantId_IncurredAtUtc; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_expenses_TenantId_IncurredAtUtc" ON public.expenses USING btree ("TenantId", "IncurredAtUtc");


--
-- Name: IX_expenses_TripId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_expenses_TripId" ON public.expenses USING btree ("TripId");


--
-- Name: IX_expenses_VehicleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_expenses_VehicleId" ON public.expenses USING btree ("VehicleId");


--
-- Name: IX_fuel_logs_DriverId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_fuel_logs_DriverId" ON public.fuel_logs USING btree ("DriverId");


--
-- Name: IX_fuel_logs_TenantId_LoadedAtUtc; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_fuel_logs_TenantId_LoadedAtUtc" ON public.fuel_logs USING btree ("TenantId", "LoadedAtUtc");


--
-- Name: IX_fuel_logs_TenantId_VehicleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_fuel_logs_TenantId_VehicleId" ON public.fuel_logs USING btree ("TenantId", "VehicleId");


--
-- Name: IX_fuel_logs_TripId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_fuel_logs_TripId" ON public.fuel_logs USING btree ("TripId");


--
-- Name: IX_fuel_logs_VehicleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_fuel_logs_VehicleId" ON public.fuel_logs USING btree ("VehicleId");


--
-- Name: IX_maintenance_orders_TenantId_Folio; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_maintenance_orders_TenantId_Folio" ON public.maintenance_orders USING btree ("TenantId", "Folio");


--
-- Name: IX_maintenance_orders_TenantId_Status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_maintenance_orders_TenantId_Status" ON public.maintenance_orders USING btree ("TenantId", "Status");


--
-- Name: IX_maintenance_orders_VehicleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_maintenance_orders_VehicleId" ON public.maintenance_orders USING btree ("VehicleId");


--
-- Name: IX_tenants_Slug; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_tenants_Slug" ON public.tenants USING btree ("Slug");


--
-- Name: IX_trips_CustomerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_trips_CustomerId" ON public.trips USING btree ("CustomerId");


--
-- Name: IX_trips_DriverId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_trips_DriverId" ON public.trips USING btree ("DriverId");


--
-- Name: IX_trips_TenantId_DriverId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_trips_TenantId_DriverId" ON public.trips USING btree ("TenantId", "DriverId");


--
-- Name: IX_trips_TenantId_Folio; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_trips_TenantId_Folio" ON public.trips USING btree ("TenantId", "Folio");


--
-- Name: IX_trips_TenantId_ScheduledDepartureUtc; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_trips_TenantId_ScheduledDepartureUtc" ON public.trips USING btree ("TenantId", "ScheduledDepartureUtc");


--
-- Name: IX_trips_TenantId_Status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_trips_TenantId_Status" ON public.trips USING btree ("TenantId", "Status");


--
-- Name: IX_trips_TrailerId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_trips_TrailerId" ON public.trips USING btree ("TrailerId");


--
-- Name: IX_trips_VehicleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_trips_VehicleId" ON public.trips USING btree ("VehicleId");


--
-- Name: IX_users_TenantId_Email; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_users_TenantId_Email" ON public.users USING btree ("TenantId", "Email");


--
-- Name: IX_vehicle_types_TenantId_Code; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_vehicle_types_TenantId_Code" ON public.vehicle_types USING btree ("TenantId", "Code");


--
-- Name: IX_vehicles_TenantId_EconomicNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_vehicles_TenantId_EconomicNumber" ON public.vehicles USING btree ("TenantId", "EconomicNumber");


--
-- Name: IX_vehicles_TenantId_PlateNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_vehicles_TenantId_PlateNumber" ON public.vehicles USING btree ("TenantId", "PlateNumber");


--
-- Name: IX_vehicles_TenantId_Status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_vehicles_TenantId_Status" ON public.vehicles USING btree ("TenantId", "Status");


--
-- Name: IX_vehicles_VehicleTypeId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_vehicles_VehicleTypeId" ON public.vehicles USING btree ("VehicleTypeId");


--
-- Name: expenses FK_expenses_drivers_DriverId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.expenses
    ADD CONSTRAINT "FK_expenses_drivers_DriverId" FOREIGN KEY ("DriverId") REFERENCES public.drivers("Id") ON DELETE RESTRICT;


--
-- Name: expenses FK_expenses_expense_categories_CategoryId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.expenses
    ADD CONSTRAINT "FK_expenses_expense_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES public.expense_categories("Id") ON DELETE RESTRICT;


--
-- Name: expenses FK_expenses_trips_TripId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.expenses
    ADD CONSTRAINT "FK_expenses_trips_TripId" FOREIGN KEY ("TripId") REFERENCES public.trips("Id") ON DELETE CASCADE;


--
-- Name: expenses FK_expenses_vehicles_VehicleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.expenses
    ADD CONSTRAINT "FK_expenses_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES public.vehicles("Id") ON DELETE RESTRICT;


--
-- Name: fuel_logs FK_fuel_logs_drivers_DriverId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.fuel_logs
    ADD CONSTRAINT "FK_fuel_logs_drivers_DriverId" FOREIGN KEY ("DriverId") REFERENCES public.drivers("Id") ON DELETE RESTRICT;


--
-- Name: fuel_logs FK_fuel_logs_trips_TripId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.fuel_logs
    ADD CONSTRAINT "FK_fuel_logs_trips_TripId" FOREIGN KEY ("TripId") REFERENCES public.trips("Id") ON DELETE CASCADE;


--
-- Name: fuel_logs FK_fuel_logs_vehicles_VehicleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.fuel_logs
    ADD CONSTRAINT "FK_fuel_logs_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES public.vehicles("Id") ON DELETE RESTRICT;


--
-- Name: maintenance_orders FK_maintenance_orders_vehicles_VehicleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.maintenance_orders
    ADD CONSTRAINT "FK_maintenance_orders_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES public.vehicles("Id") ON DELETE RESTRICT;


--
-- Name: trips FK_trips_customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT "FK_trips_customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public.customers("Id") ON DELETE RESTRICT;


--
-- Name: trips FK_trips_drivers_DriverId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT "FK_trips_drivers_DriverId" FOREIGN KEY ("DriverId") REFERENCES public.drivers("Id") ON DELETE RESTRICT;


--
-- Name: trips FK_trips_vehicles_TrailerId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT "FK_trips_vehicles_TrailerId" FOREIGN KEY ("TrailerId") REFERENCES public.vehicles("Id") ON DELETE RESTRICT;


--
-- Name: trips FK_trips_vehicles_VehicleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT "FK_trips_vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES public.vehicles("Id") ON DELETE RESTRICT;


--
-- Name: vehicles FK_vehicles_vehicle_types_VehicleTypeId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.vehicles
    ADD CONSTRAINT "FK_vehicles_vehicle_types_VehicleTypeId" FOREIGN KEY ("VehicleTypeId") REFERENCES public.vehicle_types("Id") ON DELETE RESTRICT;


--
-- PostgreSQL database dump complete
--

\unrestrict 8nn3x1UJbSJ1gd9kgzkxm52RfdvfjVplftv88mUhMKIhmI4pg0GmvNzkGZQZh3B

