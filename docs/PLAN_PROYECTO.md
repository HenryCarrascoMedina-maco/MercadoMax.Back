# 🏗️ Plan de Construcción — Sistema de Gestión para Mercado Mayorista

## 📌 Nombre del Proyecto: **MercadoMAX**

> Plataforma digital integral para mercados mayoristas que conecta proveedores, transportistas, encargados de recepción, estibadores y comerciantes (dueños de puestos).

---

## 🧰 Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| **Backend** | .NET 8 — Web API (Arquitectura Clean / Microservicios) |
| **Frontend** | Angular 19+ (standalone components, signals) |
| **Base de Datos** | SQL Server 2022 |
| **ORM** | Dapper (consultas) + procedimientos almacenados |
| **Auth** | JWT + Refresh Tokens |
| **Contenedores** | Docker + Docker Compose |
| **Documentación API** | Swagger / OpenAPI |
| **Control de Versiones** | Git |

---

## 🏛️ Arquitectura de Microservicios

```
┌─────────────────────────────────────────────────────────────┐
│                     API GATEWAY (Ocelot)                    │
│                      Puerto: 5000                           │
├──────────┬──────────┬──────────┬──────────┬─────────────────┤
│  Auth    │  Guías   │Transport │Recepción │  Comerciante    │
│  Service │  Service │  Service │  Service │  Service        │
│  :5001   │  :5002   │  :5003   │  :5004   │  :5005          │
├──────────┴──────────┴──────────┴──────────┴─────────────────┤
│              SQL Server (MercadoMAX_DB)                     │
│              Schemas por microservicio                      │
└─────────────────────────────────────────────────────────────┘
```

### Microservicios planificados

| # | Microservicio | Schema BD | Responsabilidad |
|---|--------------|-----------|-----------------|
| 1 | **Auth.API** | `auth` | Usuarios, roles, JWT, permisos |
| 2 | **Maestros.API** | `maestros` | Productos, proveedores, puestos, pabellones, unidades logísticas |
| 3 | **Guias.API** | `guias` | Guías de envío, detalle, multi-destino |
| 4 | **Transporte.API** | `transporte` | Transportistas, camiones, tarifas, liquidación |
| 5 | **Recepcion.API** | `recepcion` | Llegada, descarga, faltantes, validación |
| 6 | **Comerciante.API** | `comerciante` | Confirmación recepción, inventario |
| 7 | **Finanzas.API** *(Fase 4)* | `finanzas` | Ventas, deudas, pagos, estado de cuenta |

> **Nota:** Se usa una sola BD con schemas separados para simplificar el deploy inicial. Se puede migrar a BDs independientes luego.

---

## 📊 Modelo Entidad-Relación (ERD)

### Schema: `auth`

```
┌──────────────────────┐     ┌───────────────────────┐
│      Usuario         │     │        Rol             │
├──────────────────────┤     ├───────────────────────┤
│ Id (PK)              │     │ Id (PK)               │
│ Nombre               │     │ Nombre                │
│ Apellido             │     │ Descripcion           │
│ Email (UQ)           │     │ Estado                │
│ Telefono             │     │ FechaCreacion         │
│ PasswordHash         │     └──────────┬────────────┘
│ RefreshToken         │                │
│ RefreshTokenExpiry   │     ┌──────────┴────────────┐
│ RolId (FK)           │────▶│    UsuarioRol          │
│ Estado               │     │ UsuarioId (FK)         │
│ FechaCreacion        │     │ RolId (FK)             │
│ FechaActualizacion   │     └───────────────────────┘
└──────────────────────┘
                              ┌───────────────────────┐
                              │      Permiso           │
                              ├───────────────────────┤
                              │ Id (PK)               │
                              │ Modulo                │
                              │ Accion                │
                              │ Descripcion           │
                              └──────────┬────────────┘
                              ┌──────────┴────────────┐
                              │    RolPermiso          │
                              │ RolId (FK)             │
                              │ PermisoId (FK)         │
                              └───────────────────────┘
```

### Schema: `maestros`

```
┌──────────────────────┐     ┌───────────────────────┐
│     Proveedor        │     │    CategoriaProducto   │
├──────────────────────┤     ├───────────────────────┤
│ Id (PK)              │     │ Id (PK)               │
│ RazonSocial          │     │ Nombre (Frutas,       │
│ RUC                  │     │   Tubérculos, etc.)   │
│ Telefono             │     │ Estado                │
│ Direccion            │     └──────────┬────────────┘
│ Provincia            │                │
│ Departamento         │     ┌──────────┴────────────┐
│ ContactoNombre       │     │      Producto          │
│ ContactoTelefono     │     ├───────────────────────┤
│ Estado               │     │ Id (PK)               │
│ UsuarioId (FK)       │     │ Nombre                │
│ FechaCreacion        │     │ CategoriaId (FK)      │
└──────────────────────┘     │ Descripcion           │
                              │ Estado                │
┌──────────────────────┐     └───────────────────────┘
│     Pabellon         │
├──────────────────────┤     ┌───────────────────────┐
│ Id (PK)              │     │      Marca             │
│ Nombre               │     ├───────────────────────┤
│ Rubro                │     │ Id (PK)               │
│ Ubicacion            │     │ Nombre                │
│ Estado               │     │ ProveedorId (FK)      │
└──────────────────────┘     │ ProductoId (FK)       │
                              │ Estado                │
┌──────────────────────┐     └───────────────────────┘
│      Puesto          │
├──────────────────────┤     ┌───────────────────────┐
│ Id (PK)              │     │   UnidadLogistica      │
│ Numero               │     ├───────────────────────┤
│ PabellonId (FK)      │     │ Id (PK)               │
│ UsuarioId (FK)       │     │ Nombre (saco, caja,   │
│ Estado               │     │   malla, cajón)       │
│ FechaCreacion        │     │ Abreviatura           │
└──────────────────────┘     │ Estado                │
                              └───────────────────────┘
┌──────────────────────┐
│      Tamanio         │
├──────────────────────┤
│ Id (PK)              │
│ Nombre (Grande,      │
│   Mediano, Pequeño)  │
│ ProductoId (FK)      │
│ Estado               │
└──────────────────────┘
```

### Schema: `guias`

