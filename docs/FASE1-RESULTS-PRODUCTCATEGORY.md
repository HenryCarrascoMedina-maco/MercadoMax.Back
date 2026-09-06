# FASE1-RESULTS-PRODUCTCATEGORY.md
## Resultados de la migración piloto — ProductCategory (validada técnica y visualmente)

> **Fecha:** 2026-06-07 · **Estado:** ✅ **COMPLETADA Y VALIDADA** (builds verdes, smoke backend 7/7, no-regresión OK, validación visual OK).
> **Reglas respetadas:** SQL Server + Dapper + **SPs intactos** · permisos `Masters:*` · **sin Angular Material** · diseño visual conservado · ProductCategory único módulo migrado · genéricos reutilizables.

---

## 1. Archivos creados / modificados

### Backend — creados (2)
- `Backend/MercadoMAX.Shared/CrossCutting/Data/SpExecutor.cs` — `ISpExecutor` + `SpExecutor` (ejecutor de SPs Dapper).
- `Backend/MercadoMAX.Shared/CrossCutting/Api/BaseApiController.cs` — controlador base (`OkResponse`/`CreatedResponse` + CorrelationId).

### Backend — modificados (4)
- `…/CrossCutting/Extensions/CrossCuttingExtensions.cs` — registra `ISpExecutor` (aditivo).
- `…/Maestros.API/Controllers/MasterControllers.cs` — **solo** `ProductCategoryController` (hereda `BaseApiController`, `PUT /{id}`, sin mapeo de error manual).
- `…/Maestros.API/Services/MasterServices.cs` — **solo** `IProductCategoryService`/`ProductCategoryService` (excepciones + `FromSpResult`, `UpdateAsync(int id, …)`).
- `…/Maestros.API/Repositories/MasterRepositories.cs` — **solo** `ProductCategoryRepository` (usa `ISpExecutor`; mismos SPs/params).

### Frontend — creados (16)
- `shared/styles/_erp-list.scss` — partial de estilos reutilizables (copia de `_master-list.scss`) + `.req` + `.field-error` + resaltado `.ng-invalid.ng-touched`.
- `shared/models/{table-column,form-field-config,action-config}.model.ts` — contratos.
- `shared/directives/has-permission.directive.ts` — `*hasPermission`.
- `shared/components/data-table/` (`.ts`+`.scss`), `dynamic-form/` (`.ts`+`.scss`), `actions/` (`.ts`+`.scss`), `modal/` (`.ts`+`.scss`), `confirm-dialog/` (`.ts`+`.scss`).
- `shared/base/crud-list.base.ts` — base CRUD reutilizable.

### Frontend — modificados (6)
- `core/services/master.service.ts` — **solo** `ProductCategoryService` → extiende `BaseCrudService`.
- `features/maestros/product-category/product-category-list.component.ts` — usa `CrudListBase` + config.
- `features/maestros/product-category/product-category-list.component.html` — usa genéricos; `@if (modalOpen())` para recrear el form.
- `features/maestros/product/product-list.component.ts` — **1 línea** (`catSvc.list(true)` → `catSvc.getAll({status:true})`).
- `src/assets/i18n/es.json` y `en.json` — 7 claves nuevas (`common.deleteTitle/deleteConfirm/savedOk/deletedOk/statusOk/fieldRequired/invalidField`).

> Respaldos en `_backup_fase1/`. **Git en pausa** por decisión del usuario (`Backend/.git` intacto, no tocado).

---

## 2. Qué se logró
- **Backend ProductCategory** sobre el cross-cutting del ERP Template, **sin tocar los 6 SPs ni los DTOs**: `SpExecutor` (centraliza Dapper), `BaseApiController` (envelope + CorrelationId), excepciones → HTTP (incluido **409** para duplicado), `FromSpResult`.
- **Frontend ProductCategory** sobre genéricos propios **sin Angular Material**: `data-table`, `dynamic-form`, `actions`, `modal`, `confirm-dialog`, directiva `*hasPermission`, `CrudListBase`, `ProductCategoryService extends BaseCrudService`.
- **Diseño visual idéntico** (reutilización de las mismas clases CSS vía `_erp-list.scss`).
- **Molde replicable** para los 15 maestros restantes (ver §9).
- Manejo de errores **centralizado** (interceptor + `ErrorHandlerService`); permisos `Masters:*` conservados (ruta por `permissionGuard`, acciones por `*hasPermission`).

