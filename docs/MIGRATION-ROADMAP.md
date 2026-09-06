# MIGRATION-ROADMAP.md
## Plan de integración del ERP Template en MercadoMAX — por fases

> **Estado:** plan técnico. **No incluye código.** Acompaña a `MIGRATION-ANALYSIS.md`.
> **Fecha:** 2026-06-07
> **Alcance:** Backend (.NET 10, Dapper+SP) + Frontend (Angular 21).

---

## 0. Convenciones del roadmap

**Esfuerzo** (orientativo, equipo de 1-2 devs):
`S` ≈ 1-2 días · `M` ≈ 3-5 días · `L` ≈ 1-2 semanas · `XL` ≈ 3+ semanas.

**Riesgo:** 🟢 bajo · 🟠 medio · 🔴 alto.
**Impacto:** ⭐ (limpieza puntual) → ⭐⭐⭐ (transversal, alto ahorro).

**Principio rector:** el ERP Template cambia *la plomería* (cómo se invoca el SP, cómo se serializa la respuesta, cómo se manejan errores/JWT, cómo se renderiza tabla/form/modal). **NO toca la lógica de negocio (153 SPs) ni los contratos consumidos hoy por el frontend.** Migración incremental, de menor a mayor criticidad, **Auth al final**.

**Dos decisiones previas (de `MIGRATION-ANALYSIS.md` §D) — ya CERRADAS (2026-06-07):**
1. ✅ **Estrategia Material → Opción B:** reconstruir la UI sin Material, reusando interfaces y lógica del template. **No se instala Angular Material.**
2. ✅ **Contrato de paginación → forma plana de MercadoMAX:** adaptar el `ApiService`/`BaseCrudService` del template al integrarlos.

---

## FASE 0 — Cimientos y decisiones (preparación, sin cambio de comportamiento de negocio)

**Objetivo:** dejar el terreno listo para migrar un módulo, sin alterar funcionalidad existente.

### Backend
| Tarea | Esfuerzo | Riesgo | Acción tabla |
|---|---|---|---|
| Incorporar `ErpBackend.CrossCutting` a la solución MercadoMAX como proyecto referenciado por los 7 APIs (o fusionarlo con `MercadoMAX.Shared`) | M | 🟢 | REPLACE/MERGE |
| Cerrar decisión **contrato de paginación** y dejar `ApiResponse`/`PagedResponse` unificados | S | 🟠 | MERGE #1,#2 |
| Cerrar decisión **convención de permisos**; alinear `Security/*` del template a `"Modulo:Accion"` | S | 🟠 | MERGE #11 |
| Construir el **`SpExecutor`/`BaseSpRepository`** propio (hueco no provisto por el template): ejecutar SP + paginación → `PagedResult<T>` | M | 🟢 | KEEP+construir #5 |
| Cablear bootstrap unificado: `AddErpCrossCutting()` + `UseErpCrossCutting()` (middlewares) + `AddErpJwtAuthentication()` + `AddErpAuthorization()` en **un** servicio piloto | S | 🟢 | REPLACE #8,#9,#10 |

### Frontend
| Tarea | Esfuerzo | Riesgo | Acción tabla |
|---|---|---|---|
| Portar capa **no-visual** del template: `ApiService`+`BaseCrudService` (adaptados a paginación plana), modelos, `permissionGuard`(rico), `error`+`loading` interceptors, `*appHasPermission`, `CustomValidators`, pipes, `PermissionService`/`ErrorHandlerService`/`LoadingService` | M | 🟢 | REPLACE/MERGE #1,#3,#11-16 |
| Integrar `error`/`loading` interceptors **conservando** el `authInterceptor` con refresh de MercadoMAX | S | 🟠 | MERGE #12,#13 |
| Externalizar a Transloco los literales de las piezas portadas | S | 🟢 | KEEP #17 |
| **Opción B (decidida):** reconstruir esqueleto de `generic-table/form/modal` con CSS propio reusando interfaces `TableColumn`/`FormFieldConfig`/`ActionConfig`. **No instalar Angular Material.** | L | 🟠 | REPLACE #4-6 |

- **Impacto:** ⭐⭐⭐ (habilita todo lo demás; el backend ya gana middleware de errores + logging que hoy no existe).
- **Dependencias:** ninguna previa. Bloquea a todas las fases siguientes.
- **Criterio de salida:** un servicio piloto (Maestros) arranca con los middlewares/JWT del template sin romper endpoints existentes; el frontend compila con la capa no-visual integrada y la decisión Material tomada.

---

## FASE 1 — Piloto vertical: **Product Category** (end-to-end)

**Objetivo:** validar toda la tubería de integración en un módulo real de bajo riesgo y dejar la **plantilla replicable**.