```
┌───────────────────────────┐
│        Guia                │
├───────────────────────────┤
│ Id (PK)                   │
│ Numero (UQ, auto)         │
│ ProveedorId (FK)          │
│ TransportistaId (FK)      │
│ FechaEnvio                │
│ FechaLlegadaEstimada      │
│ Observaciones             │
│ EstadoGuia (Pendiente,    │
│   EnTransito, Recibida,   │
│   ConFaltantes, Cerrada)  │
│ CostoTransporteTotal      │
│ FechaCreacion             │
│ UsuarioCreacion           │
└────────────┬──────────────┘
             │ 1..N
┌────────────┴──────────────┐
│      GuiaDetalle           │
├───────────────────────────┤
│ Id (PK)                   │
│ GuiaId (FK)               │
│ PuestoDestinoId (FK)      │
│ ProductoId (FK)           │
│ MarcaId (FK)              │
│ TamanioId (FK)            │
│ UnidadLogisticaId (FK)    │
│ Cantidad                  │
│ PrecioUnitario            │
│ PrecioTransporteUnitario  │
│ SubtotalTransporte        │
│ Observaciones             │
│ Estado                    │
└───────────────────────────┘
```

### Schema: `transporte`

```
┌──────────────────────┐     ┌───────────────────────┐
│    Transportista     │     │      Camion            │
├──────────────────────┤     ├───────────────────────┤
│ Id (PK)              │     │ Id (PK)               │
│ Nombre               │     │ Placa (UQ)            │
│ Apellido             │     │ TransportistaId (FK)  │
│ DNI (UQ)             │     │ Capacidad             │
│ Telefono             │     │ Marca                 │
│ Licencia             │     │ Modelo                │
│ UsuarioId (FK)       │     │ Estado                │
│ Estado               │     └───────────────────────┘
│ FechaCreacion        │
└──────────────────────┘     ┌───────────────────────┐
                              │   TarifaTransporte     │
                              ├───────────────────────┤
                              │ Id (PK)               │
                              │ TransportistaId (FK)  │
                              │ UnidadLogisticaId(FK) │
                              │ RutaOrigen            │
                              │ RutaDestino           │
                              │ PrecioUnitario        │
                              │ FechaVigencia         │
                              │ Estado                │
                              └───────────────────────┘

┌───────────────────────────┐
│    LiquidacionTransporte   │
├───────────────────────────┤
│ Id (PK)                   │
│ TransportistaId (FK)      │
│ CamionId (FK)             │
│ FechaViaje                │
│ TotalUnidades             │
│ MontoTotal                │
│ Estado (Pendiente, Pagada)│
│ FechaCreacion             │
└────────────┬──────────────┘
             │ 1..N
┌────────────┴──────────────┐
│ LiquidacionDetalle         │
├───────────────────────────┤
│ Id (PK)                   │
│ LiquidacionId (FK)        │
│ GuiaId (FK)               │
│ UnidadLogisticaId (FK)    │
│ Cantidad                  │
│ PrecioUnitario            │
│ Subtotal                  │
└───────────────────────────┘
```

### Schema: `recepcion`

```
┌───────────────────────────┐
│     Recepcion              │
├───────────────────────────┤
│ Id (PK)                   │
│ GuiaId (FK)               │
│ EncargadoId (FK→Usuario)  │
│ FechaRecepcion            │
│ HoraLlegada               │
│ Observaciones             │
│ Estado (EnProceso,        │
│   Completada, ConNovedad) │
│ FechaCreacion             │
└────────────┬──────────────┘
             │ 1..N
┌────────────┴──────────────┐
│   RecepcionDetalle         │
├───────────────────────────┤
│ Id (PK)                   │
│ RecepcionId (FK)          │
│ GuiaDetalleId (FK)        │
│ CantidadRecibida          │
│ CantidadFaltante          │
│ CantidadDañada            │
│ Observaciones             │
│ Estado                    │
└───────────────────────────┘

┌───────────────────────────┐
│       Faltante             │
├───────────────────────────┤
│ Id (PK)                   │
│ RecepcionDetalleId (FK)   │
│ TipoIncidencia (Faltante, │
│   Dañado, Equivocado)     │
│ Cantidad                  │
│ Descripcion               │
│ EvidenciaUrl              │
│ EstadoReclamo (Abierto,   │
│   EnRevision, Resuelto)   │
│ FechaRegistro             │
│ FechaResolucion           │
└───────────────────────────┘
```

### Schema: `comerciante`

```
┌───────────────────────────┐
│  ConfirmacionRecepcion     │
├───────────────────────────┤
│ Id (PK)                   │
│ PuestoId (FK)             │
│ RecepcionDetalleId (FK)   │
│ CantidadConfirmada        │
│ Conforme (bit)            │
│ Observaciones             │
│ FechaConfirmacion         │
└───────────────────────────┘

┌───────────────────────────┐
│       Inventario           │
├───────────────────────────┤
│ Id (PK)                   │
│ PuestoId (FK)             │
│ ProductoId (FK)           │
│ MarcaId (FK)              │
│ TamanioId (FK)            │
│ UnidadLogisticaId (FK)    │
│ StockActual               │
│ StockMinimo               │
│ UltimaActualizacion       │
└───────────────────────────┘

┌───────────────────────────┐
│    MovimientoInventario    │
├───────────────────────────┤
│ Id (PK)                   │
│ InventarioId (FK)         │
│ TipoMovimiento (Entrada,  │
│   Salida, Ajuste)         │
│ Cantidad                  │
│ ReferenciaId              │
│ ReferenciaTipo (Recepcion,│
│   Venta, Ajuste)          │
│ Observaciones             │
│ FechaMovimiento           │
│ UsuarioId (FK)            │
└───────────────────────────┘
```

### Schema: `finanzas` *(Fase 4)*

```
┌───────────────────────────┐
│         Venta              │
├───────────────────────────┤
│ Id (PK)                   │
│ PuestoId (FK)             │
│ ClienteNombre             │
│ FechaVenta                │
│ MontoTotal                │
│ TipoPago (Contado, Crédito)│
│ Estado                    │
│ UsuarioId (FK)            │
└────────────┬──────────────┘
             │ 1..N
┌────────────┴──────────────┐
│       VentaDetalle         │
├───────────────────────────┤
│ Id (PK)                   │
│ VentaId (FK)              │
│ ProductoId (FK)           │
│ MarcaId (FK)              │
│ TamanioId (FK)            │
│ UnidadLogisticaId (FK)    │
│ Cantidad                  │
│ PrecioUnitario            │
│ Subtotal                  │
└───────────────────────────┘

┌───────────────────────────┐
│     CuentaPorPagar         │
├───────────────────────────┤
│ Id (PK)                   │
│ PuestoId (FK)             │
│ ProveedorId (FK)          │
│ GuiaId (FK)               │
│ MontoTotal                │
│ MontoPagado               │
│ Saldo                     │
│ FechaVencimiento          │
│ Estado (Pendiente,        │
│   ParcialmentePagada,     │
│   Pagada)                 │
│ FechaCreacion             │
└────────────┬──────────────┘
             │ 1..N
┌────────────┴──────────────┐
│         Pago               │
├───────────────────────────┤
│ Id (PK)                   │
│ CuentaPorPagarId (FK)     │
│ Monto                     │
│ MetodoPago                │
│ NumeroOperacion           │
│ FechaPago                 │
│ Observaciones             │
│ UsuarioId (FK)            │
└───────────────────────────┘
```

