# MercadoMAX

Plataforma digital integral para mercados mayoristas que conecta proveedores, transportistas, encargados de recepción, estibadores y comerciantes (dueños de puestos).

---

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Backend | .NET 10 — Web API (Microservicios) |
| Frontend | Angular 21 (standalone components, signals) |
| Base de Datos | SQL Server (LocalDB) |
| ORM | Dapper + Stored Procedures |
| Autenticación | JWT + Refresh Tokens |
| API Gateway | Ocelot |
| Documentación API | Swagger / OpenAPI |
| Contenedores | Docker Compose |

---

## Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│         Frontend Angular (:4200) — MercadoMax.Front          │
├─────────────────────────────────────────────────────────────┤
│                  API Gateway Ocelot (:5000)                  │
├──────────┬──────────┬──────────┬──────────┬─────────────────┤
│  Auth    │ Maestros │  Guías   │Transport │  Recepción      │
│  API     │  API     │  API     │  API     │  API            │
│  :5001   │  :5002   │  :5003   │  :5004   │  :5005          │
├──────────┴─────┬────┴──────────┼──────────┴─────────────────┤
│ Comerciante    │  Finanzas     │  Shared Library            │
│ API :5006      │  API :5007    │  (+ Cross-Cutting)         │
├────────────────┴───────────────┴────────────────────────────┤
│              SQL Server — MercadoMAX_DB                     │
│              Schemas: auth, maestros, guias,                │
│              transporte, recepcion, comerciante, finanzas   │
└─────────────────────────────────────────────────────────────┘
```

---

## Microservicios

| # | Servicio | Puerto | Schema BD | Responsabilidad |
|---|----------|--------|-----------|-----------------|
| 1 | Auth.API | 5001 | `auth` | Usuarios, roles, JWT, permisos |
| 2 | Maestros.API | 5002 | `master` | Productos, proveedores, puestos, pabellones |
| 3 | Guias.API | 5003 | `guide` | Guías de envío, detalle, multi-destino |
| 4 | Transporte.API | 5004 | `transport` | Transportistas, camiones, tarifas |
| 5 | Recepcion.API | 5005 | `reception` | Llegada, descarga, faltantes |
| 6 | Comerciante.API | 5006 | `merchant` | Confirmación recepción, inventario |
| 7 | Finanzas.API | 5007 | `finance` | Ventas, cuentas por pagar, pagos, reportes |
| — | Gateway | 5000 | — | Punto de entrada único (Ocelot), rate limiting, health checks |

---

## Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (opcional, para levantar todo con compose)
- [Node.js 20+](https://nodejs.org/) y npm
- [Angular CLI 21+](https://angular.dev/) (`npm i -g @angular/cli`)
- SQL Server (LocalDB o instancia completa)

---

## Configuración de Base de Datos

Los scripts de base de datos no viven en este repositorio ni en el del
frontend: son comunes a los dos y se mantienen en una carpeta aparte,
`ScriptBD/`, junto a ambas copias de trabajo.

Ejecutarlos en orden:

```powershell
# Desde SQL Server Management Studio o sqlcmd, ejecutar en orden:
01_CreateDatabase.sql
02_Tables_Auth.sql
03_SP_Auth.sql
04_SeedData_Auth.sql
05_Tables_Master.sql
06_SP_Master.sql
07_SeedData_Master.sql
08_Tables_Guide.sql
09_SP_Guide.sql
10_Tables_Transport.sql
11_SP_Transport.sql
12_Tables_Reception.sql
13_SP_Reception.sql
14_Tables_Merchant.sql
15_SP_Merchant.sql
16_Tables_Finance.sql
17_SP_Finance.sql
18_PasswordReset_Auth.sql
```

La cadena de conexión por defecto apunta a LocalDB:
```
Server=(localdb)\MSSQLLocalDB;Database=MercadoMAX_DB;Trusted_Connection=True;
```

---

## Levantar el Proyecto

### Todo el entorno con Docker

```powershell
docker compose up --build
# Gateway → http://localhost:5000   Frontend → http://localhost:4200
```

El servicio `frontend` se construye desde el repositorio MercadoMax.Front. Si
lo clonas en otra ruta, indícala con `FRONTEND_PATH`.

### Backend (microservicio por microservicio)

Desde la raíz de este repositorio, abrir terminales independientes:

```powershell
# Auth API — http://localhost:5001
cd MercadoMAX.Auth.API
dotnet run