## 3. Qué problemas aparecieron
1. **Build frontend (compilación):** `product-list` (no migrado) consumía `ProductCategoryService.list(true)` con la firma vieja → `TS2345` al cambiar a `BaseCrudService`.
2. **UX – form no se limpiaba:** al reabrir "Nueva Categoría" tras crear una, el formulario arrastraba los datos anteriores (la instancia de `dynamic-form` proyectada por `ng-content` persistía; con `[value]=null` el setter no se redisparaba).
3. **UX – no se distinguían los campos requeridos** (sin indicador visual).
4. **UX – sin aviso al dejar vacío un requerido** (no se mostraba validación al perder foco).

## 4. Cómo se resolvieron
1. Cambio mínimo y necesario de 1 línea en `product-list`: `getAll({status:true})` (mismo resultado). Respaldado.
2. `@if (modalOpen())` envolviendo `<app-dynamic-form>` → la instancia se **recrea** en cada apertura (form fresco en crear, precargado en editar). Además el setter `value` hace `reset()`+`patchValue()`.
3. Asterisco `*` rojo en `label` para campos `required` (`dynamic-form` + `.req` en el partial).
4. Mensaje de validación por campo on-blur: `errorKey(field)` muestra `common.fieldRequired` cuando el control está `touched/dirty` e inválido; borde rojo vía `.ng-invalid.ng-touched`.

> Los 4 arreglos viven en los **componentes genéricos**, por lo que ya benefician a todos los maestros siguientes.

## 5. Evidencia de builds
- **Backend:** `dotnet build MercadoMAX.slnx` → `Compilación correcta. 0 Advertencia(s) 0 Errores`.
- **Frontend:** `ng build` → `EXIT_CODE=0`, `Application bundle generation complete`. `ng serve` recompila en caliente sin errores tras los ajustes de UX. (Advertencias de presupuesto de bundle/SCSS = **preexistentes**, no nuevas.)

## 6. Evidencia de smoke tests (API real sobre `(localdb)\MSSQLLocalDB`)
Auth(5001)+Maestros(5002)+Guías(5003) `Healthy`; login admin OK.

| Caso | Resultado |
|---|---|
| Listar `GET /api/ProductCategory` | ✅ 200 |
| Crear `POST` | ✅ **201** (`correlationId` presente) |
| Duplicado `POST` mismo Name | ✅ **409** `errorCode: CONFLICT` |
| Editar `PUT /{id}` | ✅ 200 |
| Toggle `PATCH /{id}/toggle-status` | ✅ 200 |
| GetById | ✅ 200 (refleja edición + toggle) |
| Eliminar `DELETE /{id}` | ✅ 200 (soft-delete) |
| **No-regresión** Brand / Supplier (5002) / Guías (5003) | ✅ 200 / 200 / 200 |

## 7. Estado visual (validación manual del usuario)
- ✅ Pantalla Categorías carga; **diseño idéntico** al anterior (encabezado, buscador, tabla con sombra, badges, íconos 👁️✏️🗑️).
- ✅ Tabla, paginación ("Mostrando 1–N de N", selector de tamaño), búsqueda server-side.
- ✅ Modal crear/editar/ver (deshabilitado), confirm dialog de borrado (no `window.confirm`), toggle de estado, alerta de **duplicado 409**.
- ✅ Tras correcciones: form se limpia al reabrir, asterisco en requeridos, aviso de validación on-blur con borde rojo.
- ✅ Brand y Supplier (no migrados) se ven y funcionan igual.
- ⏸️ **Permisos UI (ocultamiento):** validado el caso admin (ve todo). El ocultamiento por falta de permiso queda por verificar con un usuario `Masters:List` sin `Create/Update/Delete` (no existe semilla así; pendiente opcional, sin tocar datos).

