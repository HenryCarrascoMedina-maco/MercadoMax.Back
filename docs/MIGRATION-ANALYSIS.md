# MIGRATION-ANALYSIS.md
## Auditoría comparativa MercadoMAX ⇄ ERP Template

> **Estado:** análisis técnico. **No incluye código.** Documento previo a la integración.
> **Fecha:** 2026-06-07
> **Proyecto destino:** MercadoMAX (Backend .NET 10 microservicios + Frontend Angular 21)
> **Framework reutilizable (ERP Template):**
> - Backend: `erp-template/backend/ErpBackend/ErpBackend.CrossCutting/`
> - Frontend: `erp-template/frontend/erp-frontend/src/app/shared/`
> - Ubicación real verificada: `d:/HenryC/0.5. Miniproyectos/2. Shared and Crosscutting/erp-template/`

---

## 0. Resumen ejecutivo (leer primero)

| Dimensión | MercadoMAX | ERP Template | Veredicto |
|---|---|---|---|
| Backend .NET | **net10.0** | **net10.0** | ✅ Misma versión, sin fricción de framework |
| Persistencia | Dapper + 153 Stored Procedures | **Agnóstico** (NO usa EF, NO usa Dapper, NO DbContext) | ✅ El template es una capa transversal HTTP/contratos; **convive con Dapper+SP sin cambios** |
| Contrato API | `ApiResponse<T>{Success,Message,Data}` + `PagedResponse<T>` plano | `ApiResponse<T>{Success,Message,Data,CorrelationId}` + `PagedResponse<T>` con `Meta` anidado | ⚠️ Casi idéntico; la **paginación difiere en forma y nombres** → unificar |
| Manejo de errores | ❌ Sin middleware global, sin logging, sin try/catch | ✅ `GlobalExceptionMiddleware` + jerarquía de excepciones + 4 middlewares | 🟢 **El template llena el hueco más grave de MercadoMAX** |
| JWT / permisos | HS256, claim `permission`=`"Modulo:Accion"`, `PermissionPolicyProvider` dinámico | HS256, claim `permission`=`"module.action"`, `PermissionPolicyProvider` dinámico | ⚠️ Mismo mecanismo; **distinta convención de string** → alinear |
| Frontend | **Angular 21**, standalone + signals, **sin librería UI**, Transloco | **Angular 19.2**, standalone + signals, **Angular Material + CDK**, sin i18n (inglés) | 🔴 **Material es la fricción nº1**; la lógica es portable, la capa visual NO sin decisión |
| Boilerplate eliminable | ~60-70% backend / ~70-80% frontend (maestros) | Provee BaseCrudService, tabla/form/modal genéricos, CrudListBase | 🟢 Ahorro alto si se resuelve la fricción de Material |

**Conclusión:** la integración es **viable y de alto retorno**. El backend es prácticamente "plug-in" (misma versión, agnóstico de datos). El frontend aporta el mayor ahorro pero exige **una decisión estratégica sobre Angular Material** (ver §7) y la **alineación del contrato de paginación**. Nada obliga a tocar los 153 SPs ni la lógica de negocio.

---

# PARTE A — BACKEND

## A.1. Qué es realmente el ERP Template (backend)

`ErpBackend.CrossCutting` es un **único proyecto transversal self-contained** (los proyectos `Domain`, `Application`, `Infrastructure` del template están **vacíos**; son esqueletos). Evidencia clave:

- `.csproj`: solo `FrameworkReference Microsoft.AspNetCore.App` + `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.8`. **Cero paquetes de datos** (sin `EntityFrameworkCore`, sin `Dapper`).
- `grep` global: 0 ocurrencias de `DbContext`, `IDbConnection`, `StoredProcedure`, `FluentValidation`.
- No existe `BaseRepository<T>` ni `IRepository<T>`. El único "repo" es un `InMemoryRepository` de demo en `Api/Samples/` (descartable).
- `QueryableExtensions.cs` opera sobre `IQueryable<T>` puro → **opcional**; con Dapper+SP la paginación la resuelve el propio SP construyendo `PagedResult<T>` a mano.