---

## 📁 Estructura de Carpetas del Proyecto

```
📦 MercadoMAX/
├── 📁 Backend/
│   ├── 📁 MercadoMAX.Gateway/              ← API Gateway (Ocelot)
│   ├── 📁 MercadoMAX.Auth.API/             ← Microservicio Auth
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Repositories/
│   │   ├── Middleware/
│   │   ├── appsettings.json
│   │   └── Program.cs
│   ├── 📁 MercadoMAX.Maestros.API/         ← Microservicio Maestros
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Repositories/
│   │   └── ...
│   ├── 📁 MercadoMAX.Guias.API/            ← Microservicio Guías
│   ├── 📁 MercadoMAX.Transporte.API/       ← Microservicio Transporte
│   ├── 📁 MercadoMAX.Recepcion.API/        ← Microservicio Recepción
│   ├── 📁 MercadoMAX.Comerciante.API/      ← Microservicio Comerciante
│   ├── 📁 MercadoMAX.Finanzas.API/         ← Microservicio Finanzas (Fase 4)
│   ├── 📁 MercadoMAX.Shared/               ← Librería compartida
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Helpers/
│   │   ├── Constants/
│   │   └── Extensions/
│   ├── MercadoMAX.sln
│   └── docker-compose.yml
├── 📁 Frontend/
│   ├── 📁 src/
│   │   ├── 📁 app/
│   │   │   ├── 📁 core/                    ← Guards, interceptors, auth
│   │   │   │   ├── guards/
│   │   │   │   ├── interceptors/
│   │   │   │   ├── services/
│   │   │   │   └── models/
│   │   │   ├── 📁 shared/                  ← Componentes reutilizables
│   │   │   │   ├── components/
│   │   │   │   ├── pipes/
│   │   │   │   ├── directives/
│   │   │   │   └── utils/
│   │   │   ├── 📁 features/                ← Módulos por funcionalidad
│   │   │   │   ├── 📁 auth/
│   │   │   │   ├── 📁 maestros/
│   │   │   │   │   ├── productos/
│   │   │   │   │   ├── proveedores/
│   │   │   │   │   ├── puestos/
│   │   │   │   │   └── pabellones/
│   │   │   │   ├── 📁 guias/
│   │   │   │   ├── 📁 transporte/
│   │   │   │   ├── 📁 recepcion/
│   │   │   │   ├── 📁 comerciante/
│   │   │   │   └── 📁 finanzas/
│   │   │   ├── 📁 layout/
│   │   │   │   ├── sidebar/
│   │   │   │   ├── navbar/
│   │   │   │   └── footer/
│   │   │   └── app.routes.ts
│   │   ├── 📁 assets/
│   │   ├── 📁 environments/
│   │   └── styles.scss
│   ├── angular.json
│   ├── package.json
│   └── Dockerfile
├── 📁 ScriptBD/
│   ├── 01_CreateDatabase.sql
│   ├── 02_Schema_Auth.sql
│   ├── 03_Schema_Maestros.sql
│   ├── 04_Schema_Guias.sql
│   ├── 05_Schema_Transporte.sql
│   ├── 06_Schema_Recepcion.sql
│   ├── 07_Schema_Comerciante.sql
│   ├── 08_Schema_Finanzas.sql
│   ├── 09_StoredProcedures_Auth.sql
│   ├── 10_StoredProcedures_Maestros.sql
│   ├── 11_StoredProcedures_Guias.sql
│   ├── 12_StoredProcedures_Transporte.sql
│   ├── 13_StoredProcedures_Recepcion.sql
│   ├── 14_StoredProcedures_Comerciante.sql
│   ├── 15_StoredProcedures_Finanzas.sql
│   ├── 50_SeedData_Roles.sql
│   ├── 51_SeedData_Permisos.sql
│   ├── 52_SeedData_UnidadesLogisticas.sql
│   ├── 53_SeedData_Categorias.sql
│   └── 99_FullScript.sql
└── 📁 Docs/
    ├── PLAN_PROYECTO.md
    ├── ERD_Diagrama.md
    └── API_Endpoints.md
```

---

## 🔐 Roles del Sistema

| Rol | Accesos |
|-----|---------|
| **Administrador** | Acceso total, gestión de usuarios y configuración |
| **Proveedor** | Crear guías, ver estado de envíos, historial |
| **Transportista** | Ver guías asignadas, tarifas, liquidaciones |
| **EncargadoRecepcion** | Registrar llegada, validar carga, registrar faltantes |
| **Comerciante** | Confirmar recepción, ver inventario, ventas, deudas |
| **Estibador** | Ver asignaciones de descarga (lectura) |

---

## � Convención de Nombres para Stored Procedures

Todos los SPs siguen este formato estándar:

| Operación | Formato | Ejemplo |
|-----------|---------|---------|
| Listar todos | `SP_LIST_{ENTIDAD}` | `SP_LIST_PRODUCTO` |
| Obtener uno | `SP_READ_{ENTIDAD}` | `SP_READ_PRODUCTO` |
| Crear | `SP_CREATE_{ENTIDAD}` | `SP_CREATE_PRODUCTO` |
| Actualizar | `SP_UPDATE_{ENTIDAD}` | `SP_UPDATE_PRODUCTO` |
| Eliminar (lógico) | `SP_DELETE_{ENTIDAD}` | `SP_DELETE_PRODUCTO` |
| Filtro especial | `SP_LIST_{ENTIDAD}_BY_{FILTRO}` | `SP_LIST_PUESTO_BY_PABELLON` |
| Acción especial | `SP_{ACCION}_{ENTIDAD}` | `SP_LOGIN_USUARIO`, `SP_ANULAR_GUIA` |
| Reporte | `SP_REPORTE_{NOMBRE}` | `SP_REPORTE_ESTADO_CUENTA` |

---