| Tarea | Esfuerzo | Riesgo |
|---|---|---|
| **Backend:** refactor de `ProductCategory` → repositorio usa `SpExecutor`; controller hereda `BaseApiController`; service lanza `NotFound/ConflictException` (Name único→409) en vez del ternario; paginación vía `PagedResult<T>` | M | 🟢 |
| **Frontend:** `ProductCategoryService` extiende `BaseCrudService` (+`toggleStatus`); componente extiende `CrudListBase`; declarar `columns`+`formFields`+`rowActions`(con `permission`)+filtros; modal por `DialogService.openForm`; toasts por `ToastService` | M | 🟢 |
| Verificación e2e: listar/paginar/buscar/crear/editar/ver/borrar/toggle + permisos + 409 en nombre duplicado | S | 🟢 |
| Documentar el patrón (checklist "cómo migrar un maestro") | S | 🟢 |

- **Impacto:** ⭐⭐ (1 módulo, pero define el molde y prueba contratos/errores/JWT/permisos).
- **Dependencias:** Fase 0 completa.
- **Riesgo global:** 🟢 — mapea 1:1 con el sample `catalogs/Categories` del template (patrón probado).
- **Criterio de salida:** Product Category funcionando con ~70% menos código que el componente original; checklist validado.

---

## FASE 2 — Migración masiva de Maestros + Catálogos

**Objetivo:** eliminar el grueso del boilerplate aplicando el molde del piloto a todos los catálogos CRUD.

**Orden:** Supplier (form rico + 1er select `UserId`) → LogisticUnit → Pavilion → ProductSize → Product → **Brand** (selects Supplier+Product, ya disponibles) → Stall → Carrier → Truck → TransportRate → Role → Permission. (**User se trata con Auth en Fase 4.**)

| Tarea | Esfuerzo | Riesgo |
|---|---|---|
| Migrar cada entidad backend (repo→SpExecutor, controller→BaseApiController, service→excepciones) | L (lote) | 🟢 |
| Migrar cada entidad frontend (service→BaseCrudService, componente→CrudListBase + config) | L (lote) | 🟢 |
| Unificar paginación cliente→servidor en estos listados (`GenericPagination`) | M | 🟠 |
| Aplicar `*appHasPermission` a botones de acción | S | 🟢 |
| Verificación por entidad (smoke test CRUD) | M | 🟢 |

- **Impacto:** ⭐⭐⭐ (aquí se materializa el ahorro: 16 listados, 19 tablas, 27 forms, 156 modales, 15 servicios → configuración declarativa).
- **Dependencias:** Fase 1 (molde). Brand depende de que Supplier+Product ya expongan `BaseCrudService` (para sus dropdowns).
- **Riesgo:** 🟢-🟠. Volumen alto, pero repetitivo y de bajo riesgo de negocio. Migrar en lotes pequeños verificando cada uno.
- **Criterio de salida:** todos los maestros/catálogos migrados; servicios CRUD copy-paste retirados.

---

## FASE 3 — Dominios transaccionales (plumbing + UI base, **conservando lógica**)

**Objetivo:** llevar Guías, Recepción, Comerciante y Finanzas a la nueva plomería sin reescribir su lógica de negocio.

**Orden por riesgo creciente:** Transporte/Settlement → Recepción → Comerciante/Inventario → Finanzas → **Guías** (wizard, lo más complejo).