> **Implicación:** no hay nada "acoplado a EF" que desacoplar. El template aporta **plomería HTTP, contratos de respuesta, seguridad, middlewares y helpers** — exactamente lo que a MercadoMAX le falta — sin imponer ORM.

## A.2. Inventario del ERP Template backend (por carpeta)

| Carpeta | Contenido | Agnóstico a Dapper+SP |
|---|---|---|
| `Common/` | `BaseEntity`(Id=**Guid**), `AuditableEntity`, `SoftDeleteEntity`, `BaseDto`, `BaseCreate/UpdateDto`, `BaseCatalogDto`, `Result`, `Result<T>` | ✅ (entidades base asumen PK Guid → opcional) |
| `Responses/` | `ApiResponse<T>`, `PagedResponse<T>`+`PaginationMeta`, `ErrorResponse`, `ValidationErrorResponse`, `CreatedResponse<TKey>`, `NoContentResponse` | ✅ |
| `Pagination/` | `PaginationParams`, `SortParams`, `FilterParams`, `PagedResult<T>`, `PaginationMeta` | ✅ (PagedResult se arma desde el SP) |
| `Exceptions/` | `AppException`(base+StatusCode), `Business`(400), `Conflict`(409), `Forbidden`(403), `NotFound`(404), `Unauthorized`(401), `Validation`(422) | ✅ |
| `Middlewares/` | `GlobalExceptionMiddleware`, `CorrelationIdMiddleware`, `PerformanceMiddleware`, `RequestLoggingMiddleware` | ✅ |
| `Security/` | `JwtSettings`, `CurrentUserService`, `HasPermissionAttribute`, `PermissionHandler`, `PermissionPolicyProvider`, `PermissionRequirement`, `PermissionConstants`, `RoleConstants`, `PasswordPolicy`, `ClaimsConstants` | ✅ |
| `Helpers/` | `JwtHelper`(HS256), `PasswordHelper`(PBKDF2), `ClaimsHelper`, `CsvHelper`, `DateTimeHelper`, `FileHelper`, `PdfHelper`(hook), `StringHelper` | ✅ |
| `Logging/` | `AuditLog`, `IAuditService`/`AuditService`(solo ILogger, NO persiste), `LogEvent`, `LoggingOptions` | ✅ |
| `Extensions/` | `AddErpCrossCutting()`, `AddErpJwtAuthentication()`, `AddErpAuthorization()`, `UseErpCrossCutting()`, `ClaimsPrincipalExtensions`, `QueryableExtensions` | ✅ (bootstrap unificado) |
| `Utilities/` | `ExportService`(CSV ok; Excel/PDF=hook), `ImportService`, `FileStorageService`(disco local) | ✅ |
| `Constants/` | `ErrorMessages`, `SuccessMessages`, `HttpConstants` | ✅ |
| `Api/Controllers/BaseApiController` | `OkResponse/PagedOk/CreatedResponse/NoContentResponse` helpers | ✅ |

**Lo que el template NO trae (hueco a construir en MercadoMAX):** `BaseRepository`/ejecutor de SP con Dapper. El template deliberadamente deja la capa de datos a cada proyecto.

## A.3. Tabla de decisión — BACKEND

> Valores: **KEEP** (mantener lo de MercadoMAX) · **REPLACE** (adoptar el del template y retirar el actual) · **MERGE** (adoptar el del template adaptándolo/extendiéndolo) · **DEFER** (no tocar ahora).