## �📋 PLAN DE TAREAS POR FASES

---

### 🔷 FASE 0 — Infraestructura y Setup (Fundación)

| # | Tarea | Tipo | Prioridad |
|---|-------|------|-----------|
| 0.1 | Crear estructura de carpetas del proyecto | Setup | 🔴 Alta |
| 0.2 | Crear solución .NET `MercadoMAX.sln` | Backend | 🔴 Alta |
| 0.3 | Crear proyecto `MercadoMAX.Shared` (class library) | Backend | 🔴 Alta |
| 0.4 | Configurar `docker-compose.yml` para SQL Server | Infra | 🔴 Alta |
| 0.5 | Crear base de datos `MercadoMAX_DB` | BD | 🔴 Alta |
| 0.6 | Crear schemas en BD (auth, maestros, guias, transporte, recepcion, comerciante) | BD | 🔴 Alta |
| 0.7 | Crear proyecto Angular con standalone components | Frontend | 🔴 Alta |
| 0.8 | Configurar environments (dev/prod) en Angular | Frontend | 🟡 Media |
| 0.9 | Instalar dependencias Angular (Angular Material, NGX, etc.) | Frontend | 🟡 Media |
| 0.10 | Crear `README.md` con instrucciones de setup | Doc | 🟢 Baja |

---

### 🔷 FASE 1 — Autenticación (Microservicio Auth)

#### 📂 Base de Datos

| # | Tarea | Descripción |
|---|-------|-------------|
| 1.1 | Crear tabla `auth.Rol` | Roles del sistema |
| 1.2 | Crear tabla `auth.Permiso` | Permisos por módulo y acción |
| 1.3 | Crear tabla `auth.RolPermiso` | Relación N:N roles-permisos |
| 1.4 | Crear tabla `auth.Usuario` | Usuarios con hash de password |
| 1.5 | Crear tabla `auth.UsuarioRol` | Relación N:N usuarios-roles |
| 1.6 | Crear SP `SP_CREATE_USUARIO` | Registro de usuario |
| 1.7 | Crear SP `SP_LOGIN_USUARIO` | Validar credenciales |
| 1.8 | Crear SP `SP_READ_USUARIO_BY_EMAIL` | Buscar por email |
| 1.9 | Crear SP `SP_UPDATE_USUARIO_REFRESH_TOKEN` | Guardar refresh token |
| 1.10 | Crear SP `SP_LIST_ROL` | Listar roles |
| 1.11 | Crear SP `SP_READ_ROL` | Obtener un rol por ID |
| 1.12 | Crear SP `SP_LIST_PERMISO_BY_ROL` | Permisos de un rol |
| 1.13 | Seed data: Roles iniciales | Admin, Proveedor, Transportista, etc. |
| 1.14 | Seed data: Permisos iniciales | CRUD por módulo |
| 1.15 | Seed data: Usuario admin por defecto | admin@mercadomax.com |

#### 🔧 Backend — `MercadoMAX.Auth.API`

| # | Tarea | Descripción |
|---|-------|-------------|
| 1.16 | Crear proyecto Web API `Auth.API` | .NET 8 minimal API |
| 1.17 | Configurar `appsettings.json` (conexión BD, JWT settings) | Config |
| 1.18 | Crear modelo `Usuario` | Entidad |
| 1.19 | Crear DTOs: `LoginRequest`, `LoginResponse`, `RegisterRequest` | DTOs |
| 1.20 | Crear `IAuthRepository` + `AuthRepository` | Repositorio con Dapper |
| 1.21 | Crear `IAuthService` + `AuthService` | Lógica de negocio |
| 1.22 | Implementar generación de JWT + Refresh Token | Auth |
| 1.23 | Implementar hash de passwords (BCrypt) | Security |
| 1.24 | Crear `AuthController` — POST `/api/auth/login` | Endpoint |
| 1.25 | Crear `AuthController` — POST `/api/auth/register` | Endpoint |
| 1.26 | Crear `AuthController` — POST `/api/auth/refresh-token` | Endpoint |
| 1.27 | Crear Middleware de autenticación JWT | Middleware |
| 1.28 | Crear Middleware de autorización por permisos | Middleware |
| 1.29 | Configurar Swagger para Auth.API | Doc |
| 1.30 | Probar endpoints con Swagger/Postman | Test |

#### 🎨 Frontend — Módulo Auth

| # | Tarea | Descripción |
|---|-------|-------------|
| 1.31 | Crear servicio `AuthService` | HTTP calls, token storage |
| 1.32 | Crear interceptor HTTP para JWT | Attach token a headers |
| 1.33 | Crear guard `AuthGuard` | Proteger rutas |
| 1.34 | Crear guard `RoleGuard` | Verificar rol para ruta |
| 1.35 | Crear página de Login | Componente standalone |
| 1.36 | Crear página de Registro (solo admin) | Componente |
| 1.37 | Crear componente Layout (sidebar + navbar) | Layout |
| 1.38 | Configurar rutas con lazy loading | Routing |
| 1.39 | Implementar logout y expiración de token | UX |

---

### 🔷 FASE 2 — Maestros (Datos Base)

#### 📂 Base de Datos

