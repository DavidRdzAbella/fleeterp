# FleetERP · control de flotilla

MVP de ERP para transportistas: inventario de unidades, padrón de operadores,
control de viajes de punta a punta y los tableros que responden **cómo va la
flotilla**. Pensado para demostrarse ante varias empresas y ponerse en marcha en
cada una configurando datos, no escribiendo código.

---

## Arrancar

La configuración de `appsettings.json` apunta a **PostgreSQL en `localhost:5432`,
base `fleeterp`, usuario `postgres`**. El esquema ya está aplicado y la empresa
de demostración sembrada.

```bash
dotnet run --project src/FleetErp.Api/FleetErp.Api.csproj --urls http://localhost:5200
```

```bash
dotnet run --project src/FleetErp.Web/FleetErp.Web.csproj --urls http://localhost:5100
```

Portal en `http://localhost:5100` · API y Swagger en `http://localhost:5200/swagger`

| Empresa | Usuario | Contraseña | Perfil |
|---|---|---|---|
| `demo` | `admin@demo.com` | `Demo123$` | Administrador |
| `demo` | `despacho@demo.com` | `Demo123$` | Despachador |
| `demo` | `consulta@demo.com` | `Demo123$` | Solo consulta |

Entrar con los tres muestra cómo cambia la interfaz según el permiso: consulta ve
los tableros pero ningún botón de captura, y no ve el menú de configuración.

### Levantar la base desde cero

La forma más rápida es restaurar el respaldo completo, que trae estructura y
datos de demostración en un solo archivo:

```bash
psql -h localhost -p 5432 -U postgres -c "CREATE DATABASE fleeterp;"
```

```bash
psql -h localhost -p 5432 -U postgres -d fleeterp -f deploy/fleeterp-demo.sql
```

Queda lista para entrar: una empresa, 3 usuarios, 9 unidades, 6 operadores,
4 clientes y 92 viajes con sus cargas de combustible, gastos y órdenes de taller.

**Restaure siempre sobre una base vacía.** El respaldo no lleva `DROP`, así que
aplicarlo encima de datos existentes falla por llaves duplicadas en lugar de
sobrescribir — que es el comportamiento seguro.

| Archivo | Qué trae | Cuándo usarlo |
|---|---|---|
| [`deploy/fleeterp-demo.sql`](deploy/fleeterp-demo.sql) | Estructura **y** datos de demostración | Montar el demo en otra máquina o volver al punto de partida |
| [`deploy/schema.sql`](deploy/schema.sql) | Solo estructura, idempotente | Instalar en un cliente real, que arranca con su propia información |

Para volver a generar el respaldo después de cambiar los datos:

```bash
pg_dump -h localhost -p 5432 -U postgres -d fleeterp --no-owner --no-privileges --encoding=UTF8 --file=deploy/fleeterp-demo.sql
```

Si prefiere partir de la estructura vacía y que el sistema siembre los datos,
aplique `deploy/schema.sql` y arranque la API una vez con
`Database__SeedDemoData=true`; el sembrado no duplica nada si la empresa ya existe.

### Sin instalar nada (para enseñarlo en otra máquina)

Poniendo `Database:Provider` en `InMemory` y `Database:SeedDemoData` en `true`
—como ya viene en `appsettings.Development.json`— la API levanta con base en
memoria y datos listos, sin necesidad de PostgreSQL. Es la forma de llevar el
demo a la laptop de un cliente.

### Con Docker

```bash
docker compose up --build
```

Levanta PostgreSQL 16, la API y el portal en contenedores separados.

---

## Qué pidió el cliente y dónde quedó

Requerimientos levantados de la nota de voz, y la pantalla que los resuelve.

| Lo que pidió | Dónde está |
|---|---|
| Control e inventario de unidades: tractocamiones, cajas y remolques | **Unidades**. Una sola tabla; el tipo (catálogo por empresa) define si es motriz o de arrastre |
| Registro de conductores | **Conductores**, con licencia, vigencia y esquema de pago propio |
| Pantalla de viaje con hora de salida y de llegada | **Viajes → detalle**: se despacha y se cierra con hora real |
| Kilómetros por recorrer | Campo del viaje; se contrasta contra la distancia real del odómetro |
| Combustible inicial y si va a cargar gasolina o no | Campos del viaje; las cargas en ruta se registran en el detalle |
| Kilogramos o toneladas de carga | Peso más su unidad, que se elige al capturar |
| Destino del viaje | Origen y destino, con distancia planeada |
| Gráfico por conductor: kilómetros de la semana, gasto de combustible, cuánto vendió, ganancia de la empresa | **Conductores → desempeño**, con selector de periodo de 7 a 90 días |
| Dashboard con todos los conductores y su posición (top 1, 2, 3) | **Conductores → ranking**, ordenable por distancia, venta, utilidad, viajes o rendimiento |
| Pantalla de gasto total en combustible, ganancias, lo que se le pagó por hora al chofer y total de kilómetros | **Gastos y ganancias**, con nómina desglosada por operador |
| Ver cómo va toda la flotilla: las entradas y las salidas | **Tablero de flotilla**: tira de despacho con cada unidad y su estado, más salidas y llegadas del día |
| Que sirva a una empresa que va empezando | Multi‑empresa, catálogos y campos configurables, y datos de demostración listos |