| # | MercadoMAX Actual | ERP Template | Acción | Notas |
|---|---|---|---|---|
| 1 | `Shared/DTOs/ApiResponse<T>` `{Success,Message,Data}` | `Responses/ApiResponse<T>` `{...,CorrelationId}` | **MERGE** | Adoptar el del template (superset). Mantener nombres `Success/Message/Data` para no romper frontend. Añade `CorrelationId`. |
| 2 | `PagedResponse<T>` **plano** `{Data,TotalRecords,PageNumber,PageSize,TotalPages}` | `PagedResponse<T>` con **`Meta`** `{Page,PageSize,TotalItems,TotalPages,HasNext,HasPrevious}` | **MERGE** | ⚠️ Decisión de contrato (impacta frontend). Recomendado: **conservar la forma plana de MercadoMAX** y mapear, o adoptar `Meta` en ambos lados a la vez. Elegir UNO. |
| 3 | `Shared/DTOs/SpResult` `{Success,Message,Id}` | — (no equivalente) | **KEEP** | Es el contrato de retorno de los SP. Imprescindible. |
| 4 | `Shared/Data/DbConnectionFactory` | — (agnóstico) | **KEEP** | El template no impone conexión. Se mantiene. |
| 5 | Repositorios Dapper (plomería repetida en cada método) | — (no hay BaseRepository) | **KEEP + construir** | El template NO lo provee. Construir un `SpExecutor`/`BaseSpRepository` **propio** (hueco identificado). No es del template, pero es la pieza de mayor ahorro backend. |
| 6 | Controllers (mapeo manual `Ok/BadRequest/NotFound`) | `BaseApiController` (`OkResponse/PagedOk/CreatedResponse`) | **REPLACE** | Heredar de `BaseApiController`; elimina el árbol de decisión HTTP repetido. |
| 7 | Services (adaptador `sp.Success==1 ? Ok : Fail`) | Excepciones + `Result<T>` | **MERGE** | Lanzar `NotFoundException/ConflictException/...` y dejar que el middleware traduzca; sustituye el ternario duplicado. |
| 8 | ❌ Sin middleware global de excepciones | `GlobalExceptionMiddleware` + `Exceptions/` | **REPLACE** | 🟢 Llena el hueco más grave. Adoptar tal cual. |
| 9 | ❌ Sin logging estructurado | `RequestLogging/Performance/CorrelationId` (ILogger) | **REPLACE** | Adoptar. Opcional: enchufar Serilog como provider después. |
| 10 | JWT por servicio duplicado 7× (`AddJwtBearer` + `JwtSettings`/`Jwt`) | `AddErpJwtAuthentication()` + `JwtSettings` + `JwtHelper` | **MERGE** | Unificar en un solo extension method. **Corrige de paso la divergencia de Finanzas** (sección `Jwt` sin `ClockSkew`). |
| 11 | `Shared/Authorization/PermissionAuthorization` (`PermissionPolicyProvider`, claim `permission`=`"Modulo:Accion"`) | `Security/*` (`PermissionPolicyProvider`, `HasPermissionAttribute`, claim `"module.action"`) | **MERGE** | Mismo patrón. **Alinear convención de string** (`Modulo:Accion` vs `module.action`). Comparador case-insensitive ayuda. Mantener `[Authorize(Policy=...)]` o migrar a `[HasPermission(...)]`. |
| 12 | `Auth.API/TokenService` (emisión token, BCrypt, refresh, `tokenVersion`) | `JwtHelper`+`ClaimsHelper`+`PasswordHelper`(PBKDF2) | **DEFER** | Auth es lo más frágil. Migrar al final. Ojo: MercadoMAX usa BCrypt; el template usa PBKDF2 → no migrar hashing sin re-hash plan. |
| 13 | Paginación (`QueryMultipleAsync` items+total en repos) | `PagedResult<T>` + `PaginationParams` | **MERGE** | Envolver la salida del SP en `PagedResult<T>`. No usar `QueryableExtensions`. |
| 14 | **153 Stored Procedures** | — | **KEEP** | Activo central intocable. El template no los afecta. |
| 15 | `Gateway` (Ocelot 23.4) | — (no hay gateway en el template) | **KEEP** | Conservar Ocelot y `ocelot.json`. |
| 16 | — (MercadoMAX no tiene entidades base) | `Common/BaseEntity`(Guid), `AuditableEntity`, `SoftDeleteEntity` | **DEFER / SKIP** | Asumen PK Guid; MercadoMAX usa `int IDENTITY` vía SP. No aportan con Dapper+SP. Ignorar salvo refactor mayor de BD. |
| 17 | — (sin helpers) | `Helpers/*`, `Utilities/*` (Export/Import/FileStorage) | **DEFER** | Adoptar cuando se necesiten (export CSV, archivos). No bloqueante. |
| 18 | ❌ Sin auditoría histórica (hueco de BD) | `Logging/IAuditService` (solo ILogger) | **DEFER** | Implementar `IAuditService` con un SP de inserción → cubre el hueco de bitácora detectado en la auditoría de BD. Fase tardía. |