| # | Tarea | Descripción |
|---|-------|-------------|
| 2.1 | Crear tabla `maestros.CategoriaProducto` | Frutas, Tubérculos, Verduras |
| 2.2 | Crear tabla `maestros.Producto` | Limón, Papa, Maracuyá, etc. |
| 2.3 | Crear tabla `maestros.UnidadLogistica` | Saco, caja, malla, cajón |
| 2.4 | Crear tabla `maestros.Tamanio` | Grande, mediano, pequeño por producto |
| 2.5 | Crear tabla `maestros.Pabellon` | Pabellones del mercado |
| 2.6 | Crear tabla `maestros.Puesto` | Puestos dentro de pabellones |
| 2.7 | Crear tabla `maestros.Proveedor` | Datos del proveedor |
| 2.8 | Crear tabla `maestros.Marca` | Marca/lote por proveedor-producto |
| 2.9 | Crear SP `SP_LIST_CATEGORIA_PRODUCTO` | Listar todas las categorías |
| 2.10 | Crear SP `SP_READ_CATEGORIA_PRODUCTO` | Obtener una categoría por ID |
| 2.11 | Crear SP `SP_CREATE_CATEGORIA_PRODUCTO` | Crear categoría |
| 2.12 | Crear SP `SP_UPDATE_CATEGORIA_PRODUCTO` | Actualizar categoría |
| 2.13 | Crear SP `SP_DELETE_CATEGORIA_PRODUCTO` | Eliminar categoría (lógico) |
| 2.14 | Crear SP `SP_LIST_PRODUCTO` | Listar todos los productos |
| 2.15 | Crear SP `SP_READ_PRODUCTO` | Obtener un producto por ID |
| 2.16 | Crear SP `SP_CREATE_PRODUCTO` | Crear producto |
| 2.17 | Crear SP `SP_UPDATE_PRODUCTO` | Actualizar producto |
| 2.18 | Crear SP `SP_DELETE_PRODUCTO` | Eliminar producto (lógico) |
| 2.19 | Crear SP `SP_LIST_UNIDAD_LOGISTICA` | Listar unidades logísticas |
| 2.20 | Crear SP `SP_READ_UNIDAD_LOGISTICA` | Obtener una unidad por ID |
| 2.21 | Crear SP `SP_CREATE_UNIDAD_LOGISTICA` | Crear unidad logística |
| 2.22 | Crear SP `SP_UPDATE_UNIDAD_LOGISTICA` | Actualizar unidad logística |
| 2.23 | Crear SP `SP_DELETE_UNIDAD_LOGISTICA` | Eliminar unidad logística (lógico) |
| 2.24 | Crear SP `SP_LIST_TAMANIO` | Listar tamaños |
| 2.25 | Crear SP `SP_READ_TAMANIO` | Obtener un tamaño por ID |
| 2.26 | Crear SP `SP_CREATE_TAMANIO` | Crear tamaño |
| 2.27 | Crear SP `SP_UPDATE_TAMANIO` | Actualizar tamaño |
| 2.28 | Crear SP `SP_DELETE_TAMANIO` | Eliminar tamaño (lógico) |
| 2.29 | Crear SP `SP_LIST_PABELLON` | Listar pabellones |
| 2.30 | Crear SP `SP_READ_PABELLON` | Obtener un pabellón por ID |
| 2.31 | Crear SP `SP_CREATE_PABELLON` | Crear pabellón |
| 2.32 | Crear SP `SP_UPDATE_PABELLON` | Actualizar pabellón |
| 2.33 | Crear SP `SP_DELETE_PABELLON` | Eliminar pabellón (lógico) |
| 2.34 | Crear SP `SP_LIST_PUESTO` | Listar puestos |
| 2.35 | Crear SP `SP_READ_PUESTO` | Obtener un puesto por ID |
| 2.36 | Crear SP `SP_CREATE_PUESTO` | Crear puesto |
| 2.37 | Crear SP `SP_UPDATE_PUESTO` | Actualizar puesto |
| 2.38 | Crear SP `SP_DELETE_PUESTO` | Eliminar puesto (lógico) |
| 2.39 | Crear SP `SP_LIST_PUESTO_BY_PABELLON` | Listar puestos por pabellón |
| 2.40 | Crear SP `SP_LIST_PROVEEDOR` | Listar proveedores |
| 2.41 | Crear SP `SP_READ_PROVEEDOR` | Obtener un proveedor por ID |
| 2.42 | Crear SP `SP_CREATE_PROVEEDOR` | Crear proveedor |
| 2.43 | Crear SP `SP_UPDATE_PROVEEDOR` | Actualizar proveedor |
| 2.44 | Crear SP `SP_DELETE_PROVEEDOR` | Eliminar proveedor (lógico) |
| 2.45 | Crear SP `SP_LIST_MARCA` | Listar marcas |
| 2.46 | Crear SP `SP_READ_MARCA` | Obtener una marca por ID |
| 2.47 | Crear SP `SP_CREATE_MARCA` | Crear marca |
| 2.48 | Crear SP `SP_UPDATE_MARCA` | Actualizar marca |
| 2.49 | Crear SP `SP_DELETE_MARCA` | Eliminar marca (lógico) |
| 2.50 | Crear SP `SP_LIST_MARCA_BY_PROVEEDOR` | Listar marcas por proveedor |
| 2.51 | Seed data: Categorías iniciales | Frutas, Tubérculos, Verduras |
| 2.52 | Seed data: Unidades logísticas | Saco, Caja, Malla, Cajón |

#### 🔧 Backend — `MercadoMAX.Maestros.API`

| # | Tarea | Descripción |
|---|-------|-------------|
| 2.53 | Crear proyecto Web API `Maestros.API` | .NET 8 |
| 2.54 | Crear modelos: Categoria, Producto, UnidadLogistica, Tamanio | Entidades |
| 2.55 | Crear modelos: Pabellon, Puesto, Proveedor, Marca | Entidades |
| 2.56 | Crear DTOs para cada entidad (Request/Response) | DTOs |
| 2.57 | Crear repositorios para cada entidad | Dapper |
| 2.58 | Crear servicios para cada entidad | Lógica |
| 2.59 | Crear `CategoriaController` — CRUD completo | 5 Endpoints |
| 2.60 | Crear `ProductoController` — CRUD + filtros | 5 Endpoints |
| 2.61 | Crear `UnidadLogisticaController` — CRUD | 5 Endpoints |
| 2.62 | Crear `TamanioController` — CRUD | 5 Endpoints |
| 2.63 | Crear `PabellonController` — CRUD | 5 Endpoints |
| 2.64 | Crear `PuestoController` — CRUD + filtro por pabellón | 6 Endpoints |
| 2.65 | Crear `ProveedorController` — CRUD + búsqueda | 5 Endpoints |
| 2.66 | Crear `MarcaController` — CRUD + filtro por proveedor | 6 Endpoints |
| 2.67 | Configurar Swagger | Doc |
| 2.68 | Validar con JWT middleware (referenciar Shared) | Security |

#### 🎨 Frontend — Módulo Maestros

| # | Tarea | Descripción |
|---|-------|-------------|
| 2.69 | Crear servicio HTTP para cada entidad de maestros | Services |
| 2.70 | Crear componente tabla genérica reutilizable | Shared |
| 2.71 | Crear modal/dialog genérico para CRUD | Shared |
| 2.72 | Crear página listado de Categorías | Feature |
| 2.73 | Crear formulario crear/editar Categoría | Feature |
| 2.74 | Crear página listado de Productos (con filtro por categoría) | Feature |
| 2.75 | Crear formulario crear/editar Producto | Feature |
| 2.76 | Crear página listado de Unidades Logísticas | Feature |
| 2.77 | Crear formulario crear/editar Unidad Logística | Feature |
| 2.78 | Crear página listado de Pabellones | Feature |
| 2.79 | Crear formulario crear/editar Pabellón | Feature |
| 2.80 | Crear página listado de Puestos (filtro por pabellón) | Feature |
| 2.81 | Crear formulario crear/editar Puesto | Feature |
| 2.82 | Crear página listado de Proveedores (búsqueda) | Feature |
| 2.83 | Crear formulario crear/editar Proveedor | Feature |
| 2.84 | Crear página listado de Marcas (filtro por proveedor) | Feature |
| 2.85 | Crear formulario crear/editar Marca | Feature |
| 2.86 | Crear página listado de Tamaños (filtro por producto) | Feature |
| 2.87 | Crear formulario crear/editar Tamaño | Feature |
| 2.88 | Agregar rutas al módulo maestros con lazy loading | Routing |
| 2.89 | Agregar items al sidebar por rol | Layout |

