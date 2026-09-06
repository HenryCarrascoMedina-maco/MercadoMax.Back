# FASE1-RESULTADO.md
## Resultado de la migración piloto — ProductCategory con el ERP Template

> **Fecha:** 2026-06-07
> **Resultado global:** ✅ **Fase 1 completada.** Backend y frontend compilan; smoke backend 7/7 OK (incluido 409); no-regresión OK (Brand, Supplier, Guías).
> **Reglas respetadas:** SQL Server + Dapper + **SPs intactos** · permisos `Masters:*` · **sin Angular Material** · diseño visual conservado · ProductCategory único módulo migrado · genéricos reutilizables para los demás maestros.

---

## 1. Lista exacta de archivos creados/modificados

### Backend — creados (2)
- `Backend/MercadoMAX.Shared/CrossCutting/Data/SpExecutor.cs` — `ISpExecutor` + `SpExecutor` (ejecutor de SPs Dapper).
- `Backend/MercadoMAX.Shared/CrossCutting/Api/BaseApiController.cs` — controlador base (`OkResponse`/`CreatedResponse` + CorrelationId).

### Backend — modificados (4)
- `Backend/MercadoMAX.Shared/CrossCutting/Extensions/CrossCuttingExtensions.cs` — registra `ISpExecutor` (aditivo).
- `Backend/MercadoMAX.Maestros.API/Controllers/MasterControllers.cs` — **solo** `ProductCategoryController` (hereda `BaseApiController`, `PUT /{id}`, sin mapeo de error manual).
- `Backend/MercadoMAX.Maestros.API/Services/MasterServices.cs` — **solo** `IProductCategoryService`/`ProductCategoryService` (excepciones + `FromSpResult`; `UpdateAsync(int id, …)`).
- `Backend/MercadoMAX.Maestros.API/Repositories/MasterRepositories.cs` — **solo** `ProductCategoryRepository` (usa `ISpExecutor`; mismos SPs/params).

### Frontend — creados (16)
- `src/app/shared/styles/_erp-list.scss` — partial de estilos reutilizables (copia de `_master-list.scss`).
- `src/app/shared/models/table-column.model.ts`, `form-field-config.model.ts`, `action-config.model.ts` — contratos.
- `src/app/shared/directives/has-permission.directive.ts` — `*hasPermission`.
- `src/app/shared/components/data-table/` (`.ts` + `.scss`) — tabla genérica.
- `src/app/shared/components/dynamic-form/` (`.ts` + `.scss`) — formulario dinámico.
- `src/app/shared/components/actions/` (`.ts` + `.scss`) — acciones de fila.
- `src/app/shared/components/modal/` (`.ts` + `.scss`) — modal genérico.
- `src/app/shared/components/confirm-dialog/` (`.ts` + `.scss`) — confirmación.
- `src/app/shared/base/crud-list.base.ts` — base CRUD reutilizable.

### Frontend — modificados (6)
- `src/app/core/services/master.service.ts` — **solo** `ProductCategoryService` (extiende `BaseCrudService`).
- `src/app/features/maestros/product-category/product-category-list.component.ts` — usa `CrudListBase` + config.
- `src/app/features/maestros/product-category/product-category-list.component.html` — usa genéricos (`app-data-table`/`app-modal`/`app-dynamic-form`/`app-confirm-dialog`).
- `src/app/features/maestros/product/product-list.component.ts` — **1 línea** (`catSvc.list(true)` → `catSvc.getAll({status:true})`): ajuste obligado por el cambio de firma de `ProductCategoryService` (ver §9). Cambio de comportamiento nulo.
- `src/assets/i18n/es.json` y `en.json` — 5 claves nuevas (`common.deleteTitle/deleteConfirm/savedOk/deletedOk/statusOk`).

> El `.scss` de product-category **no cambió** (sigue siendo `@use '../master-list'`). Backup de los archivos modificados en `_backup_fase1/`.

---

## 2. Commit base · 3. Commit Fase 1
**Decisión del usuario: Git en pausa.** No se crearon commits. Motivo: `Backend/` ya es un repositorio Git propio con remoto y cambios ajenos sin commitear; se optó por **respaldo manual** en `_backup_fase1/` (archivos a modificar) en vez de commits. Rollback descrito en §8.
- Se eliminó el `.git` raíz creado por error; **`Backend/.git` quedó intacto** y no se tocó.

## 4. Resultado `dotnet build`
```
dotnet build MercadoMAX.slnx
Compilación correcta.  0 Advertencia(s)  0 Errores
```

## 5. Resultado `ng build`
```
EXIT_CODE = 0
Application bundle generation complete. [5.2 s]
Output location: dist/mercado-max
```
- Advertencias **preexistentes** (no nuevas de Fase 1): presupuesto de bundle (834 kB; +~35 kB por los genéricos — sigue siendo el warning de lazy-loading ya conocido) y SCSS de user/guide/role-list (componentes no tocados).

