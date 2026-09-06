# POST-FASE0-VALIDATION.md
## Evidencia real de la ejecución de la Fase 0

> **Ejecutado por:** Claude (vía `sqlcmd` contra la BD operativa).
> **Servidor:** `(localdb)\MSSQLLocalDB` · **Base:** `MercadoMAX_DB` (la misma que usan los 7 microservicios).
> **Fecha:** 2026-06-07 · **Login:** `AzureAD\HenryCarrascoMedina` (Windows auth).
> **Resultado global:** ✅ **FASE 0 APLICADA CORRECTAMENTE** (4/4 scripts OK, 5/5 verificaciones OK, smoke test de BD OK).

---

## 0. Backup de seguridad (antes de cualquier cambio)
```
BACKUP DATABASE successfully processed 1290 pages in 0.031 seconds.
BACKUP_EXIT = 0
RESTORE VERIFYONLY → "The backup set on file 1 is valid."
VERIFY_EXIT = 0
```
- Archivo: `C:\Backups\MercadoMAX_DB_PreFase0.bak` (10,670,080 bytes) · creado 2026-06-07 15:42.
- ✅ Backup válido y verificado **antes** de ejecutar los scripts. Punto de restauración disponible.

---

## 1. Ejecución de scripts (en orden, parando ante cualquier fallo)

### Script 01 — `01_ServerOptions_AutoClose_RCSI.sql`
Pre-check de conexiones activas: **0** (aparte de la sesión de ejecución → RCSI sin contención).
```
Changed database context to 'master'.
AUTO_CLOSE establecido en OFF.
READ_COMMITTED_SNAPSHOT establecido en ON.
SCRIPT01_EXIT = 0
```
Verificación: `is_auto_close_on = 0` · `is_read_committed_snapshot_on = 1` ✅

### Script 02 — `02_ForeignKeys_Missing.sql`
```
Changed database context to 'MercadoMAX_DB'.
FK_Guide_Carrier creada.
SCRIPT02_EXIT = 0
```
Verificación: `FK_Guide_Carrier` → `is_disabled = 0`, `is_not_trusted = 0` (creada **WITH CHECK**, validó los datos existentes) ✅

### Script 03 — `03_Indexes_Missing.sql`
```
Changed database context to 'MercadoMAX_DB'.
Indices de Fase 0 verificados/creados.
SCRIPT03_EXIT = 0
```
Verificación: 8 índices FK + 3 de Inventory = **11 índices nuevos** (ver §2.3) ✅

### Script 04 — `04_Constraints_Unique_Inventory.sql`
```
Changed database context to 'MercadoMAX_DB'.
Indice unico UX_Inventory_Sku creado.
SCRIPT04_EXIT = 0
```
Verificación: `UX_Inventory_Sku` → `is_unique = 1`, `type_desc = NONCLUSTERED` ✅

> Ningún script abortó por pre-check (0 huérfanos, 0 duplicados) ni devolvió error (`EXIT=0` en los 4).

---

## 2. Auditoría final de solo lectura (evidencia)

### 2.1 Opciones de servidor
| Propiedad | Valor | Esperado |
|---|---|---|
| `is_auto_close_on` | **0** (OFF) | 0 ✅ |
| `is_read_committed_snapshot_on` | **1** (ON) | 1 ✅ |
| `snapshot_isolation_state_desc` | OFF | OFF (no se tocó) ✅ |

### 2.2 FK_Guide_Carrier
| name | is_not_trusted |
|---|---|
| FK_Guide_Carrier | 0 ✅ (trusted) |

### 2.3 Índices de Fase 0 creados (11)
```
IX_GuideDetail_BrandId
IX_GuideDetail_LogisticUnitId
IX_GuideDetail_ProductSizeId
IX_Inventory_BrandId
IX_Inventory_LogisticUnitId
IX_Inventory_ProductSizeId
IX_SaleDetail_BrandId
IX_SaleDetail_LogisticUnitId
IX_SaleDetail_ProductId
IX_SaleDetail_ProductSizeId
IX_SettlementDetail_LogisticUnitId
```
✅ 11 de 11.

### 2.4 UX_Inventory_Sku
| name | is_unique |
|---|---|
| UX_Inventory_Sku | 1 ✅ |

### 2.5 Conteos antes vs después
| Métrica | Baseline (pre) | Post Fase 0 | Δ |
|---|---|---|---|
| Foreign keys (total) | 58 | **59** | +1 (FK_Guide_Carrier) |
| Índices (total, index_id>0) | 101 | **113** | +12 (11 índices + 1 único) |

---

## 3. Smoke test (nivel base de datos)
| Check | Resultado |
|---|---|
| Conexión a `MercadoMAX_DB` | ✅ `CONN_OK` (login `AzureAD\HenryCarrascoMedina`, `DB_NAME()=MercadoMAX_DB`) |
| Lectura `master.ProductCategory` | ✅ **5 filas** (datos semilla intactos) |
| SPs de ProductCategory presentes | ✅ `SP_CREATE/READ/LIST/UPDATE/DELETE/TOGGLE_STATUS_PRODUCT_CATEGORY` |

> **Alcance del smoke test:** se validó a nivel **BD** (conexión, lectura, SPs). El smoke test a nivel **aplicación** (levantar los 7 microservicios + login HTTP + listado vía API) **no se ejecutó** porque los microservicios no estaban corriendo (0 sesiones de app detectadas). Recomendado hacerlo al iniciar la Fase 1, pero no es bloqueante: los cambios son aditivos/estructurales y no alteran datos ni contratos.

---

## 4. Conclusión
✅ **La Fase 0 quedó aplicada y verificada correctamente en la BD operativa.**
- 4/4 scripts ejecutados con `EXIT=0`.
- 5/5 verificaciones correctas (AUTO_CLOSE OFF, RCSI ON, FK creada y trusted, 11 índices, índice único).
- Datos intactos (sin INSERT/UPDATE/DELETE; ProductCategory sigue con 5 filas).
- Backup verificado disponible para rollback (`C:\Backups\MercadoMAX_DB_PreFase0.bak`) y rollback estructural en `ScriptBD/Fase0/99_Rollback_Fase0.sql`.

**No se avanzó a Fase 1.** Pendiente: tu confirmación para generar `FASE1-PLAN-PRODUCTCATEGORY.md` (plan técnico, sin tocar código todavía).