---

### 🔷 FASE 3 — Guías de Envío

#### 📂 Base de Datos

| # | Tarea | Descripción |
|---|-------|-------------|
| 3.1 | Crear tabla `guias.Guia` | Cabecera de guía |
| 3.2 | Crear tabla `guias.GuiaDetalle` | Líneas de detalle multi-destino |
| 3.3 | Crear SP `SP_CREATE_GUIA` | Insertar cabecera + detalles |
| 3.4 | Crear SP `SP_READ_GUIA` | Obtener guía por ID con detalles |
| 3.5 | Crear SP `SP_LIST_GUIA` | Listar guías con filtros y paginación |
| 3.6 | Crear SP `SP_LIST_GUIA_BY_PROVEEDOR` | Filtro por proveedor |
| 3.7 | Crear SP `SP_LIST_GUIA_BY_PUESTO` | Filtro por puesto destino |
| 3.8 | Crear SP `SP_UPDATE_GUIA_ESTADO` | Cambio de estado |
| 3.9 | Crear SP `SP_ANULAR_GUIA` | Anular guía (lógico) |
| 3.10 | Crear SP `SP_LIST_GUIA_DETALLE_BY_GUIA` | Detalles de una guía |

#### 🔧 Backend — `MercadoMAX.Guias.API`

| # | Tarea | Descripción |
|---|-------|-------------|
| 3.11 | Crear proyecto Web API `Guias.API` | .NET 8 |
| 3.12 | Crear modelos: Guia, GuiaDetalle | Entidades |
| 3.13 | Crear DTOs: GuiaCreateRequest (con lista de detalles) | DTOs |
| 3.14 | Crear DTOs: GuiaResponse, GuiaDetalleResponse | DTOs |
| 3.15 | Crear `IGuiaRepository` + `GuiaRepository` | Dapper |
| 3.16 | Crear `IGuiaService` + `GuiaService` | Lógica: cálculo transporte auto |
| 3.17 | Crear `GuiaController` — POST crear guía | Endpoint |
| 3.18 | Crear `GuiaController` — GET listar (paginado, filtros) | Endpoint |
| 3.19 | Crear `GuiaController` — GET obtener por ID | Endpoint |
| 3.20 | Crear `GuiaController` — PUT actualizar estado | Endpoint |
| 3.21 | Crear `GuiaController` — DELETE anular | Endpoint |
| 3.22 | Implementar cálculo automático de costo de transporte | Lógica |
| 3.23 | Configurar Swagger | Doc |

#### 🎨 Frontend — Módulo Guías

| # | Tarea | Descripción |
|---|-------|-------------|
| 3.24 | Crear servicio `GuiaService` | HTTP calls |
| 3.25 | Crear página listado de Guías (tabla con filtros) | Feature |
| 3.26 | Crear formulario crear Guía — cabecera | Feature |
| 3.27 | Crear componente agregar líneas de detalle (multi-row) | Feature |
| 3.28 | Implementar selección multi-destino (puestos) | Feature |
| 3.29 | Mostrar cálculo automático de transporte en tiempo real | Feature |
| 3.30 | Crear vista detalle de Guía (solo lectura) | Feature |
| 3.31 | Crear componente estado de guía con timeline visual | UX |
| 3.32 | Implementar impresión/PDF de guía | Feature |

---

### 🔷 FASE 4 — Transporte

#### 📂 Base de Datos

| # | Tarea | Descripción |
|---|-------|-------------|
| 4.1 | Crear tabla `transporte.Transportista` | Datos del transportista |
| 4.2 | Crear tabla `transporte.Camion` | Vehículos |
| 4.3 | Crear tabla `transporte.TarifaTransporte` | Precios por unidad+ruta |
| 4.4 | Crear tabla `transporte.LiquidacionTransporte` | Cabecera liquidación |
| 4.5 | Crear tabla `transporte.LiquidacionDetalle` | Detalle liquidación |
| 4.6 | Crear SP `SP_LIST_TRANSPORTISTA` | Listar transportistas |
| 4.7 | Crear SP `SP_READ_TRANSPORTISTA` | Obtener transportista por ID |
| 4.8 | Crear SP `SP_CREATE_TRANSPORTISTA` | Crear transportista |
| 4.9 | Crear SP `SP_UPDATE_TRANSPORTISTA` | Actualizar transportista |
| 4.10 | Crear SP `SP_DELETE_TRANSPORTISTA` | Eliminar transportista (lógico) |
| 4.11 | Crear SP `SP_LIST_CAMION` | Listar camiones |
| 4.12 | Crear SP `SP_READ_CAMION` | Obtener camión por ID |
| 4.13 | Crear SP `SP_CREATE_CAMION` | Crear camión |
| 4.14 | Crear SP `SP_UPDATE_CAMION` | Actualizar camión |
| 4.15 | Crear SP `SP_DELETE_CAMION` | Eliminar camión (lógico) |
| 4.16 | Crear SP `SP_LIST_TARIFA_TRANSPORTE` | Listar tarifas |
| 4.17 | Crear SP `SP_READ_TARIFA_TRANSPORTE` | Obtener tarifa por ID |
| 4.18 | Crear SP `SP_CREATE_TARIFA_TRANSPORTE` | Crear tarifa |
| 4.19 | Crear SP `SP_UPDATE_TARIFA_TRANSPORTE` | Actualizar tarifa |
| 4.20 | Crear SP `SP_DELETE_TARIFA_TRANSPORTE` | Eliminar tarifa (lógico) |
| 4.21 | Crear SP `SP_CREATE_LIQUIDACION` | Generar liquidación desde guías |
| 4.22 | Crear SP `SP_LIST_LIQUIDACION` | Listar liquidaciones con filtros |
| 4.23 | Crear SP `SP_READ_LIQUIDACION` | Obtener liquidación con detalle |
| 4.24 | Crear SP `SP_UPDATE_LIQUIDACION_ESTADO` | Marcar como pagada |