| Tarea | Esfuerzo | Riesgo |
|---|---|---|
| Backend: repos→SpExecutor, controllers→BaseApiController, excepciones; **preservar SPs y multi-resultset** (`SP_READ_GUIDE` cabecera+detalle, listados paginados) | L | 🟠 |
| Frontend: adoptar tabla/`BaseCrudService`/modales **donde aplique**, conservando wizards y máquinas de estado (`guide-list`, `sale-list`, `account-payable-list`, `settlement-list`) | L | 🟠 |
| Mantener componentes/flows específicos (KEEP/DEFER #21): no forzar el molde de maestro donde hay lógica propia | M | 🟠 |
| Verificación e2e de flujos críticos (guía→recepción→inventario→venta→pago) | M | 🔴 |

- **Impacto:** ⭐⭐ (menos boilerplate eliminable que en maestros, pero homogeneiza errores/JWT/respuestas en todo el sistema).
- **Dependencias:** Fase 2 (BaseCrudService y tabla maduros).
- **Riesgo:** 🟠-🔴. Tocan dinero y stock. No alterar la semántica de los SPs ni el orden de resultsets. Verificación reforzada.
- **Criterio de salida:** transaccionales sobre la nueva plomería; wizards intactos; flujos críticos verificados.

---

## FASE 4 — Auth + endurecimiento + transversales avanzados

**Objetivo:** cerrar lo más frágil y sumar capacidades nuevas del template.

| Tarea | Esfuerzo | Riesgo |
|---|---|---|
| Migrar **Auth.API** (login/refresh/`tokenVersion`) a la plomería del template **conservando BCrypt** (NO migrar a PBKDF2 sin plan de re-hash); migrar `User`/`Role`/`Permission` UI | L | 🔴 |
| Normalizar JWT en todos los servicios; **corregir divergencia de Finanzas** (sección `Jwt`→`JwtSettings`, `ClockSkew=Zero`) | M | 🟠 |
| Propagar validación de `tokenVersion` (revocación) a todos los servicios o moverla al Gateway | M | 🟠 |
| (Opcional) Validación JWT en el Gateway Ocelot | M | 🟠 |
| Implementar `IAuditService` con SP de inserción → **cubre el hueco de bitácora** detectado en la auditoría de BD | M | 🟢 |
| Frontend: **lazy loading** (`loadComponent` por feature); retirar utilidades muertas (`msg()`, `confirm`, overlays); integrar `ExportService` con la tabla | M | 🟢 |
| (Opcional) Adoptar `Utilities/` del template (Export Excel/PDF, FileStorage) si el negocio lo pide | M | 🟢 |

- **Impacto:** ⭐⭐ (seguridad consistente, auditoría real, arranque optimizado).
- **Dependencias:** Fases 1-3 (Auth se migra cuando el patrón está consolidado y probado).
- **Riesgo:** 🔴 en Auth. Migrar con pruebas e2e de login/refresh/revocación; rollback listo.
- **Criterio de salida:** Auth migrado y verificado; JWT homogéneo; bitácora persistente; lazy loading activo.

---

## Mapa de dependencias entre fases

```
Fase 0 (cimientos + decisiones)
   └─> Fase 1 (piloto Product Category) ── define molde
          └─> Fase 2 (maestros masivo)
                 └─> Fase 3 (transaccionales)
                        └─> Fase 4 (Auth + endurecimiento)
```
Las fases son secuenciales en su *gate* de salida, pero dentro de la Fase 2 las entidades se paralelizan por lotes.

---

## Tabla resumen de fases

| Fase | Foco | Esfuerzo | Riesgo | Impacto | Dependencias |
|---|---|---|---|---|---|
| **0** | Cimientos + decisiones (Material, paginación, permisos, SpExecutor, middlewares) | **L** | 🟠 | ⭐⭐⭐ | — |
| **1** | Piloto Product Category end-to-end | **M** | 🟢 | ⭐⭐ | Fase 0 |
| **2** | Maestros + catálogos masivo (16 entidades) | **XL** | 🟢-🟠 | ⭐⭐⭐ | Fase 1 |
| **3** | Transaccionales (Guías/Recepción/Comerciante/Finanzas) | **XL** | 🟠-🔴 | ⭐⭐ | Fase 2 |
| **4** | Auth + endurecimiento + auditoría + lazy loading | **L** | 🔴 (Auth) | ⭐⭐ | Fases 1-3 |

---

## Riesgos transversales y mitigaciones (consolidado)

| Riesgo | Severidad | Mitigación |
|---|---|---|
| Contrato de paginación divergente (Meta vs plano) a medias | 🔴 | Decidir en Fase 0; aplicar coordinado back+front; no mezclar |
| Material vs sin-UI-library | 🔴 | Decisión §D.1 en Fase 0; recomendación Opción B |
| Tocar Auth/JWT rompe a todos | 🔴 | Auth en Fase 4, con pruebas e2e y rollback |
| Alterar SPs / orden de resultsets en transaccionales | 🔴 | Preservar SPs intactos; solo cambia la invocación |
| Convención de permisos mezclada deja endpoints abiertos | 🟠 | Una convención (Fase 0); migrar seeds/claims/atributos juntos |
| BCrypt(MMX) vs PBKDF2(template) | 🟠 | No migrar hashing; mantener BCrypt |
| Divergencia JWT de Finanzas | 🟠 | Normalizar en Fase 4 con verificación |
| Salto Angular 19→21 | 🟡 | Código ya standalone/signals/functional; cambios menores |
| Expectativa de "copiar BaseRepository" del template | 🟡 | No existe; el `SpExecutor` es construcción propia (Fase 0) |

---

## Ahorro esperado (recordatorio del análisis)
- **Backend:** ~60-70% menos boilerplate de plomería + se gana middleware de errores y logging hoy inexistentes.
- **Frontend (maestros):** ~70-80% menos código (16 list + 19 tablas + 27 forms + 156 modales + 15 servicios → config declarativa).
- El mayor valor a largo plazo es **mantenibilidad**: un cambio de patrón pasa de tocar decenas de archivos a uno.

---

## Qué NO se toca en toda la migración (KEEP duro)
- Los **153 Stored Procedures** y su lógica de negocio.
- El **Gateway Ocelot** y la topología de puertos (5000-5007) / `ocelot.json`.
- **Transloco** (i18n) en el frontend.
- La identidad visual de MercadoMAX (bajo Opción B).
- El **`authInterceptor` con refresh-token** de MercadoMAX (se conserva su lógica).
- Los **componentes/flows transaccionales** con lógica propia (wizard de guías, settlement, ventas) — se adaptan, no se reescriben.
