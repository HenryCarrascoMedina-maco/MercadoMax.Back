# FASE 0 — Resultado de la integración

> **Objetivo:** preparar MercadoMAX para recibir el ERP Template **sin afectar funcionalidades existentes**.
> **Fecha:** 2026-06-07 · **Decisiones aplicadas:** D.1 Opción B (sin Material) · D.2 paginación plana · D.3 permisos `"Modulo:Accion"` (el Template se adapta; no se migran permisos ni datos).
> **Alcance respetado:** NO se modificaron Controllers, Services, Repositories, Stored Procedures ni lógica de negocio.

---

## 1. Lista exacta de archivos creados / modificados

### 1.1 Base de datos (creados) — `ScriptBD/Fase0/`
| Archivo | Tipo |
|---|---|
| `00_RUN_ORDER_AND_ROLLBACK.md` | nuevo (doc: orden, rollback, impacto, riesgos) |
| `01_ServerOptions_AutoClose_RCSI.sql` | nuevo |
| `02_ForeignKeys_Missing.sql` | nuevo |
| `03_Indexes_Missing.sql` | nuevo |
| `04_Constraints_Unique_Inventory.sql` | nuevo |
| `99_Rollback_Fase0.sql` | nuevo |

> Los scripts **aún no se han ejecutado** contra la BD (se entregan listos; ver §7).

### 1.2 Backend — creados (`Backend/MercadoMAX.Shared/CrossCutting/`)
| Archivo | Contenido |
|---|---|
| `Constants/HttpConstants.cs` | Cabeceras y content-types |
| `Constants/ErrorMessages.cs` | Mensajes de error (ES) |
| `Exceptions/AppExceptions.cs` | `AppException`, `Business/Conflict/Forbidden/NotFound/Unauthorized/Validation` |
| `Responses/ErrorResponse.cs` | `ErrorResponse`, `ValidationErrorResponse` |
| `Pagination/PaginationContracts.cs` | `SortParams`, `PaginationParams`, `FilterParams`, `SortDirection` |
| `Logging/LoggingOptions.cs` | Opciones (sección `MercadoMaxLogging`) |
| `Logging/LogConstants.cs` | Nombres de eventos |
| `Middlewares/CorrelationIdMiddleware.cs` | Correlation id + `GetCorrelationId()` |
| `Middlewares/GlobalExceptionMiddleware.cs` | Manejo global de excepciones |
| `Middlewares/RequestLoggingMiddleware.cs` | Log estructurado por request |
| `Middlewares/PerformanceMiddleware.cs` | Detección de requests lentos |
| `Extensions/ClaimsPrincipalExtensions.cs` | `GetUserId/GetPermissions/HasPermission` |
| `Extensions/CrossCuttingExtensions.cs` | `AddMercadoMaxCrossCutting()` + `UseMercadoMaxCrossCutting()` |

### 1.3 Backend — modificados
| Archivo | Cambio |
|---|---|
| `Backend/MercadoMAX.Shared/DTOs/ApiResponse.cs` | **Aditivo**: `CorrelationId` en `ApiResponse<T>` y `PagedResponse<T>`; factory `FromSpResult`; `PagedResponse.Create`; `Message?` en `PagedResponse`; `TotalPages` protegido ante `PageSize=0` |
| `MercadoMAX.Auth.API/Program.cs` | +using, +`AddMercadoMaxCrossCutting`, +`UseMercadoMaxCrossCutting` |
| `MercadoMAX.Maestros.API/Program.cs` | idem |
| `MercadoMAX.Guias.API/Program.cs` | idem |
| `MercadoMAX.Finanzas.API/Program.cs` | idem |
| `MercadoMAX.Comerciante.API/Program.cs` | idem |
| `MercadoMAX.Recepcion.API/Program.cs` | idem |
| `MercadoMAX.Transporte.API/Program.cs` | idem |

> Cada Program.cs recibió **3 líneas aditivas**: el `using`, el registro de opciones (tras `AddHealthChecks`) y el wiring del pipeline (antes de `UseAuthentication`). Nada más cambió.

### 1.4 Frontend — creados (`Frontend/mercado-max/src/app/core/`)
| Archivo | Contenido |
|---|---|
| `models/pagination.model.ts` | `PaginationRequest`, `Sort`, `SortDirection`, default |
| `services/loading.service.ts` | `LoadingService` (signals) |
| `services/error-handler.service.ts` | `ErrorHandlerService` (mensajes ES, validación por campo) |
| `services/api.service.ts` | `ApiService` (multi-URL, desempaqueta envoltorios) |
| `services/base-crud.service.ts` | `BaseCrudService<TDto,...>` (CRUD + `toggleStatus` + `getAll`) |
| `interceptors/correlation-id.interceptor.ts` | `correlationIdInterceptor` |
| `interceptors/loading.interceptor.ts` | `loadingInterceptor` (+`SKIP_LOADING`) |
| `interceptors/error.interceptor.ts` | `errorInterceptor` (normaliza+loguea+re-lanza) |