#### 🔧 Backend — `MercadoMAX.Transporte.API`

| # | Tarea | Descripción |
|---|-------|-------------|
| 4.25 | Crear proyecto Web API `Transporte.API` | .NET 8 |
| 4.26 | Crear modelos y DTOs | Entidades |
| 4.27 | Crear repositorios | Dapper |
| 4.28 | Crear servicios con lógica de liquidación | Lógica |
| 4.29 | Crear `TransportistaController` — CRUD | Endpoints |
| 4.30 | Crear `CamionController` — CRUD | Endpoints |
| 4.31 | Crear `TarifaController` — CRUD | Endpoints |
| 4.32 | Crear `LiquidacionController` — Generar, Listar, Ver, Pagar | Endpoints |
| 4.33 | Configurar Swagger | Doc |

#### 🎨 Frontend — Módulo Transporte

| # | Tarea | Descripción |
|---|-------|-------------|
| 4.34 | Crear servicio `TransporteService` | HTTP calls |
| 4.35 | Crear página CRUD Transportistas | Feature |
| 4.36 | Crear página CRUD Camiones | Feature |
| 4.37 | Crear página gestión Tarifas | Feature |
| 4.38 | Crear página Liquidaciones (generar + listar) | Feature |
| 4.39 | Crear vista detalle Liquidación | Feature |
| 4.40 | Crear componente resumen de costos | UX |

---

### 🔷 FASE 5 — Recepción en Mercado

#### 📂 Base de Datos

| # | Tarea | Descripción |
|---|-------|-------------|
| 5.1 | Crear tabla `recepcion.Recepcion` | Cabecera de recepción |
| 5.2 | Crear tabla `recepcion.RecepcionDetalle` | Detalle por línea de guía |
| 5.3 | Crear tabla `recepcion.Faltante` | Registro de incidencias |
| 5.4 | Crear SP `SP_CREATE_RECEPCION` | Crear recepción desde guía recibida |
| 5.5 | Crear SP `SP_READ_RECEPCION` | Obtener recepción por ID |
| 5.6 | Crear SP `SP_LIST_RECEPCION` | Listar recepciones con filtros |
| 5.7 | Crear SP `SP_CREATE_RECEPCION_DETALLE` | Registrar cantidades reales |
| 5.8 | Crear SP `SP_COMPLETAR_RECEPCION` | Cerrar recepción |
| 5.9 | Crear SP `SP_CREATE_FALTANTE` | Registrar incidencia/faltante |
| 5.10 | Crear SP `SP_LIST_FALTANTE` | Listar faltantes con filtros |
| 5.11 | Crear SP `SP_READ_FALTANTE` | Obtener faltante por ID |
| 5.12 | Crear SP `SP_UPDATE_FALTANTE_ESTADO` | Resolver reclamo |

#### 🔧 Backend — `MercadoMAX.Recepcion.API`

| # | Tarea | Descripción |
|---|-------|-------------|
| 5.13 | Crear proyecto Web API `Recepcion.API` | .NET 8 |
| 5.14 | Crear modelos y DTOs | Entidades |
| 5.15 | Crear repositorios | Dapper |
| 5.16 | Crear servicios | Lógica |
| 5.17 | Crear `RecepcionController` — Crear, Ver, Completar | Endpoints |
| 5.18 | Crear `FaltanteController` — Registrar, Listar, Resolver | Endpoints |
| 5.19 | Configurar Swagger | Doc |

#### 🎨 Frontend — Módulo Recepción

| # | Tarea | Descripción |
|---|-------|-------------|
| 5.20 | Crear servicio `RecepcionService` | HTTP calls |
| 5.21 | Crear página recepción de camión (seleccionar guía) | Feature |
| 5.22 | Crear formulario registro de cantidades reales vs guía | Feature |
| 5.23 | Crear componente comparativo (esperado vs recibido) | UX |
| 5.24 | Crear página registro de faltantes con fotos | Feature |
| 5.25 | Crear página listado de faltantes con estados | Feature |
| 5.26 | Crear notificaciones al dueño de puesto | Feature |

---

### 🔷 FASE 6 — Comerciante (Confirmación + Inventario)

#### 📂 Base de Datos

| # | Tarea | Descripción |
|---|-------|-------------|
| 6.1 | Crear tabla `comerciante.ConfirmacionRecepcion` | Confirmación del comerciante |
| 6.2 | Crear tabla `comerciante.Inventario` | Stock por producto-puesto |
| 6.3 | Crear tabla `comerciante.MovimientoInventario` | Kardex de movimientos |
| 6.4 | Crear SP `SP_CREATE_CONFIRMACION_RECEPCION` | Confirmar recepción |
| 6.5 | Crear SP `SP_LIST_CONFIRMACION_RECEPCION` | Listar confirmaciones |
| 6.6 | Crear SP `SP_READ_CONFIRMACION_RECEPCION` | Obtener confirmación por ID |
| 6.7 | Crear SP `SP_UPDATE_INVENTARIO` | Actualizar stock |
| 6.8 | Crear SP `SP_LIST_INVENTARIO_BY_PUESTO` | Stock actual por puesto |
| 6.9 | Crear SP `SP_READ_INVENTARIO` | Obtener inventario por ID |
| 6.10 | Crear SP `SP_LIST_MOVIMIENTO_INVENTARIO` | Historial de movimientos |
| 6.11 | Crear SP `SP_AUTO_UPDATE_INVENTARIO` | Actualizar inventario automático al confirmar |

#### 🔧 Backend — `MercadoMAX.Comerciante.API`

| # | Tarea | Descripción |
|---|-------|-------------|
| 6.12 | Crear proyecto Web API `Comerciante.API` | .NET 8 |
| 6.13 | Crear modelos y DTOs | Entidades |
| 6.14 | Crear repositorios | Dapper |
| 6.15 | Crear servicios | Lógica |
| 6.16 | Crear `ConfirmacionController` — Confirmar, Listar | Endpoints |
| 6.17 | Crear `InventarioController` — Ver stock, Historial | Endpoints |
| 6.18 | Configurar Swagger | Doc |

#### 🎨 Frontend — Módulo Comerciante

