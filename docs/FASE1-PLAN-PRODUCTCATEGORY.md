# FASE1-PLAN-PRODUCTCATEGORY.md
## Plan técnico — Módulo piloto **ProductCategory** con el ERP Template

> **Estado:** plan técnico. **NO se modifica código en este documento.**
> **Fecha:** 2026-06-07 · **Pre-requisito:** Fase 0 aplicada y verificada (ver `POST-FASE0-VALIDATION.md`).
> **Decisiones vigentes:** mantener **SQL Server + Dapper + Stored Procedures + permisos `Masters:*` + diseño visual actual**; adoptar **CrossCutting / ApiResponse / PagedResponse plano / BaseCrudService / ApiService / errores centralizados / Generic Table-Form-Actions-Modal**, **sin Angular Material** (Opción B).
> **Alcance:** un solo módulo (ProductCategory). Sirve de **molde** para los 15 maestros restantes.

---

## 1. Inventario actual de ProductCategory

### 1.1 Base de datos (`master` schema)
- Tabla `master.ProductCategory`: `Id` (PK IDENTITY), `Name` (UQ `UQ_ProductCategory_Name`), `Description`, `Status` (bit), `CreatedAt`, `UpdatedAt`. **5 filas** (datos semilla).
- Referenciada por `master.Product.CategoryId` (FK `FK_Product_Category`).

### 1.2 Stored Procedures (`ScriptBD/06_SP_Master.sql`) — **se conservan intactos**
| SP | Parámetros | Comportamiento relevante |
|---|---|---|
| `SP_LIST_PRODUCT_CATEGORY` | `@Status BIT=NULL, @Search NVARCHAR(100)=NULL` | Filtra por estado y `Name LIKE '%@Search%'`. **No paginado** (devuelve todas las filas). |
| `SP_READ_PRODUCT_CATEGORY` | `@Id INT` | Devuelve 1 fila. |
| `SP_CREATE_PRODUCT_CATEGORY` | `@Name, @Description` | Si `Name` existe → `SELECT 0,'Category already exists',0`. Si no → inserta y devuelve `1,'...',SCOPE_IDENTITY()`. |
| `SP_UPDATE_PRODUCT_CATEGORY` | `@Id,@Name,@Description,@Status` | Si `Name` repetido en otro Id → `0,'Category name already in use'`. Si no → update. |
| `SP_DELETE_PRODUCT_CATEGORY` | `@Id` | Soft-delete (`Status=0`). Si hay productos activos → `0,'Cannot deactivate: category has active products'`. |
| `SP_TOGGLE_STATUS_PRODUCT_CATEGORY` | `@Id` | Alterna `Status`. |

> Todos retornan el contrato `SpResult { int Success; string Message; int Id }`.

### 1.3 Backend (`MercadoMAX.Maestros.API`)
| Capa | Archivo | ProductCategory |
|---|---|---|
| Controller | `Controllers/MasterControllers.cs` | `ProductCategoryController : ControllerBase`, `[Route("api/[controller]")]`, `[Authorize]`. Endpoints: `GET ?status&search`, `GET /{id}`, `POST` (`Masters:Create`), **`PUT`** (base, id en body, `Masters:Update`), `DELETE /{id}` (`Masters:Delete`), `PATCH /{id}/toggle-status` (`Masters:Delete`). Mapeo HTTP manual (`Success ? Ok : BadRequest/NotFound`). |
| Service | `Services/MasterServices.cs` | `IProductCategoryService` + `ProductCategoryService`. Adaptador `r.Success==1 ? ApiResponse.Ok : ApiResponse.Fail`. |
| Repository | `Repositories/MasterRepositories.cs` (+ `IMasterRepositories.cs`) | `ProductCategoryRepository`. Plomería Dapper inline (`using conn; QueryFirstAsync<SpResult>/QueryAsync<...>`). |
| DTOs | `DTOs/MasterDTOs.cs` | `CreateProductCategoryRequest{Name,Description}`, `UpdateProductCategoryRequest{Id,Name,Description,Status}`, `ProductCategoryResponse{Id,Name,Description,Status,CreatedAt,UpdatedAt}`. |
| Models | `Models/MasterModels.cs` | POCO de mapeo. |
| Respuesta | `MercadoMAX.Shared.DTOs.ApiResponse<T>` (ya con `FromSpResult` y `CorrelationId` desde Fase 0). |