### Backend — clasificación rápida
- **Copiar directamente (REPLACE/adoptar):** `Exceptions/`, `Middlewares/`, `Responses/` (ApiResponse), `Constants/`, `BaseApiController`, extensiones de bootstrap.
- **Fusionar (MERGE):** contrato de paginación, seguridad/permisos (alinear strings), JWT unificado, capa Service (excepciones).
- **Mantener (KEEP):** SPs, `SpResult`, `DbConnectionFactory`, Gateway Ocelot, y **construir** el `SpExecutor` (no lo da el template).
- **No tocar todavía (DEFER):** Auth/TokenService, entidades base Guid, Utilities, AuditService persistente.

## A.4. Riesgos de integración — BACKEND

| Riesgo | Severidad | Mitigación |
|---|---|---|
| **Contrato de paginación divergente** (`Meta` anidado vs plano) rompe el frontend si se cambia a medias | 🔴 Alta | Decidir UNA forma en Fase 0 y aplicarla coordinada backend+frontend. No mezclar. |
| **Convención de permisos** (`Modulo:Accion` vs `module.action`) deja endpoints sin proteger si se mezcla | 🟠 Media | Elegir una convención y migrar seeds/claims/atributos a la vez. Comparador case-insensitive. |
| **Hashing de contraseñas** BCrypt (MMX) vs PBKDF2 (template) | 🟠 Media | NO migrar hashing en la integración. Mantener BCrypt en Auth. DEFER. |
| **Divergencia JWT de Finanzas** al unificar cambia validación | 🟠 Media | Unificar con verificación e2e de login/refresh por servicio. |
| **SpExecutor no existe en el template** → expectativa de "copiar y listo" | 🟡 Baja | Documentado: es construcción propia, no copia. |
| **Tocar Auth** deja a todos fuera | 🔴 Alta | DEFER a la última fase, con pruebas. |
| Entidades base asumen Guid | 🟡 Baja | No adoptarlas; MercadoMAX sigue con int+SP. |

---

# PARTE B — FRONTEND

## B.1. Qué es realmente el ERP Template (frontend)

- **Angular 19.2**, TypeScript 5.7, **100% standalone + signals + functional guards/interceptors**, `OnPush`.
- **Angular Material 19.2 + CDK** (tema `azure-blue`). **Toda la capa visual depende de Material** (MatTable, MatDialog, MatFormField, MatSnackBar, MatPaginator, MatSort).
- **Sin i18n** (textos en inglés hardcodeados).
- Todo lo transversal vive en `src/app/shared/` (no hay `core/`). Incluye un **patrón CRUD de referencia** (`features/catalogs/Categories` + `features/_shared/crud-list.base.ts`).

> MercadoMAX es **Angular 21, sin librería UI, con Transloco**. La **lógica** del template es portable casi sin cambios; la **capa visual** exige decidir sobre Material (§7).

## B.2. Inventario del ERP Template frontend (`shared/`)