| # | Tarea | Descripción |
|---|-------|-------------|
| 6.19 | Crear servicio `ComercianteService` | HTTP calls |
| 6.20 | Crear página confirmación de recepción | Feature |
| 6.21 | Crear dashboard de inventario del puesto | Feature |
| 6.22 | Crear vista de movimientos (Kardex) | Feature |
| 6.23 | Crear alertas de stock bajo | UX |

---

### 🔷 FASE 7 — API Gateway

| # | Tarea | Descripción |
|---|-------|-------------|
| 7.1 | Crear proyecto `MercadoMAX.Gateway` | .NET 8 con Ocelot |
| 7.2 | Configurar rutas a todos los microservicios | Config |
| 7.3 | Configurar rate limiting | Security |
| 7.4 | Configurar CORS | Security |
| 7.5 | Configurar health checks | Monitoring |
| 7.6 | Actualizar `docker-compose.yml` con todos los servicios | Infra |
| 7.7 | Probar flujo completo end-to-end | Test |

---

### 🔷 FASE 8 — Finanzas *(Avanzado)*

#### 📂 Base de Datos

| # | Tarea | Descripción |
|---|-------|-------------|
| 8.1 | Crear tabla `finanzas.Venta` | Ventas del puesto |
| 8.2 | Crear tabla `finanzas.VentaDetalle` | Líneas de venta |
| 8.3 | Crear tabla `finanzas.CuentaPorPagar` | Deudas con proveedores |
| 8.4 | Crear tabla `finanzas.Pago` | Registro de pagos |
| 8.5 | Crear SP `SP_LIST_VENTA` | Listar ventas |
| 8.6 | Crear SP `SP_READ_VENTA` | Obtener venta por ID |
| 8.7 | Crear SP `SP_CREATE_VENTA` | Crear venta con detalle |
| 8.8 | Crear SP `SP_UPDATE_VENTA` | Actualizar venta |
| 8.9 | Crear SP `SP_DELETE_VENTA` | Eliminar venta (lógico) |
| 8.10 | Crear SP `SP_CREATE_CUENTA_POR_PAGAR` | Generar cuenta por pagar |
| 8.11 | Crear SP `SP_LIST_CUENTA_POR_PAGAR` | Listar cuentas por pagar |
| 8.12 | Crear SP `SP_READ_CUENTA_POR_PAGAR` | Obtener cuenta por ID |
| 8.13 | Crear SP `SP_CREATE_PAGO` | Registrar pago |
| 8.14 | Crear SP `SP_LIST_PAGO` | Listar pagos |
| 8.15 | Crear SP `SP_READ_PAGO` | Obtener pago por ID |
| 8.16 | Crear SP `SP_REPORTE_ESTADO_CUENTA` | Reporte estado de cuenta por proveedor |

#### 🔧 Backend — `MercadoMAX.Finanzas.API`

| # | Tarea | Descripción |
|---|-------|-------------|
| 8.17 | Crear proyecto Web API `Finanzas.API` | .NET 8 |
| 8.18 | Crear modelos, DTOs, repositorios, servicios | Full stack |
| 8.19 | Crear `VentaController` — CRUD | Endpoints |
| 8.20 | Crear `CuentaPorPagarController` — Generar, Ver | Endpoints |
| 8.21 | Crear `PagoController` — Registrar, Listar | Endpoints |
| 8.22 | Crear `ReporteController` — Estado de cuenta | Endpoints |

#### 🎨 Frontend — Módulo Finanzas

| # | Tarea | Descripción |
|---|-------|-------------|
| 8.23 | Crear página punto de venta | Feature |
| 8.24 | Crear página cuentas por pagar | Feature |
| 8.25 | Crear página registro de pagos | Feature |
| 8.26 | Crear dashboard estado de cuenta | Feature |
| 8.27 | Crear reportes con gráficos | Feature |

---

### 🔷 FASE 9 — Dashboard y Reportes Generales

| # | Tarea | Descripción |
|---|-------|-------------|
| 9.1 | Crear dashboard Admin (KPIs generales) | Feature |
| 9.2 | Crear dashboard Proveedor (envíos, guías, pagos) | Feature |
| 9.3 | Crear dashboard Transportista (viajes, liquidación) | Feature |
| 9.4 | Crear dashboard Comerciante (inventario, ventas, deudas) | Feature |
| 9.5 | Implementar exportación a Excel | Feature |
| 9.6 | Implementar exportación a PDF | Feature |

---

### 🔷 FASE 10 — Docker, Deploy y Optimización

| # | Tarea | Descripción |
|---|-------|-------------|
| 10.1 | Crear Dockerfile para cada microservicio | Infra |
| 10.2 | Crear Dockerfile para Frontend (multi-stage) | Infra |
| 10.3 | Configurar `docker-compose.yml` completo (todos los servicios) | Infra |
| 10.4 | Crear script de inicialización de BD completo | BD |
| 10.5 | Pruebas de integración end-to-end | Test |
| 10.6 | Optimización de queries y performance | Opt |
| 10.7 | Documentación final de API (Swagger completo) | Doc |
| 10.8 | Manual de usuario básico | Doc |

---

## 📊 Resumen de Tareas

| Fase | Área | Cantidad de Tareas |
|------|------|-------------------|
| 0 — Infraestructura | Setup | 10 |
| 1 — Auth | BD + BE + FE | 39 |
| 2 — Maestros | BD + BE + FE | 89 |
| 3 — Guías | BD + BE + FE | 32 |
| 4 — Transporte | BD + BE + FE | 40 |
| 5 — Recepción | BD + BE + FE | 26 |
| 6 — Comerciante | BD + BE + FE | 23 |
| 7 — Gateway | Infra | 7 |
| 8 — Finanzas | BD + BE + FE | 27 |
| 9 — Dashboards | FE | 6 |
| 10 — Deploy | Infra + Doc | 8 |
| **TOTAL** | | **~307 tareas** |

---

## 🚀 Orden de Ejecución Recomendado

```
FASE 0  →  FASE 1  →  FASE 2  →  FASE 3  →  FASE 4
  ↓                                              ↓
Setup                                        FASE 5
                                                ↓
                                            FASE 6
                                                ↓
                                            FASE 7
                                                ↓
                                            FASE 8
                                                ↓
                                         FASE 9 + 10
```

---

## 🎯 Próximo Paso

> **Empezar con FASE 0 (Setup) + FASE 1 BD (Scripts de tablas auth).**
>
> Dime: **"Empezar Fase 0"** y creo toda la infraestructura base del proyecto.

---

*Documento generado para el proyecto MercadoMAX — Sistema de Gestión para Mercado Mayorista*
*Última actualización: Abril 2026*