---

## Módulos y cómo se capturan

Todos los módulos de captura comparten la misma **mesa de trabajo**: a la
izquierda la lista con su buscador, a la derecha la ficha del registro
seleccionado. Se eligió sobre la tabla clásica porque dar de alta o corregir no
obliga a cambiar de pantalla: se ve el contexto y el detalle al mismo tiempo.

La botonera es igual en todos: **+ Nuevo** arriba a la derecha, y en la ficha
**Editar**, **Guardar**, **Cancelar** y **Dar de baja** (o **Eliminar**, según
corresponda). El estado vive en la dirección — `/Unidades/Index/{id}?mode=edit` —
así que la pantalla es compartible y el botón de regresar del navegador funciona.

| Módulo | Alta | Edición | Baja | Notas |
|---|---|---|---|---|
| Unidades | Sí | Sí | Baja lógica y reactivación | Además, mandar a taller o marcar fuera de servicio |
| Conductores | Sí | Sí | Baja lógica y reactivación | La ficha muestra su resultado de los últimos 30 días |
| Clientes | Sí | Sí | Baja lógica y reactivación | |
| Usuarios | Sí | Sí | Desactivar y reactivar | Restablecer contraseña es acción aparte |
| Mantenimiento | Sí | — | Cierre de la orden | Una orden es evidencia del gasto: se cierra, no se reescribe |
| Combustible | Sí | Sí | Eliminar | Fuente única de litros y costo de diésel |
| Gastos | Sí | Sí | Eliminar | Todo lo que no es combustible |
| Catálogos | Sí | Sí | Desactivar y reactivar | Tipos de unidad, conceptos de gasto y campos a la medida |
| Viajes | Sí | Sí | Cancelar | Conserva su lista y su pantalla de despacho: es un flujo de trabajo, no un catálogo |

**Por qué las bajas son lógicas.** Los catálogos y padrones tienen movimientos
históricos que los citan; borrarlos dejaría viajes sin conductor y reportes sin
concepto. Se desactivan, desaparecen de los combos, y el mismo botón los
reactiva. Combustible y gastos sí se eliminan de verdad: son movimientos
puntuales que se capturan mal y se corrigen.

**Por qué Viajes no usa la mesa de trabajo.** No es un catálogo sino un
documento con ciclo de vida —planear, despachar, cerrar— y su pantalla de
detalle carga formularios de salida y llegada, cargas de combustible y gastos de
ruta. Meterlo en un panel lateral lo apretaría sin ganar nada.

---

## Cómo se adapta a otra empresa

Todo lo que cambia entre un transportista y otro es dato, no código. Se hace
desde **Parámetros de la empresa** sin recompilar ni migrar:

- **Multi‑empresa real.** Cada empresa es un *tenant* con sus datos aislados por
  filtro global en el contexto de datos. Una misma instalación atiende a varias.
- **Unidades de medida y moneda.** Kilómetros o millas, litros o galones,
  kilogramos o toneladas, moneda y símbolo, zona horaria y formato regional.
- **Catálogos propios.** Tipos de unidad (tractocamión, rabón, caja, pipa,
  dolly…) y conceptos de gasto (casetas, viáticos, maniobras, multas…).
- **Campos a la medida.** Se declara un campo —texto, número, fecha, sí/no o
  lista— y aparece solo en los formularios de viaje, unidad, conductor o cliente.
  Se guarda en una columna `jsonb`, así que no hay migración de por medio.
- **Esquemas de pago.** Por hora, por distancia, monto fijo por viaje o
  porcentaje del flete; conviven en la misma empresa y se congelan en el viaje
  para que un aumento no recalcule la nómina histórica.
- **Marca visual.** El color primario sale de la parametrización y recolorea el
  portal completo.
- **Folios.** Prefijo configurable; el consecutivo es por empresa y por año.

---

## Arquitectura

Dos aplicaciones desplegables por separado sobre una solución en capas.

```
FleetErp.Web  ──HTTP/JWT──►  FleetErp.Api
  (MVC, Razor)                 (Web API)
                                   │
                    ┌──────────────┴──────────────┐
                    │      FleetErp.Application    │  casos de uso, DTOs,
                    │                              │  validación, tableros
                    ├──────────────┬───────────────┤
   FleetErp.Infrastructure         │      FleetErp.Domain
   (EF Core · PostgreSQL)          │      (entidades y reglas)
```

Las dependencias apuntan siempre hacia adentro. El dominio no conoce a nadie; la
aplicación define **puertos** (`IRepository`, `IUnitOfWork`, `ICurrentTenant`,
`IClock`, `IAnalyticsDataSource`, `ITripQueries`…) y la infraestructura los
implementa. Cambiar de PostgreSQL a otro motor, o del hash de contraseñas a otro
algoritmo, no toca las reglas de negocio.