## 8. Lecciones aprendidas
- **Contratos primero:** el `ApiResponse`/paginación plano y `SpResult` ya existentes hicieron el backend casi mecánico.
- **Cuidado con consumidores cruzados:** cambiar la firma de un service compartido (ProductCategory) rompe a quien lo usa para dropdowns (Product). Al migrar un maestro, **buscar sus consumidores** (`grep <Servicio>`) y ajustarlos a `getAll`/`getById`.
- **Content projection + estado:** un componente proyectado por `ng-content` **no** se reinicia con el `@if` del contenedor; para "form nuevo" hay que **recrearlo** (`@if` en el consumidor) o resetearlo explícitamente.
- **Validación visible:** required (asterisco) + error on-blur deben venir de fábrica en el form genérico; añadirlos una vez sirve a todos.
- **Encapsulación CSS:** reutilizar el look exige que cada genérico cargue el partial compartido (estilos scoped por componente) → cero regresión sin Material.
- **El smoke real vale oro:** levantar las APIs y probar el 409/CRUD detectó que el contrato y el pipeline funcionan de verdad, no solo que compila.

## 9. Patrón final para migrar el siguiente maestro
**Backend (por entidad):**
1. `Repository`: inyectar `ISpExecutor`; reemplazar plomería Dapper por `QueryListSpAsync`/`QuerySingleSpAsync`/`ExecSpResultAsync` (mismos SPs/params).
2. `Service`: lanzar `NotFoundException`/`ConflictException`/`BusinessException` (mapear mensajes de duplicado del SP) y usar `ApiResponse.FromSpResult` en el camino feliz; `UpdateAsync(int id, req)`.
3. `Controller`: heredar `BaseApiController`; endpoints REST (`GET`, `GET /{id}`, `POST`→201, `PUT /{id}`, `DELETE /{id}`, `PATCH /{id}/toggle-status`) con `[Authorize(Policy="Masters:*")]`.
4. `ISpExecutor` ya está registrado globalmente (no tocar DI de otros).

**Frontend (por entidad):**
1. `XService extends BaseCrudService<XResponse, CreateX, UpdateX, number>` con `baseUrl` + `resource`. **Buscar y migrar consumidores** que usaban `.list(...)` → `getAll(...)`.
2. `XListComponent extends CrudListBase<…>`: declarar `columns`, `formFields` (con `required`), `rowActions` (con `permission`), `service`, `ngOnInit→loadData()`, `formValue()`.
3. HTML = copia del de ProductCategory: page-header + buscador + `<app-data-table>` + `<app-modal>`(con `@if(modalOpen())` y `<app-dynamic-form>`) + `<app-confirm-dialog>`.
4. SCSS = `@use '../master-list' as *;` (igual). Claves i18n del módulo (`X.title/new/searchPlaceholder/createTitle/editTitle/notFound/namePlaceholder`).
5. Para campos `select` (FKs) usar `type: 'select'` con `options` cargadas vía `getAll` de los services relacionados.

**Verificación (siempre):** `dotnet build` + `ng build` verdes · smoke API (CRUD + 409) · no-regresión de 1-2 módulos vecinos · revisión visual.

---

## Estado de cierre de sesión (2026-06-07)
- ✅ **Datos de prueba limpiados:** `master.ProductCategory` quedó con **solo las 5 categorías semilla** (Frutas, Verduras, Tubérculos, Legumbres, Granos), todas **Activas** (se restauró Frutas, que había quedado inactiva por el toggle de prueba).
- ✅ **Servidores detenidos:** `ng serve` (4200), Auth (5001), Maestros (5002), Guías (5003) — los 4 puertos libres.
- ⏸️ **Git:** pendiente por decisión del usuario (respaldo manual en `_backup_fase1/`; `Backend/.git` intacto).

## ▶️ Próximo paso recomendado — FASE 2
**Planificar la migración de `Supplier` como Fase 2** (generar `FASE2-PLAN-SUPPLIER.md`, sin código) replicando el patrón del §9, considerando lo específico de Supplier:
- Form con **varios campos** (BusinessName, TaxId, teléfono, provincia/departamento, contacto…).
- **TaxId único → 409:** agregar el mensaje de duplicado del SP de Supplier al mapeo de `ConflictException` (es por-entidad).
- **`UserId` opcional (FK):** primer campo `select` poblado desde otro service (usuarios) → valida ese tipo del form genérico.
- **Consumidores cruzados de `SupplierService`** (p. ej. dropdown en `brand-list`): migrar sus llamadas `.list(...)` → `getAll(...)`.

> Retomar en la siguiente sesión con `FASE2-PLAN-SUPPLIER.md`. Supplier **no** se migró ni se planificó todavía (según lo acordado).