# Maestros API — http://localhost:5002
cd MercadoMAX.Maestros.API
dotnet run

# Guias API — http://localhost:5003
cd MercadoMAX.Guias.API
dotnet run

# Transporte API — http://localhost:5004
cd MercadoMAX.Transporte.API
dotnet run

# Recepcion API — http://localhost:5005
cd MercadoMAX.Recepcion.API
dotnet run

# Comerciante API — http://localhost:5006
cd MercadoMAX.Comerciante.API
dotnet run

# Finanzas API — http://localhost:5007
cd MercadoMAX.Finanzas.API
dotnet run

# Gateway — http://localhost:5000
cd MercadoMAX.Gateway
dotnet run
```

### Frontend

El cliente Angular vive en su propio repositorio:
[MercadoMax.Front](https://github.com/HenryCarrascoMedina-maco/MercadoMax.Front).

```powershell
cd ../Frontend/mercado-max   # o donde hayas clonado MercadoMax.Front
npm install
npm start
# → http://localhost:4200
```

---

## Swagger (Documentación API)

Cada microservicio expone su documentación Swagger en desarrollo:

| Servicio | URL |
|----------|-----|
| Auth | http://localhost:5001/swagger |
| Maestros | http://localhost:5002/swagger |
| Guias | http://localhost:5003/swagger |
| Transporte | http://localhost:5004/swagger |
| Recepcion | http://localhost:5005/swagger |
| Comerciante | http://localhost:5006/swagger |
| Finanzas | http://localhost:5007/swagger |

---

## Estructura del Proyecto

El proyecto vive en dos repositorios:

| Repositorio | Contenido |
|-------------|-----------|
| [MercadoMax.Back](https://github.com/HenryCarrascoMedina-maco/MercadoMax.Back) | Microservicios .NET, gateway, compose y documentación |
| [MercadoMax.Front](https://github.com/HenryCarrascoMedina-maco/MercadoMax.Front) | SPA Angular 21 |

Los scripts SQL quedan fuera de ambos, en una carpeta `ScriptBD/` al lado de
las dos copias de trabajo, porque la base de datos es una sola y no pertenece
a ninguno de los dos lados.

```
MercadoMax.Back/
├── MercadoMAX.Auth.API/           # Microservicio Auth
├── MercadoMAX.Maestros.API/       # Microservicio Maestros
├── MercadoMAX.Guias.API/          # Microservicio Guías
├── MercadoMAX.Transporte.API/     # Microservicio Transporte
├── MercadoMAX.Recepcion.API/      # Microservicio Recepción
├── MercadoMAX.Comerciante.API/    # Microservicio Comerciante
├── MercadoMAX.Finanzas.API/       # Microservicio Finanzas
├── MercadoMAX.Gateway/            # API Gateway (Ocelot)
├── MercadoMAX.Shared/             # Librería compartida (DTOs, Data, Cross-Cutting)
├── docs/                          # Plan, guías, flujo y reportes de fases
├── docker-compose.yml             # Entorno completo (BD, APIs, gateway, front)
├── MercadoMAX.slnx                # Solución
└── README.md                      # Este archivo
```

---

## Roles del Sistema

| Rol | Accesos |
|-----|---------|
| Administrador | Acceso total, gestión de usuarios y configuración |
| Proveedor | Crear guías, ver estado de envíos, historial |
| Transportista | Ver guías asignadas, tarifas, liquidaciones |
| EncargadoRecepcion | Registrar llegada, validar carga, registrar faltantes |
| Comerciante | Confirmar recepción, ver inventario, ventas, deudas |

---

## Convenciones

- **Stored Procedures**: `SP_{OPERACION}_{ENTIDAD}` (ej: `SP_CREATE_PRODUCTO`, `SP_LIST_PUESTO_BY_PABELLON`)
- **Eliminación lógica**: Campo `Estado` en lugar de DELETE físico
- **Schemas separados** por microservicio en una sola BD
- **Standalone components** en Angular (sin NgModules)