## 6. Smoke test backend ProductCategory (API real, localdb)
Servicios levantados: Auth (5001), Maestros (5002), Guías (5003) → `/health` = Healthy. Login `admin@mercadomax.com` → token OK.

| Caso | Resultado |
|---|---|
| **Listar** `GET /api/ProductCategory` | ✅ 200, 5 categorías |
| **Crear** `POST` | ✅ **201**, `data=6`, `correlationId` presente |
| **Duplicado** `POST` (mismo Name) | ✅ **409** `{errorCode: CONFLICT}` |
| **Editar** `PUT /{id}` | ✅ 200 |
| **Toggle status** `PATCH /{id}/toggle-status` | ✅ 200 (status→false) |
| **GetById** | ✅ 200, refleja edición + toggle |
| **Eliminar** `DELETE /{id}` | ✅ 200 (soft-delete) |

→ Cross-cutting verificado end-to-end: `SpExecutor` (SPs), excepciones→HTTP (409), `FromSpResult`, `BaseApiController` + CorrelationId. Datos de prueba limpiados (ProductCategory vuelve a **5** filas).

## 7. Smoke test frontend ProductCategory
- ✅ **Compilación** (`ng build` EXIT 0): el componente migrado, los 5 genéricos, la directiva `*hasPermission` y `CrudListBase` compilan e integran correctamente; tipos del contrato OK.
- ⚠️ **Verificación de UI en runtime (clic real en navegador):** no ejecutada — no dispongo de automatización de navegador headless en este entorno. La lógica subyacente (API) quedó **verificada end-to-end** en §6. Recomendado: `npm start` (ng serve) y validar manualmente listar/buscar/paginar/crear/editar/ver/eliminar/toggle/ocultamiento de botones por permiso. Puedo guiarte paso a paso si quieres.
- Diseño visual: garantizado por reutilizar las **mismas clases CSS** (`_erp-list.scss` = copia de `_master-list.scss`); pendiente confirmación visual en `ng serve`.

## 8. Confirmación de no-regresión
| Endpoint (no migrado) | Resultado |
|---|---|
| `GET /api/Brand` (5002) | ✅ 200, datos OK |
| `GET /api/Supplier` (5002) | ✅ 200, datos OK |
| `GET /api/Guide` (5003) | ✅ 200 |
→ El registro aditivo de `ISpExecutor` y los cambios de ProductCategory **no afectaron** a los demás maestros ni a Guías.

## 9. Incidencia encontrada y resuelta (transparencia)
- **Build frontend falló al primer intento:** `product-list.component.ts:46` llamaba `catSvc.list(true)` con la firma antigua de `ProductCategoryService`. Al migrar ese service a `BaseCrudService`, la firma de `list` cambió (`PaginationRequest`) → error TS2345.
- **Resolución (mínima y necesaria):** se cambió esa única línea a `catSvc.getAll({status:true})` (mismo resultado: categorías activas para el dropdown). Respaldado en `_backup_fase1/`. Re-build → EXIT 0.
- No hubo otros fallos. Tras el arreglo, todo verde.

## 10. Plan de rollback de código (Git en pausa → respaldo manual)
Restaurar desde `_backup_fase1/` (archivos modificados) y **eliminar** los archivos nuevos (§1). Pasos:
1. Copiar de vuelta: `MasterControllers.cs`, `MasterServices.cs`, `MasterRepositories.cs`, `CrossCuttingExtensions.cs`, `master.service.ts`, `product-category-list.component.{ts,html}`, `product-list.component.ts` (`.bak`).
2. Borrar nuevos: `SpExecutor.cs`, `BaseApiController.cs`, `shared/styles/_erp-list.scss`, `shared/models/{table-column,form-field-config,action-config}.model.ts`, `shared/directives/has-permission.directive.ts`, `shared/components/{data-table,dynamic-form,actions,modal,confirm-dialog}/`, `shared/base/crud-list.base.ts`.
3. Revertir 5 claves i18n en `es.json`/`en.json` (opcional; son aditivas e inertes).
4. `dotnet build` + `ng build` para confirmar regreso a verde.
> Los cambios de **Fase 0** se conservan. La BD no requiere rollback (no se tocó esquema/SPs; el dato de prueba ya se limpió).

---

## Resumen
- **Molde replicable listo:** un maestro nuevo ahora = `Service extends BaseCrudService` + `Component extends CrudListBase` con `columns`/`formFields`/`rowActions` declarativos. Backend: `repo→SpExecutor`, `controller→BaseApiController`, excepciones.
- **Sin Material, sin tocar SPs, permisos intactos, diseño conservado.**
- Pendiente no bloqueante: verificación visual en `ng serve`.