### 1.4 Frontend (`Frontend/mercado-max/src/app`)
| Pieza | Archivo | Detalle |
|---|---|---|
| Service | `core/services/master.service.ts` → `ProductCategoryService` | `url=${BASE}/ProductCategory`. CRUD manual con `HttpClient`: `list(status?,search?)`, `getById`, `create`, `update`(PUT base), `delete`, `toggleStatus`. |
| Modelos | `core/models/master.model.ts` | `ProductCategoryResponse`, `Create/UpdateProductCategoryRequest`. |
| Component | `features/maestros/product-category/product-category-list.component.ts` | Standalone + signals. Tabla manual, modal manual, reactive form manual, `msg()`+setTimeout, `confirm()`, paginación **cliente** (`pagedItems = slice`), búsqueda **servidor** (recarga). |
| HTML | `...product-category-list.component.html` | `<table>` manual, badge-toggle, acciones emoji 👁️✏️🗑️, `app-pagination`, modal `.modal-overlay/.modal` con form. |
| SCSS | `...product-category-list.component.scss` | **1 línea**: `@use '../master-list' as *;` → todo el estilo vive en el parcial compartido `features/maestros/_master-list.scss` (clases `master-list-page`, `page-header`, `toolbar`, `search-input`, `table-container`, `table`, `badge`/`badge-toggle`, `btn`/`btn-icon`, `modal-overlay`, `modal`, `form-group`, `alert`). |
| Reutilizable existente | `shared/components/pagination/pagination.component.ts` | Paginador cliente (`totalItems`, `pageChange`, `pageSizeChange`). |

---

## 2. Backend — plan

> Principio: **los 6 SPs y los DTOs no se tocan.** Solo cambia la *plomería* (cómo se invoca el SP, cómo se serializa, cómo se manejan errores).

### 2.1 `SpExecutor` (la pieza que el Template no trae — se construye ahora)
- **Ubicación:** `MercadoMAX.Shared/CrossCutting/Data/SpExecutor.cs` (+ `ISpExecutor`).
- **Dependencia:** envuelve el `DbConnectionFactory` existente (Fase 0 lo conserva). No reemplaza Dapper; lo encapsula.
- **API propuesta (especificación, no implementación):**
  - `Task<TResult> QuerySingleSpAsync<TResult>(string sp, object? p = null)` → `QueryFirstOrDefaultAsync`.
  - `Task<List<TResult>> QueryListSpAsync<TResult>(string sp, object? p = null)` → `QueryAsync` → `ToList`.
  - `Task<SpResult> ExecSpResultAsync(string sp, object? p = null)` → `QueryFirstAsync<SpResult>`.
  - *(Futuro, no en el piloto)* `QueryPagedSpAsync<T>` con `QueryMultiple` (items+total) para módulos transaccionales con SPs paginados.
- **DI:** registrar en `AddMercadoMaxCrossCutting` (o método `AddSpExecutor`) como `Scoped`. Aditivo; no afecta a los repos que aún no lo usan.

### 2.2 `BaseApiController`
- **Ubicación:** `MercadoMAX.Shared/CrossCutting/Api/BaseApiController.cs`.
- **Especificación:** `[ApiController]`, hereda `ControllerBase`. Helpers `protected` que devuelven `ApiResponse<T>`/`PagedResponse<T>` planos con el `CorrelationId` del `HttpContext`:
  - `OkResponse<T>(T data, string msg)`, `CreatedResponse<T>(...)` (201), `NoContent(msg)`.
