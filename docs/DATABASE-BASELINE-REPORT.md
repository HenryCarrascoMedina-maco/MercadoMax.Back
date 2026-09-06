# DATABASE-BASELINE-REPORT.md
## Fotografía del estado de `MercadoMAX_DB` (antes de la Fase 0)

> **Solo lectura.** No se ejecutó ningún `ALTER/CREATE/DROP/UPDATE/DELETE/INSERT`.
> **Fecha de captura:** 2026-06-07
> **Servidor:** `(localdb)\MSSQLLocalDB` (instancia `PELMLP056\LOCALDB#...`)
> **Herramienta usada:** `sqlcmd` 15.0.1300.359 (Windows auth / `-E`). `Invoke-Sqlcmd` no está disponible en la máquina.

---

## 0. Conectividad verificada
| Check | Resultado |
|---|---|
| `sqlcmd` instalado | ✅ `C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE` |
| `Invoke-Sqlcmd` | ❌ No disponible (no está el módulo `SqlServer`) |
| Instancia detectada | ✅ LocalDB `MSSQLLocalDB` (proceso `sqlservr` activo) |
| Conexión a `MercadoMAX_DB` | ✅ Correcta (Windows auth) |

---

## 1. Versión de SQL Server
```
Microsoft SQL Server 2025 (RTM-CU3) (KB5077896) - 17.0.4025.3 (X64)
Express Edition (64-bit) on Windows 10 Pro
```
> Nota: la auditoría documental previa estimó "SQL Server 2019/2022"; el motor real es **SQL Server 2025 (17.0)**, compatibility level **170**.

## 2. Tamaño de la BD
| Archivo | Tipo | Tamaño | Ruta |
|---|---|---|---|
| MercadoMAX_DB | ROWS (datos) | 72.00 MB | `C:\Users\HenryCarrascoMedina\MercadoMAX_DB.mdf` |
| MercadoMAX_DB_log | LOG | 8.00 MB | `C:\Users\HenryCarrascoMedina\MercadoMAX_DB_log.ldf` |

- **Reservado:** 9.75 MB · **Usado (datos+índices):** 5.73 MB.
- El archivo de datos (72 MB) está mayormente preasignado; el uso real es ~6 MB (BD pequeña, poca data transaccional).
- ⚠️ Los archivos viven en `C:\Users\...` (ruta de perfil) — punto de la auditoría a corregir en fases posteriores (no en Fase 0).

## 3. Número de tablas
**32 tablas** de usuario.
> La auditoría documental contó 27; el número real es **32** (ver §7 para el desglose con filas).

## 4. Número de vistas
**0 vistas.**

## 5. Número de stored procedures
**153 stored procedures.** (Coincide con la auditoría.)

## 6. Número de funciones
**0 funciones** (escalares/inline/tabla). Confirma el hallazgo "cero UDFs".

## 7. Esquemas existentes
7 esquemas de usuario: **`auth`, `finance`, `guide`, `master`, `merchant`, `reception`, `transport`**.

## 8. Cantidad de registros por tabla
| Tabla | Filas | | Tabla | Filas |
|---|---|---|---|---|
| auth.PasswordResetToken | 1 | | master.Brand | 2 |
| auth.Permission | 40 | | master.LogisticUnit | 6 |
| auth.Role | 6 | | master.Pavilion | 4 |
| auth.RolePermission | 82 | | master.Product | 14 |
| auth.User | 7 | | master.ProductCategory | **5** |
| auth.UserPermission | 40 | | master.ProductSize | 9 |
| auth.UserRole | 7 | | master.Stall | 5 |
| finance.AccountPayable | 0 | | master.Supplier | 1 |
| finance.Payment | 0 | | merchant.Inventory | 0 |
| finance.Sale | 0 | | merchant.InventoryMovement | 0 |
| finance.SaleDetail | 0 | | merchant.ReceptionConfirmation | 0 |
| guide.Guide | 0 | | reception.Reception | 0 |
| guide.GuideDetail | 0 | | reception.ReceptionDetail | 0 |
| transport.Carrier | 0 | | reception.Shortage | 0 |
| transport.SettlementDetail | 0 | | transport.TransportRate | 0 |
| transport.TransportSettlement | 0 | | transport.Truck | 0 |

**Lectura:** datos semilla en `auth` y `master`; **todo el flujo transaccional está vacío** (guías, recepción, inventario, ventas, pagos, transporte = 0). `master.ProductCategory` tiene **5 filas** → el piloto de Fase 1 tendrá datos reales para probar.