| Pieza | Archivo | Depende de Material |
|---|---|---|
| Tabla genérica `GenericTableComponent<T>` (columnas, sort server-side, paginación, acciones, selección) | `components/generic-table/` | 🔴 Sí (MatTable) |
| Form dinámico `GenericFormComponent<T>` (secciones/campos, validators, visibilidad condicional, modos create/edit/view) | `components/generic-form/` | 🔴 Sí (Mat form fields) |
| `DialogService` (`confirm`, `openForm`, `open`) + modal/confirm/form-dialog genéricos | `services/dialog.service.ts` + `components/generic-*` | 🔴 Sí (MatDialog) |
| `ApiService` (desempaqueta envelopes) + `BaseCrudService<TDto,...>` (CRUD tipado) | `services/api.service.ts`, `services/base-crud.service.ts` | 🟢 No (solo HttpClient) |
| `ToastService` | `services/toast.service.ts` | 🔴 Sí (MatSnackBar) |
| Guards: `authGuard`, `permissionGuard`(any/all), `roleGuard`, `unsaved-changes` | `guards/` | 🟢 No |
| Interceptors: `auth`, `error`, `loading`, `correlation-id` | `interceptors/` | 🟢 No |
| Directiva `*appHasPermission` (+ otras: only-numbers, uppercase, trim, autofocus, prevent-double-click) | `directives/` | 🟢 No |
| `CustomValidators` (notBlank, onlyLetters, email, password, dateRange, file, duplicate, match) | `validators/` | 🟢 No |
| Pipes (date, currency, boolean, status, truncate, enum, file-size) | `pipes/` | 🟢 No |
| Modelos: `ApiResponse`, `PagedResponse`+`PaginationMeta`, `PaginationRequest`, `Sort`, `SelectOption` | `models/` | 🟢 No |
| Componentes UI extra: pagination, actions, filter, status-badge, breadcrumb, page-header, card, kpi-card, tabs, skeleton, empty-state, file-upload, export/import-button | `components/` | 🔴 Sí (Material) |
| `ExportService`, `StorageService`, `LoadingService`, `PermissionService`, `AuthService`, `ErrorHandlerService` | `services/` | 🟢 No |
| **Patrón CRUD**: `CrudListBase<TDto>` (signals rows/total/loading; load/onPage/onSort/onFilter/openCreate/openEdit/openView/confirmDelete) | `features/_shared/crud-list.base.ts` | 🟢 No (orquesta servicios) |

## B.3. Tabla de decisión — FRONTEND