- **Adopción en ProductCategory:** `ProductCategoryController` pasa a heredar `BaseApiController`. El **mapeo HTTP de errores ya no se hace a mano**: las excepciones (`NotFound/Conflict/Business`) las traduce el `GlobalExceptionMiddleware` (Fase 0).

### 2.3 `ApiResponse` / `PagedResponse` plano + `FromSpResult`
- Ya existen (Fase 0). En el piloto:
  - Listado → `ApiResponse<List<ProductCategoryResponse>>` (no paginado; igual que hoy).
  - Create/Update/Delete/Toggle → el service construye la respuesta con `ApiResponse<T>.FromSpResult(spResult, data)` en el **camino feliz**, y **lanza excepción** en el camino de error (ver 2.5). Se elimina el ternario `Success==1 ? Ok : Fail`.

### 2.4 Adaptación del repository
- `ProductCategoryRepository` deja de usar `DbConnectionFactory` + Dapper inline y pasa a depender de `ISpExecutor`:
  - `ListAsync(status,search)` → `QueryListSpAsync<ProductCategoryResponse>("master.SP_LIST_PRODUCT_CATEGORY", new {Status,Search})`.
  - `GetByIdAsync(id)` → `QuerySingleSpAsync<ProductCategoryResponse>("master.SP_READ_PRODUCT_CATEGORY", new {Id})`.
  - `Create/Update/Delete/ToggleStatus` → `ExecSpResultAsync("master.SP_...", ...)`.
- **Nombres de SP, parámetros y firmas de retorno se conservan.** Solo cambia el medio de ejecución.

### 2.5 `ConflictException` para duplicados → HTTP 409
- Los SP señalan duplicado con `Success=0` + mensaje conocido (`'Category already exists'`, `'Category name already in use'`).
- **Mapeo en el service** (sin tocar el SP):
  - Si `SpResult.Success==0` y el mensaje pertenece al conjunto de duplicado → `throw new ConflictException(message)` → **409**.
  - Si `Success==0` por otra regla de negocio (p. ej. `'Cannot deactivate: category has active products'`) → `throw new BusinessException(message)` → **400**.
  - `GetById` sin fila → `throw new NotFoundException("ProductCategory", id)` → **404**.
  - Camino feliz → `FromSpResult`.
- **Detección de duplicado:** encapsulada en un helper privado del service con la lista de mensajes de duplicado conocidos (puntual y revisable). *Nota:* es un puente pragmático para no modificar SPs; mejora futura (diferida) = que los SP devuelvan un `ErrorCode` distinguible.

### 2.6 Qué se mantiene SIN tocar
- Los 6 SPs, los DTOs (`Create/Update/ProductCategoryResponse`), el modelo POCO.
- La convención de permisos `Masters:Create/Update/Delete` y `[Authorize]`.
- El contrato de datos consumido por el frontend (forma `ApiResponse<T>` con `success/message/data`).
- JWT, autorización, el resto de entidades de Maestros (Brand, Supplier, etc.) — **intactas**.

### 2.7 Qué se modifica (solo ProductCategory)
- `ProductCategoryController` → hereda `BaseApiController`; **`PUT` pasa de base (`PUT /api/ProductCategory` con Id en body) a RESTful `PUT /api/ProductCategory/{id}`** para alinear con `BaseCrudService.update(id,payload)`. Es el único consumidor (su frontend), que se migra en el mismo PR.
- `ProductCategoryService` → excepciones + `FromSpResult` (elimina el adaptador ternario).
- `ProductCategoryRepository` → `ISpExecutor`.
- DI de Maestros (`Program.cs`) → registrar `ISpExecutor` (1 línea aditiva; no afecta a los demás repos).

### 2.8 Qué queda preparado para los demás maestros
- `SpExecutor` + `BaseApiController` + el patrón de mapeo de excepciones quedan **reutilizables** para Brand, Supplier, LogisticUnit, etc.
- Opcional (fase posterior): un `CrudControllerBase<...>` para no repetir el árbol CRUD; **no** se introduce en el piloto para mantener el cambio acotado.

