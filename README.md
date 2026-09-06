# MercadoMAX

Plataforma digital integral para mercados mayoristas que conecta proveedores, transportistas, encargados de recepción, estibadores y comerciantes (dueños de puestos).

---

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Backend | .NET 8 — Web API (Microservicios) |
| Frontend | Angular 21 (standalone components, signals) |
| Base de Datos | SQL Server (LocalDB) |
| ORM | Dapper + Stored Procedures |
| Autenticación | JWT + Refresh Tokens |
| Documentación API | Swagger / OpenAPI |

---

## Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                  Frontend Angular (:4200)                    │
├──────────┬──────────┬──────────┬──────────┬─────────────────┤
│  Auth    │ Maestros │  Guías   │Transport │  Recepción      │
│  API     │  API     │  API     │  API     │  API            │
│  :5001   │  :5002   │  :5003   │  :5004   │  :5005          │
├──────────┴──────────┴──────────┴──────────┼─────────────────┤
│  Comerciante API :5006                    │  Shared Library │
├───────────────────────────────────────────┴─────────────────┤
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
| 2 | Maestros.API | 5002 | `maestros` | Productos, proveedores, puestos, pabellones |
| 3 | Guias.API | 5003 | `guias` | Guías de envío, detalle, multi-destino |
| 4 | Transporte.API | 5004 | `transporte` | Transportistas, camiones, tarifas |
| 5 | Recepcion.API | 5005 | `recepcion` | Llegada, descarga, faltantes |
| 6 | Comerciante.API | 5006 | `comerciante` | Confirmación recepción, inventario |

---

## Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) y npm
- [Angular CLI 21+](https://angular.dev/) (`npm i -g @angular/cli`)
- SQL Server (LocalDB o instancia completa)

---

## Configuración de Base de Datos

Ejecutar los scripts SQL en orden desde la carpeta `ScriptBD/`:

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

### Backend (todos los microservicios)

Desde la carpeta `Backend/`, abrir terminales independientes para cada servicio:

```powershell
# Auth API — http://localhost:5001
cd Backend/MercadoMAX.Auth.API
dotnet run

# Maestros API — http://localhost:5002
cd Backend/MercadoMAX.Maestros.API
dotnet run

# Guias API — http://localhost:5003
cd Backend/MercadoMAX.Guias.API
dotnet run

# Transporte API — http://localhost:5004
cd Backend/MercadoMAX.Transporte.API
dotnet run

# Recepcion API — http://localhost:5005
cd Backend/MercadoMAX.Recepcion.API
dotnet run

# Comerciante API — http://localhost:5006
cd Backend/MercadoMAX.Comerciante.API
dotnet run
```

### Frontend

```powershell
cd Frontend/mercado-max
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

---

## Estructura del Proyecto

```
MercadoMAX/
├── Backend/
│   ├── MercadoMAX.Auth.API/          # Microservicio Auth
│   ├── MercadoMAX.Maestros.API/      # Microservicio Maestros
│   ├── MercadoMAX.Guias.API/         # Microservicio Guías
│   ├── MercadoMAX.Transporte.API/    # Microservicio Transporte
│   ├── MercadoMAX.Recepcion.API/     # Microservicio Recepción
│   ├── MercadoMAX.Comerciante.API/   # Microservicio Comerciante
│   ├── MercadoMAX.Shared/            # Librería compartida (DTOs, Data)
│   └── MercadoMAX.slnx               # Solución
├── Frontend/
│   └── mercado-max/                   # Angular 21 SPA
├── ScriptBD/                          # Scripts SQL (tablas, SPs, seed data)
├── PLAN_PROYECTO.md                   # Plan detallado del proyecto
└── README.md                          # Este archivo
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