| # | MercadoMAX Actual | ERP Template | Acción | Notas |
|---|---|---|---|---|
| 1 | `core/models/api-response.model.ts` `ApiResponse<T>{success,message,data}` | `models/api-response.model.ts` `{...,correlationId}` | **MERGE** | Alinear (añadir `correlationId`). Casi idéntico. |
| 2 | `PagedResponse<T>` **plano** `{data,totalRecords,pageNumber,pageSize,totalPages}` | `PagedResponse<T>` con `meta{page,pageSize,totalItems,...}` | **MERGE** | ⚠️ Mismo dilema que backend #2. Adaptar en `ApiService`/`BaseCrudService` o unificar contrato. |
| 3 | 15+ servicios CRUD copy-paste (`getById/create/update/delete/toggleStatus`) | `ApiService` + `BaseCrudService` | **REPLACE** | Cada servicio pasa a extender `BaseCrudService` (~3 líneas). **Añadir `toggleStatus`** (el template no lo trae → MERGE). |
| 4 | 19 tablas HTML manuales (markup + acciones emoji + badge) | `GenericTableComponent<T>` | **REPLACE** | 🔴 Depende de Material → sujeto a decisión §7. |
| 5 | 27 reactive forms a mano (markup en cada modal) | `GenericFormComponent<T>` (dinámico) | **REPLACE** | 🔴 Material. Define forms por config. |
| 6 | 156 usos de modal manual (`showModal`+overlay) | `DialogService` + modales genéricos | **REPLACE** | 🔴 Material. |
| 7 | `window.confirm()` (16 archivos) | `DialogService.confirm()` | **REPLACE** | UX consistente. |
| 8 | `private msg()`+`setTimeout` (21 archivos) | `ToastService` | **REPLACE** | 🔴 Material (MatSnackBar) o reescribir sin Material. |
| 9 | `shared/components/pagination` (cliente) | `GenericPaginationComponent` (server-side base-1) | **MERGE** | Unificar a paginación server-side. |
| 10 | `authGuard` | `authGuard` | **MERGE** | El del template es equivalente; el de MMX sirve. Quedarse con uno. |
| 11 | `permissionGuard` (permiso simple) | `permissionGuard` (`any`/`all`, modo) | **MERGE** | El del template es más rico. Adoptar adaptando a `route.data`. |
| 12 | `authInterceptor` (JWT + **refresh-token automático + retry 401**) | `auth.interceptor` + `error.interceptor` + `loading.interceptor` | **MERGE** | 🟢 **Conservar la lógica de refresh de MMX** (mejor que la del template) y **añadir** `error` + `loading` interceptors del template. |
| 13 | ❌ Sin error interceptor (manejo duplicado en cada `subscribe`) | `error.interceptor` + `ErrorHandlerService` | **REPLACE** | Adoptar. Centraliza errores + toast. |
| 14 | ❌ Sin directiva de permisos (botones siempre visibles) | `*appHasPermission` | **REPLACE** | Adoptar. Oculta acciones por permiso. |
| 15 | ❌ Sin validators reutilizables | `CustomValidators` | **REPLACE** | Adoptar (añadir validador de documento/RUC si se necesita). |
| 16 | ❌ Sin pipes compartidos | `pipes/*` | **REPLACE** | Adoptar (date/currency/status/...). |
| 17 | Transloco (i18n) | ❌ Sin i18n (inglés) | **KEEP** | Mantener Transloco. **Externalizar** los literales del template al integrar. |
| 18 | **Sin librería UI** (CSS manual + variables tema) | **Angular Material + CDK** | **DECISIÓN §7** | Fricción nº1. No es KEEP/REPLACE simple → ver opciones. |
| 19 | `AuthService` (signals + sessionStorage + hasPermission) | `AuthService`/`PermissionService` | **MERGE** | Conservar el de MMX; opcional extraer `PermissionService`. |
| 20 | `ExportService` (jspdf/xlsx) | `ExportService` + `export-button` | **KEEP/MERGE** | MMX ya exporta; integrar con la tabla genérica. |
| 21 | Componentes transaccionales: `guide-list`(wizard), `sale-list`, `account-payable-list`, `settlement-list`, `dashboard` | — (no equivalente) | **KEEP / DEFER** | Lógica de negocio propia. Se benefician de tabla/modal/BaseService pero migran al final. |
| 22 | Sin lazy loading (34 rutas eager) | (patrón con loadComponent) | **DEFER** | Optimización posterior. |

### Frontend — clasificación rápida
- **Copiar directamente (REPLACE/adoptar — sin Material):** `BaseCrudService`+`ApiService` (adaptando contrato), `error`/`loading` interceptors, `*appHasPermission`, `CustomValidators`, pipes, `permissionGuard`(rico), modelos, `CrudListBase`, `PermissionService`/`ErrorHandlerService`/`LoadingService`.
- **Fusionar (MERGE):** contrato de paginación, `authInterceptor` (refresh MMX + error/loading template), guards, `toggleStatus` en BaseCrudService, `ExportService`.
- **Mantener (KEEP):** Transloco, `AuthService` de MMX, componentes transaccionales, look & feel actual.
- **Decisión estratégica:** **Angular Material** (§7) — condiciona tabla/form/modal/toast genéricos.
- **No tocar todavía (DEFER):** lazy loading, transaccionales complejos.

## B.4. Componentes/duplicados detectados en MercadoMAX (mapa de reemplazo)

| Duplicación actual (MercadoMAX) | Nº focos | Reemplazo en ERP Template |
|---|---|---|
| Componentes list-CRUD casi idénticos | 16 | `CrudListBase` + tabla/form/modal genéricos |
| Tablas HTML manuales | 19 | `GenericTableComponent` |
| Reactive forms a mano | 27 | `GenericFormComponent` |
| Modales manuales (`showModal`/overlay) | 50 archivos / 156 usos | `DialogService` |
| `window.confirm()` | 16 | `DialogService.confirm()` |
| `private msg()` + setTimeout | 21 | `ToastService` |
| Servicios CRUD copy-paste | 15+ | `BaseCrudService` |
| Manejo de error en cada subscribe | difuso | `error.interceptor` |
| Botones CRUD sin control de permiso | global | `*appHasPermission` |