---

## 3. Frontend — plan (sin Angular Material)

> Los genéricos se construyen reusando los **contratos** del ERP Template (`TableColumn`, `FormFieldConfig`, `ActionConfig`) pero con **markup propio que emite las mismas clases CSS de `_master-list.scss`** → cero regresión visual.

### 3.1 `ApiService` / `BaseCrudService` (ya creados en Fase 0)
- `ProductCategoryService` se reescribe para **extender `BaseCrudService<ProductCategoryResponse, CreateProductCategoryRequest, UpdateProductCategoryRequest, number>`** con `baseUrl = environment.maestrosApiUrl`, `resource = 'ProductCategory'`.
- Listado no paginado → usa `getAll({ status, search })` (devuelve el array desempaquetado de `ApiResponse.data`).
- `create/update(id,payload)/delete/toggleStatus` → heredados.
- Búsqueda server-side (vía `@Search` del SP) se mantiene recargando `getAll` con `search`; la **paginación queda client-side dentro de la tabla genérica** (igual UX que hoy; no requiere modificar el SP).

### 3.2 `generic-table` sin Material
- **Ubicación:** `src/app/shared/components/data-table/`.
- **Contrato:** `TableColumn<T> { key; label; type?: 'text'|'badge'|'date'|...; sortable?; formatter?; align? }` (interfaz TS, sin dependencias UI).
- **Inputs/Outputs (signals/inputs Angular 21):** `[columns]`, `[data]`, `[loading]`, `[rowActions]`, `[pageSize]`, `(action)`, `(rowClick)`.
- **Render:** `@for` sobre filas con el **mismo `<table>`/`<thead>`/`<tbody>` y clases `table-container`, `table`, `badge badge-toggle`, `empty`** que el HTML actual → estilos heredados de `_master-list.scss`.
- **Paginación:** reutiliza el `PaginationComponent` existente (cliente) o lo integra; **misma apariencia**.
- **Estado de columna:** la celda de `Status` se renderiza con el `badge-toggle` clickable (emite acción `toggleStatus`).

### 3.3 `generic-form` (dinámico) sin Material
- **Ubicación:** `src/app/shared/components/dynamic-form/`.
- **Contrato:** `FormFieldConfig { key; label; type: 'text'|'textarea'|'number'|'select'|'checkbox'; validators?; placeholder?; options?; disabled? }`.
- Construye un `FormGroup` reactivo a partir de `fields`. Render con `form-group`/`label`/`input`/`textarea` **idénticos a los actuales** (clases de `_master-list.scss`).
- Modos `create`/`edit`/`view` (en `view` deshabilita controles, como hoy `form.disable()`).
- Emite `(formSubmit)` con el valor tipado; `(cancel)`.

### 3.4 `generic-actions`
- **Ubicación:** `src/app/shared/components/actions/`.
- **Contrato:** `ActionConfig<T> { key; icon; label; permission?; hidden?; }`. Para ProductCategory: ver 👁️ / editar ✏️ / eliminar 🗑️ (mismos emojis y `btn-icon`).
- Emite `(action)={key,row}`. Respeta permisos vía la directiva `*hasPermission` (3.7).

### 3.5 `generic-modal`
- **Ubicación:** `src/app/shared/components/modal/`.
- Reemplaza el patrón manual `showModal`+overlay. Render con `.modal-overlay`/`.modal`/`.modal-header`/`.modal-body`/`.modal-footer` **idénticos** → estilos heredados.
- Inputs `[title]`, `[open]`; `(close)`. Se compone con `generic-form` dentro para el caso CRUD.
- Confirmación de borrado: un `confirm-dialog` ligero (mismo estilo de modal) reemplaza `window.confirm()`.