| Proyecto | Responsabilidad |
|---|---|
| `FleetErp.Domain` | Entidades con comportamiento, invariantes y máquina de estados del viaje. Sin dependencias |
| `FleetErp.Application` | Casos de uso, contratos, validadores y toda la aritmética de los tableros |
| `FleetErp.Infrastructure` | EF Core sobre PostgreSQL, repositorios, consultas de lectura, JWT, folios, semilla |
| `FleetErp.Api` | Web API con JWT, políticas por rol, Swagger y traducción de errores a `ProblemDetails` |
| `FleetErp.Web` | Portal MVC; consume la API por HTTP y define sus propios modelos |
| `FleetErp.UnitTests` | 67 pruebas sobre reglas del dominio, cálculos de los tableros y firma de tokens |

**Por qué el portal duplica los DTOs.** No referencia ningún proyecto del
backend: su contrato es el JSON de la API. Esa duplicación deliberada es lo que
permite desplegarlos, versionarlos y escalarlos por separado.

### Decisiones que conviene conocer

- **CQRS ligero.** Escritura por servicios de aplicación sobre agregados; lectura
  por puertos de consulta que proyectan directo. Tienen razones de cambio
  distintas y se separaron por eso.
- **Los tableros se calculan en la capa de aplicación** a partir de hechos que
  entrega `IAnalyticsDataSource`. Es código puro y por eso está cubierto de
  pruebas. Para volúmenes grandes se sustituye la implementación del puerto por
  una que agregue en SQL, sin tocar el tablero.
- **Combustible y gastos van separados.** `FuelLog` es la única fuente de litros
  y costo de diésel —de ahí sale el rendimiento—; `Expense` cubre el resto. Así
  ningún importe se cuenta dos veces.
- **Errores de negocio, no 500.** El dominio lanza excepciones expresivas y un
  middleware las traduce a 404, 409 o 422 con mensaje en español.
- **El tenant viaja firmado en el JWT**, nunca en una cabecera que el cliente
  pueda escribir.
- **La llave de firma se deriva con SHA-256** del secreto configurado, en vez de
  usar sus bytes literales. HMAC-SHA256 exige 256 bits: sin derivar, un secreto
  de menos de 32 caracteres haría fallar la emisión del token en tiempo de
  ejecución. Derivarlo admite cualquier longitud y produce siempre el mismo
  resultado, así que quien firma y quien valida coinciden. Ojo: **derivar no crea
  entropía** — un secreto corto sigue siendo débil.

---

## Interfaz

Consola de operación, no sitio de marketing. Verde de señalamiento carretero y
ámbar de advertencia sobre gris papel; el color se reserva para comunicar estado.
Títulos en Bahnschrift (la DIN de los señalamientos viales, incluida en Windows)
y **toda cifra en monoespaciada tabular**, porque importes y odómetros se leen en
columna y deben alinearse al dígito.

El elemento distintivo es la **tira de despacho** del tablero: cada unidad como
una placa vehicular coloreada por estado. Es "ver cómo va toda la flotilla" en un
golpe de vista.

Además: teclado navegable con foco visible, `prefers-reduced-motion` respetado,
diseño adaptable hasta móvil, hoja de impresión para los reportes, y estados
vacíos que dicen qué hacer en lugar de quedarse en blanco. Chart.js va embebido
en el proyecto, así que el demo funciona sin internet.

---

## Comandos útiles

```bash
dotnet build FleetErp.sln
```

```bash
dotnet test tests/FleetErp.UnitTests/FleetErp.UnitTests.csproj
```

```bash
dotnet ef migrations add NombreDelCambio --project src/FleetErp.Infrastructure --startup-project src/FleetErp.Infrastructure --output-dir Persistence/Migrations
```

---

## Antes de llevarlo a producción

Es un MVP para demostración; lo que falta es deliberado y está acotado:

1. **Sacar `Jwt:SigningKey` del `appsettings.json`** a un gestor de secretos, y
   usar una cadena larga y aleatoria. La actual es corta y está en el repositorio.
2. **Credenciales de base de datos**: hoy corre con el superusuario `postgres`.
   En producción conviene un rol propio con permisos solo sobre `fleeterp`.
3. **HTTPS**; el portal y la API hablan HTTP en la configuración actual.
4. **Refresco de token**: la sesión dura ocho horas y luego pide entrar de nuevo.
5. **Pruebas de integración** sobre la API con `WebApplicationFactory`; el punto
   de extensión ya está declarado en `Program`.
6. **Consecutivo de folios con secuencia de PostgreSQL** si va a haber muchas
   altas concurrentes. Hoy el índice único evita el duplicado, pero una colisión
   obliga a reintentar.
7. **Bitácora de auditoría consultable**: las entidades ya guardan quién y cuándo,
   falta exponerlo.
8. **Paginación en las listas maestras**: hoy traen hasta 200 registros de un
   golpe, suficiente para una flotilla mediana pero no para miles de movimientos.