## B.5. Riesgos de integración — FRONTEND

| Riesgo | Severidad | Mitigación |
|---|---|---|
| **Angular Material** vs "sin UI library" → choque visual y de peso de bundle | 🔴 Alta | Decisión §7 explícita antes de tocar componentes visuales. |
| **Contrato de paginación** distinto rompe listados | 🔴 Alta | Coordinar con backend #2; adaptar en `ApiService`. |
| **Salto de versión Angular 19→21** | 🟡 Baja | Código ya usa standalone/signals/functional APIs; cambios menores. |
| **i18n**: textos en inglés del template | 🟠 Media | Externalizar a Transloco al portar cada pieza. |
| `BaseCrudService` **sin `toggleStatus`** | 🟡 Baja | Extenderlo (MERGE). |
| Romper el `authInterceptor` (refresh) al sustituirlo por el del template | 🟠 Media | Conservar la lógica de refresh de MMX; solo añadir error/loading. |

---

# PARTE C — MÓDULO PILOTO

## C.1. Evaluación de candidatos

Criterios: (1) auto-contenido (no depende de otros módulos no migrados para dropdowns), (2) tiene constraint único (ejercita `ConflictException`→409→toast), (3) riesgo de negocio bajo, (4) mapeo directo al patrón de referencia del template, (5) representatividad del boilerplate a eliminar.

| Candidato | Esquema | Campos clave | Dependencias (dropdowns) | Único | Riesgo negocio | Mapeo al template |
|---|---|---|---|---|---|---|
| **Product Category** | master | Name, Description?, Status | ❌ Ninguna | ✅ Name | 🟢 Muy bajo | 🟢 **1:1 con el sample `catalogs/Categories`** del template |
| Pavilion | master | Name, Category(string), Location | ❌ Ninguna | ✅ Name | 🟢 Bajo | 🟢 Alto (catálogo simple) |
| Supplier | master | BusinessName, TaxId, Province, Department, contacto, UserId? | 🟡 UserId opcional | ✅ TaxId | 🟠 Medio (referenciado por Guide/AccountPayable/Brand) | 🟠 Medio (form rico, varias secciones) |
| Brand | master | Name, **SupplierId**, **ProductId** | 🔴 Requiere Supplier + Product | ✅ | 🟠 Medio | 🔴 Bajo (necesita selects de 2 módulos) |
| Carrier | transport | Name, DocumentId, Licencia, UserId? | 🟡 UserId opcional | ✅ DocumentId | 🟢 Bajo | 🟢 Alto (catálogo simple, pero en otro servicio) |

## C.2. Recomendación: **Product Category** 🏆

**Justificación técnica:**

1. **Mapeo 1:1 con la referencia probada del template.** El ERP Template trae como feature de referencia `features/catalogs/Categories` sobre el endpoint `catalog/categories` y `BaseCatalogDto{Code,Name,Description}`. MercadoMAX `master.ProductCategory` (Name, Description, Status) es casi idéntico. **El pilotaje consiste en replicar un patrón ya demostrado**, no en inventarlo → máxima probabilidad de éxito y mínimo tiempo.

2. **Cero dependencias de otros módulos.** No necesita poblar dropdowns desde Supplier/Product/User (a diferencia de **Brand**, que requiere DOS, y de Supplier/Carrier que requieren User). Permite migrar y probar el stack completo **levantando un solo servicio** (Maestros).

3. **Riesgo de negocio mínimo.** Es el catálogo con menor radio de impacto: aunque `Product` lo referencia, no participa en transacciones financieras ni en el flujo de guías. Si algo falla en el pilotaje, no compromete operaciones críticas.