## 9. Foreign Keys existentes
**58 FKs** (todas con esquema). Desglose por tabla padre:
- **auth (7):** PasswordResetToken→User; RolePermission→Permission/Role; UserPermission→Permission/User; UserRole→Role/User.
- **finance (12):** AccountPayable→Guide/Stall/Supplier; Payment→AccountPayable/User; Sale→Stall/User; SaleDetail→Brand/LogisticUnit/Product/ProductSize/Sale.
- **guide (9):** Guide→CreatedBy(User)/Supplier/**Truck**; GuideDetail→Brand/Guide/LogisticUnit/Product/ProductSize/Stall.
- **master (7):** Brand→Product/Supplier; Product→Category; ProductSize→Product; Stall→Pavilion/User; Supplier→User.
- **merchant (10):** Inventory→Brand/LogisticUnit/Product/ProductSize/Stall; InventoryMovement→Inventory/User; ReceptionConfirmation→ReceptionDetail/Stall.
- **reception (5):** Reception→Guide/Manager(User); ReceptionDetail→GuideDetail/Reception; Shortage→ReceptionDetail.
- **transport (8):** Carrier→User; SettlementDetail→Guide/LogisticUnit/Settlement; TransportRate→Carrier/LogisticUnit; TransportSettlement→Carrier/Truck; Truck→Carrier.

🔴 **Confirmado el hallazgo de la auditoría:** `guide.Guide` tiene `FK_Guide_Truck` pero **NO** tiene `FK_Guide_Carrier` → es la FK que crea el script `02`.

## 10. Índices existentes
- **Total: 101 índices** (index_id>0): **32 PK**, **16 únicos**, **53 no-clustered no únicos**.
- Desglose relevante por tabla (NC = no-clustered no únicos):

| Tabla | PK | Únicos | NC | Observación |
|---|---|---|---|---|
| finance.SaleDetail | 1 | 0 | **1** | solo `SaleId`; faltan Product/Brand/Size/Unit |
| guide.GuideDetail | 1 | 0 | **3** | Guide/DestinationStall/Product; faltan Brand/Size/Unit |
| merchant.Inventory | 1 | 0 | **2** | Stall/Product; faltan Brand/Size/Unit + único de SKU |
| transport.SettlementDetail | 1 | 0 | **2** | Settlement/Guide; falta LogisticUnit |
| guide.Guide | 1 | 1 | 6 | bien cubierta |

(El resto de tablas tiene PK + sus índices de FK/filtro habituales.)

## 11. AUTO_CLOSE actual
**`AUTO_CLOSE = ON`** (`is_auto_close_on = 1`). → lo cambia el script `01` a OFF.

## 12. READ_COMMITTED_SNAPSHOT actual
**`READ_COMMITTED_SNAPSHOT = OFF`** (`is_read_committed_snapshot_on = 0`). Snapshot isolation: OFF. Recovery model: **SIMPLE**. Collation: `SQL_Latin1_General_CP1_CI_AS`. → RCSI lo activa el script `01`.

## 13. Posibles huérfanos
**0 huérfanos** en `guide.Guide.CarrierId` (guías con transportista inexistente).
✅ El script `02` podrá crear `FK_Guide_Carrier` **sin abortar**.

## 14. Posibles duplicados para Inventory
**0 grupos duplicados** en `merchant.Inventory(StallId, ProductId, BrandId, ProductSizeId, LogisticUnitId)`. (Además la tabla está vacía.)
✅ El script `04` podrá crear `UX_Inventory_Sku` **sin abortar**.

## 15. Índices faltantes
- **Sugerencias del motor (DMV `sys.dm_db_missing_index_*`):** ninguna — esperable, la BD tiene poca/nula carga de trabajo registrada.
- **Faltantes según el plan de Fase 0 (verificado: NINGUNO existe aún):**
  `IX_SaleDetail_ProductId/BrandId/ProductSizeId/LogisticUnitId`, `IX_GuideDetail_BrandId/ProductSizeId/LogisticUnitId`, `IX_SettlementDetail_LogisticUnitId`, `IX_Inventory_BrandId/ProductSizeId/LogisticUnitId`, y el único `UX_Inventory_Sku`. → los crean los scripts `03` y `04`.

## 16. Estadísticas relevantes
| Métrica | Valor |
|---|---|
| Tablas | 32 |
| Vistas | 0 |
| Stored procedures | 153 |
| Funciones | 0 |
| Esquemas de usuario | 7 |
| Foreign keys | 58 |
| Índices (total) | 101 (32 PK / 16 únicos / 53 NC) |
| Tamaño datos / log | 72 MB / 8 MB (usado ~5.73 MB) |
| Filas totales (todas las tablas) | ~250 (solo semillas; transaccional vacío) |

---

## Veredicto de preparación para Fase 0
| Script | Pre-condición | Estado real | ¿Aplicará limpio? |
|---|---|---|---|
| `01` AUTO_CLOSE / RCSI | — | AUTO_CLOSE=ON, RCSI=OFF | ✅ Sí (cambia ambos) |
| `02` FK_Guide_Carrier | sin huérfanos | 0 huérfanos | ✅ Sí (crea la FK) |
| `03` índices FK | no existen | 0 de 8 existen | ✅ Sí (crea los 8) |
| `04` UX_Inventory_Sku | sin duplicados | 0 duplicados (tabla vacía) | ✅ Sí (crea el único) |

**Conclusión:** la base está **lista y es segura** para ejecutar la Fase 0 completa; no se anticipan abortos por pre-check ni conflictos de datos. Como es LocalDB de un solo usuario, el paso de RCSI del script `01` debería aplicarse sin contención (igualmente, conviene cerrar otras conexiones / detener los microservicios, según la guía `EXECUTE-FASE0-SQL-GUIDE.md`).

> Recordatorio: este reporte **no modificó nada**. La ejecución de los scripts sigue siendo manual desde SSMS, a tu cargo.