### 3.6 Migración del componente ProductCategory usando los genéricos
- `product-category-list.component.ts` adelgaza a **configuración declarativa** + (opcional) extender un `CrudListBase`:
  - `columns: TableColumn[]` (Id, Name, Description, Status-badge).
  - `formFields: FormFieldConfig[]` (name required, description textarea).
  - `rowActions: ActionConfig[]` (view/edit/delete con permisos).
  - Acciones (`openCreate/openEdit/openView/save/delete/toggleStatus`) orquestadas por `CrudListBase` + `ProductCategoryService`.
- `CrudListBase` (en `src/app/shared/base/`): signals `rows/loading`, métodos `load/openCreate/openEdit/openView/confirmDelete/save/toggleStatus`, usando el `ModalService`/`generic-modal` y el manejo de errores centralizado.
- El HTML del componente pasa a usar `<app-data-table>`, `<app-generic-modal>`, `<app-dynamic-form>` — **mismas clases, mismo aspecto**.

### 3.7 Conservar Transloco, signals y permisos `Masters:*`
- **Transloco:** los genéricos reciben las etiquetas ya traducidas (o claves) — se conservan las claves existentes (`categories.*`, `common.*`). Sin textos hardcodeados en inglés.
- **Signals:** los genéricos y `CrudListBase` usan `signal`/`computed`/`input()` (Angular 21). Nada de NgRx.
- **Permisos UI:** se incorpora una directiva estructural `*hasPermission` (en `src/app/shared/directives/`) basada en `AuthService.hasPermission()` (ya existe). Los botones Crear/Editar/Eliminar se ocultan según `Masters:Create/Update/Delete`. La ruta sigue protegida por `permissionGuard` con `Masters:List` (sin cambios).
- **Errores centralizados:** los `subscribe(...).error` dejan de hacer `err.error?.message` a mano; el `errorInterceptor` + `ErrorHandlerService` (Fase 0) normalizan; el `CrudListBase` muestra el mensaje (toast/alert con la clase `alert` existente). El **409 de duplicado** llega como error HTTP y se muestra con su mensaje.

---

## 4. Criterios de aceptación
| # | Criterio | Cómo se valida |
|---|---|---|
| 1 | **Listado** | `GET /api/ProductCategory` devuelve las 5 categorías; la tabla genérica las muestra. |
| 2 | **Paginación** | Paginador (cliente) cambia de página/tamaño sin recargar datos; mismo comportamiento actual. |
| 3 | **Búsqueda** | El input filtra vía `@Search` del SP (server-side) y refresca la tabla. |
| 4 | **Crear** | Alta válida → 201 + categoría aparece en el listado; modal se cierra. |
| 5 | **Editar** | `PUT /{id}` actualiza Name/Description/Status; refleja en tabla. |
| 6 | **Eliminar** | Soft-delete (`Status=0`); si tiene productos activos → 400 con mensaje de negocio. |
| 7 | **Activar/Desactivar** | `PATCH /{id}/toggle-status` alterna el badge sin recargar toda la página. |
| 8 | **Duplicado → 409** | Crear/editar con Name existente → **HTTP 409** + mensaje del SP mostrado por el manejo centralizado. |
| 9 | **Permisos UI** | Usuario sin `Masters:Create/Update/Delete` no ve esos botones (`*hasPermission`); la API responde 403 si se fuerza. |
| 10 | **Errores centralizados** | Errores HTTP (404/409/400/500/0) producen un mensaje legible único vía `ErrorHandlerService`; sin `err.error?.message` disperso. |
| 11 | **No regresión visual** | La pantalla se ve **idéntica** (mismas clases de `_master-list.scss`); comparación visual antes/después. |

---