4. **Ejercita el camino end-to-end esencial:** tabla (listado+paginación+acciones) + form dinámico (text + textarea + switch de estado) + modal + `BaseCrudService` + `toggleStatus` + permisos (`Masters:*`) + `ConflictException` (Name único → 409 → toast) + contrato de paginación adaptado. Suficiente para validar **toda la tubería de integración** salvo selects/cascadas.

5. **Define la plantilla replicable.** Una vez migrado, los otros 15 maestros se migran por configuración declarativa siguiendo exactamente el mismo molde.

**Limitación asumida y cómo cubrirla:** Product Category **no tiene dropdowns**, así que no prueba campos `select`/cascada del form dinámico. → **Segundo módulo inmediato: `Supplier`** (form rico con varias secciones + `TaxId` único + `UserId` opcional como primer select). Solo entonces abordar **Brand/Carrier** (`DEFER` hasta que sus dropdowns —Supplier, Product, User— estén disponibles vía `BaseCrudService`).

**Orden sugerido de maestros:** Product Category (piloto) → Supplier → LogisticUnit → Pavilion → ProductSize → Product → Brand → Stall → (Transporte) Carrier → Truck → TransportRate → (Auth) Role → Permission → User.

---

# PARTE D — Decisiones que condicionan el roadmap

## D.1. (Crítica) Estrategia frontend respecto a Angular Material — ✅ DECIDIDA: **Opción B**

El template construye su capa visual sobre Angular Material; MercadoMAX no usa librería UI.

- **✅ Opción B (ELEGIDA) — Portar solo la lógica + contratos, reconstruir la capa visual sin Material.** Se adoptan `BaseCrudService`, `CrudListBase`, interceptors, guards, validators, pipes, modelos y las **interfaces** (`TableColumn`, `FormFieldConfig`, `ActionConfig`), y se reconstruyen `generic-table/form/modal` con el CSS propio de MercadoMAX. Conserva look & feel y filosofía sin dependencias; coste = reconstruir ~6 componentes visuales una sola vez.
- ~~Opción A — Adoptar Angular Material~~ (descartada: overhaul visual, re-tematizado, +bundle y riesgo de regresión en pantallas ya funcionales).

> **Decisión confirmada (2026-06-07).** El roadmap se ejecuta bajo Opción B. En Fase 0 se reconstruyen los esqueletos visuales (`generic-table/form/modal`) con CSS propio reusando las interfaces del template; **no se instala Angular Material**.

## D.2. (Crítica) Contrato de paginación — ✅ DECIDIDA: **forma plana de MercadoMAX**

Se conserva la forma **plana** `{Data, TotalRecords, PageNumber, PageSize, TotalPages}` de MercadoMAX y se **adapta el `ApiService`/`BaseCrudService` del template** (que usa `Meta` anidado) al integrarlos. Menor impacto sobre el frontend y los listados ya escritos.

> **Decisión confirmada (2026-06-07).** El mapeo `meta.totalItems→totalRecords` / `meta.page→pageNumber` se resuelve en la capa `ApiService` al portarla; el resto de MercadoMAX no cambia su contrato.

## D.3. (Media) Convención de permisos
`"Modulo:Accion"` (MercadoMAX) vs `"module.action"` (template). Recomendación: conservar la de MercadoMAX (ya está en seeds, claims y atributos) y adaptar `PermissionConstants`/handler del template.

---

## Apéndice — Fuentes auditadas
- **MercadoMAX backend:** `Backend/` (7 APIs + Gateway + Shared), `MercadoMAX.slnx`, `GUIA_CODIFICACION.md`, `GUIA_PROYECTO.md`.
- **MercadoMAX frontend:** `Frontend/mercado-max/src/app/` (core/, features/, shared/).
- **MercadoMAX BD:** `Backup 07-06-26.sql`, `ScriptBD/`.
- **ERP Template backend:** `erp-template/backend/ErpBackend/ErpBackend.CrossCutting/` + `docs/MIGRATION-GUIDE.md`.
- **ERP Template frontend:** `erp-template/frontend/erp-frontend/src/app/shared/` + `features/_shared/crud-list.base.ts` + `features/catalogs/`.