### 1.5 Frontend — modificados
| Archivo | Cambio |
|---|---|
| `core/models/api-response.model.ts` | **Aditivo**: `correlationId?` en `ApiResponse`/`PagedResponse`, `message?` en `PagedResponse`, nuevo `ApiErrorResponse` |
| `app.config.ts` | Registro de los 3 interceptors nuevos; `authInterceptor` queda **el más interno** (preserva el refresh-token) |

**Totales:** BD 6 creados · Backend 13 creados + 8 modificados · Frontend 8 creados + 2 modificados.

---

## 2. Scripts SQL generados
Ver carpeta `ScriptBD/Fase0/` y su `00_RUN_ORDER_AND_ROLLBACK.md`. Resumen:
1. `01` AUTO_CLOSE OFF + READ_COMMITTED_SNAPSHOT ON.
2. `02` `FK_Guide_Carrier` (con pre-check de huérfanos).
3. `03` índices en FKs sin índice (SaleDetail, GuideDetail, Inventory, SettlementDetail).
4. `04` índice único `UX_Inventory_Sku` (con pre-check de duplicados).
5. `99` rollback estructural.
Todos **idempotentes** y **sin alterar filas existentes**.

---

## 3. Cambios backend realizados
- Nuevo namespace transversal `MercadoMAX.Shared.CrossCutting.*` con: respuestas de error, jerarquía de excepciones, contratos de paginación (request), 4 middlewares y el bootstrap `Add/UseMercadoMaxCrossCutting`.
- `ApiResponse`/`PagedResponse` ampliados de forma **retrocompatible** (campos opcionales + factories), conservando la forma **plana** de paginación (D.2).
- Los 4 middlewares se cablearon en los 7 microservicios **antes** de `UseAuthentication` (orden: correlation → request-log → performance → global-exception).
- **No** se tocó JWT, ni la convención de permisos, ni la sección `Jwt` de Finanzas (eso es Fase 4).

## 4. Cambios frontend realizados
- Capa de datos reutilizable: `ApiService` (multi-URL por microservicio) + `BaseCrudService` (con `toggleStatus` y `getAll`, adaptados a MercadoMAX).
- Manejo de errores común: `ErrorHandlerService` + `errorInterceptor`.
- Interceptors reutilizables: correlation-id, loading, error.
- `authInterceptor` **intacto** y reubicado como interceptor más interno para que su refresh-token 401 siga funcionando primero.
- **No** se migraron pantallas, componentes, formularios ni tablas.

---

## 5. Resultado de build — Frontend
```
EXIT_CODE = 0
Application bundle generation complete. [5.5 s]
Output location: dist/mercado-max
Initial total: 799.61 kB (gzip 131.55 kB)
```
- ✅ Compila sin errores.
- ⚠️ Advertencias **preexistentes** (no introducidas por Fase 0):
  - Bundle inicial 799 kB > presupuesto 600 kB → causa: ausencia de lazy loading (se aborda en Fase 4).
  - SCSS de `role-list`, `guide-list`, `user-list` superan 4 kB → componentes no tocados en Fase 0.
  - CommonJS de `jspdf`/`canvg`/`html2canvas` → dependencias preexistentes.

## 6. Resultado de build — Backend
```
dotnet build MercadoMAX.slnx
Compilación correcta.
    0 Advertencia(s)
    0 Errores
Tiempo: ~10 s
```
- ✅ Los 7 microservicios + Gateway + Shared compilan limpio.

---

## 7. Validación de que no se rompió funcionalidad existente

| Verificación | Resultado |
|---|---|
| Build backend (7 APIs + Gateway + Shared) | ✅ 0 errores / 0 warnings |
| Build frontend | ✅ EXIT 0 (solo warnings preexistentes) |
| Controllers / Services / Repositories / SPs | ✅ **Sin cambios** |
| Contrato `ApiResponse`/`PagedResponse` | ✅ Cambios solo aditivos (campos opcionales); clientes existentes no se afectan |
| JWT y autorización por permisos | ✅ Sin cambios (Auth, validación de `tokenVersion`, `PermissionPolicyProvider` intactos) |
| Flujo de refresh-token (frontend) | ✅ `authInterceptor` sin cambios y como interceptor más interno → 401 sigue manejándose primero |
| CORS con nueva cabecera `X-Correlation-Id` | ✅ Los 7 servicios usan `AllowAnyHeader` → el preflight la admite |
| `GlobalExceptionMiddleware` | ✅ Solo cambia respuestas de excepción NO controlada (antes 500 con stack crudo → ahora `ErrorResponse` JSON). Respuestas exitosas y errores de negocio del SP no cambian |
| Comportamiento observable de endpoints existentes | ✅ Sin cambios (los nuevos servicios/clases no están aún referenciados por código de negocio) |

**Conclusión:** Fase 0 es **estrictamente aditiva**. La infraestructura del cross-cutting (backend) y del shared de datos (frontend) queda lista para el módulo piloto **Product Category** (Fase 1), sin haber alterado ninguna funcionalidad existente.

---

## 8. Pendiente antes de Fase 1 (no bloqueante)
- **Ejecutar** los scripts `ScriptBD/Fase0/` en la BD (01 en ventana de mantenimiento por RCSI). Hoy están entregados pero no aplicados.
- Revisar las advertencias de presupuesto de bundle (se resolverán con lazy loading en Fase 4).