## 5. Checklist de validación
- [ ] **`dotnet build MercadoMAX.slnx`** → 0 errores (Shared + 7 APIs + Gateway).
- [ ] **`ng build`** → EXIT 0 (sin nuevos errores; warnings de bundle preexistentes aceptables).
- [ ] **Smoke test backend (API real):** levantar Maestros.API (5002) + Auth (5001) + Gateway (5000); login; `GET /api/ProductCategory`; crear, editar, toggle, eliminar; provocar duplicado → 409; sin permiso → 403.
- [ ] **Smoke test frontend:** navegar a Categorías; listar/buscar/paginar/crear/editar/ver/eliminar/toggle; ver mensaje de duplicado; ocultamiento de botones por permiso.
- [ ] **No regresión de otros módulos:** abrir Brand/Supplier/otro maestro **no migrado** y confirmar que siguen funcionando (comparten Maestros.API; el `ISpExecutor` es aditivo y no toca sus repos). Smoke test de un endpoint de Guías para confirmar que el cross-cutting global no afectó.
- [ ] **DB sin cambios de datos:** `ProductCategory` mantiene su set (salvo las altas de prueba, que se limpian).

---

## 6. Plan de rollback de código
> El repo de MercadoMAX **no es git** (no hay control de versiones local). Por eso el rollback se hace por **copia de respaldo de archivos** antes de editar.

**Antes de iniciar la Fase 1**, copiar los archivos a modificar a `_backup_fase1/` (ruta espejo):
- Backend:
  - `Backend/MercadoMAX.Maestros.API/Controllers/MasterControllers.cs`
  - `Backend/MercadoMAX.Maestros.API/Services/MasterServices.cs`
  - `Backend/MercadoMAX.Maestros.API/Repositories/MasterRepositories.cs`
  - `Backend/MercadoMAX.Maestros.API/Repositories/IMasterRepositories.cs`
  - `Backend/MercadoMAX.Maestros.API/Program.cs`
- Frontend:
  - `Frontend/mercado-max/src/app/core/services/master.service.ts`
  - `Frontend/mercado-max/src/app/features/maestros/product-category/` (los 3 archivos)
  - `Frontend/mercado-max/src/app/app.config.ts` (si se registra el `ModalService`/directiva)

**Archivos NUEVOS de la Fase 1** (su rollback = eliminarlos):
- Backend: `MercadoMAX.Shared/CrossCutting/Data/SpExecutor.cs`, `MercadoMAX.Shared/CrossCutting/Api/BaseApiController.cs`.
- Frontend: `shared/components/data-table/`, `shared/components/dynamic-form/`, `shared/components/actions/`, `shared/components/modal/`, `shared/base/crud-list.base.ts`, `shared/directives/has-permission.directive.ts`, contratos `shared/models/table-column.model.ts`, `form-field-config.model.ts`, `action-config.model.ts`.

**Cómo volver al ProductCategory anterior si algo falla:**
1. Restaurar los archivos modificados desde `_backup_fase1/` (sobrescribir).
2. Eliminar los archivos nuevos listados arriba.
3. `dotnet build` + `ng build` para confirmar que se vuelve al estado previo (verde).
4. La BD no requiere rollback (Fase 1 no toca esquema ni SPs); si se hicieron altas de prueba en `ProductCategory`, borrarlas manualmente.
> Nota: los cambios de **Fase 0** (CrossCutting, middlewares, ApiService/BaseCrudService) **se conservan**; el rollback de Fase 1 no los revierte.

**Recomendación:** inicializar git en el repo de MercadoMAX antes de la Fase 1 para tener rollback atómico por commit (sustituye al respaldo manual). Opcional pero recomendado.

---

## Resumen
- Backend: **se conservan SPs/DTOs/permisos**; se adopta `SpExecutor` + `BaseApiController` + excepciones (`ConflictException`→409) + `FromSpResult`. Cambio acotado a ProductCategory + 1 línea de DI.
- Frontend: `ProductCategoryService` extiende `BaseCrudService`; se construyen `data-table/dynamic-form/actions/modal` **sin Material**, emitiendo las **mismas clases CSS** → cero regresión visual; Transloco, signals y permisos `Masters:*` intactos; errores centralizados.
- Sale un **molde replicable** para los 15 maestros restantes.

> **No se escribió código.** A la espera de tu OK para implementar la Fase 1 siguiendo este plan.
