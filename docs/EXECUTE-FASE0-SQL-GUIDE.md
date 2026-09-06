# EXECUTE-FASE0-SQL-GUIDE.md
## Guía de ejecución manual (SSMS) — Scripts de BD de la Fase 0

> Ejecución **uno por uno** desde SSMS, revisando mensajes y pre-checks en cada paso.
> Base de datos: **`MercadoMAX_DB`**. Carpeta de scripts: `ScriptBD/Fase0/`.
> Ejecutar cada script **completo** (F5) en una ventana conectada a la instancia correcta.
> Revisar siempre la pestaña **Mensajes** (no solo **Resultados**).

---

## 1. Backup recomendado (antes de empezar)
En una ventana nueva, ajustando la ruta de salida:
```sql
BACKUP DATABASE [MercadoMAX_DB]
TO DISK = N'C:\Backups\MercadoMAX_DB_PreFase0.bak'
WITH INIT, COMPRESSION, STATS = 10,
     NAME = N'MercadoMAX_DB - Pre Fase 0';
```
Confirmar que termina con `BACKUP DATABASE ... processed ... successfully`.

Luego **validar el archivo `.bak`** antes de confiar en él:
```sql
RESTORE VERIFYONLY
FROM DISK = N'C:\Backups\MercadoMAX_DB_PreFase0.bak';
```
Debe devolver `The backup set on file 1 is valid.`. **No continúes sin un backup válido y verificado.**

---

## 2. Validar conexiones activas (clave para el script 01 / RCSI)
`READ_COMMITTED_SNAPSHOT ON` necesita acceso exclusivo momentáneo. Antes de ejecutar el `01`, revisa quién está conectado:
```sql
SELECT s.session_id, s.login_name, s.host_name, s.program_name, s.status
FROM sys.dm_exec_sessions s
WHERE s.database_id = DB_ID(N'MercadoMAX_DB')
  AND s.session_id <> @@SPID;
```
- **Lo ideal:** 0 filas (nadie más conectado).
- Si hay filas: cierra esas conexiones (apps, otras ventanas de SSMS, los 7 microservicios **detenidos**). El script `01` usa `WITH ROLLBACK AFTER 5 SECONDS`, así que **forzará** el cambio cerrando transacciones pendientes a los 5 s; aun así conviene tener la BD lo más libre posible.
- **Recomendación:** ejecuta el `01` en **ventana de mantenimiento**, con los microservicios MercadoMAX apagados.

### 2.1 (Opcional / emergencia) SINGLE_USER → MULTI_USER si el `01` no puede aplicar RCSI
Úsalo **solo** si, pese a apagar los microservicios y cerrar otras ventanas, el script `01` sigue sin poder activar `READ_COMMITTED_SNAPSHOT` por conexiones activas que no logras cerrar. Fuerza acceso exclusivo, aplica el cambio y **devuelve la BD a multiusuario en el mismo bloque**:
```sql
USE [master];
GO
-- 1) Acceso exclusivo (cierra el resto de conexiones de inmediato)
ALTER DATABASE [MercadoMAX_DB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
-- 2) Aplicar el cambio en modo exclusivo
ALTER DATABASE [MercadoMAX_DB] SET READ_COMMITTED_SNAPSHOT ON;
GO
-- 3) IMPORTANTE: volver SIEMPRE a multiusuario
ALTER DATABASE [MercadoMAX_DB] SET MULTI_USER;
GO
```
⚠️ **Advertencias:**
- `SINGLE_USER` desconecta a todos los demás; ejecútalo solo en ventana de mantenimiento.
- **Nunca dejes la BD en `SINGLE_USER`**: ejecuta los tres pasos seguidos. Si la sesión se corta entre el paso 1 y el 3, vuelve a conectar y ejecuta `ALTER DATABASE [MercadoMAX_DB] SET MULTI_USER;`.
- Mantén una **única** ventana de SSMS conectada al hacer esto (si abres otra, podría "robar" la sesión single-user).
- Este bloque **reemplaza** al `01` solo para RCSI; el `AUTO_CLOSE OFF` puedes aplicarlo igual con el `01` (no requiere exclusividad).

---

## 3. Orden exacto de ejecución
Ejecutar **en este orden**, uno por uno, validando cada uno antes de pasar al siguiente:

| Paso | Script | Notas |
|---|---|---|
| 1 | `01_ServerOptions_AutoClose_RCSI.sql` | Ventana de mantenimiento. Revisar §2 antes. |
| 2 | `02_ForeignKeys_Missing.sql` | Tiene pre-check de huérfanos. |
| 3 | `03_Indexes_Missing.sql` | Solo crea índices. |
| 4 | `04_Constraints_Unique_Inventory.sql` | Tiene pre-check de duplicados. |

> El `99_Rollback_Fase0.sql` **NO** se ejecuta en condiciones normales (solo si algo falla, ver §9).

---

## 4. Qué revisar después de cada script

**Script 01 (opciones de servidor):**
- Mensajes esperados: `AUTO_CLOSE establecido en OFF.` y `READ_COMMITTED_SNAPSHOT establecido en ON.` (o "ya estaba..." si se re-ejecuta).
- Verificación inmediata:
```sql
SELECT name, is_auto_close_on, is_read_committed_snapshot_on
FROM sys.databases WHERE name = N'MercadoMAX_DB';
-- Esperado: is_auto_close_on = 0, is_read_committed_snapshot_on = 1
```

**Script 02 (FK_Guide_Carrier):**
- Mensaje esperado (caso bueno): `FK_Guide_Carrier creada.`
- Si aparece `ABORTADO: existen guias con CarrierId huerfano.` → revisar la grilla de resultados con los `GuideId` afectados (ver §7).

**Script 03 (índices):**
- Mensaje esperado: `Indices de Fase 0 verificados/creados.` (sin errores).

**Script 04 (índice único Inventory):**
- Mensaje esperado (caso bueno): `Indice unico UX_Inventory_Sku creado.`
- Si aparece `ABORTADO: existen combinaciones duplicadas...` → revisar la grilla con los duplicados (ver §7).

---

## 5. Mensajes NORMALES (todo bien)
- `... processed ... successfully` (backup).
- `AUTO_CLOSE establecido en OFF.` / `... ya estaba OFF. Sin cambios.`
- `READ_COMMITTED_SNAPSHOT establecido en ON.` / `... ya estaba ON. Sin cambios.`
- `FK_Guide_Carrier creada.` / `FK_Guide_Carrier ya existe. Sin cambios.`
- `Indices de Fase 0 verificados/creados.`
- `Indice unico UX_Inventory_Sku creado.` / `UX_Inventory_Sku ya existe. Sin cambios.`
- `Comandos completados correctamente.`
- Re-ejecutar cualquier script y ver “ya existe / Sin cambios” es **normal** (son idempotentes).

---

## 6. Mensajes de ALERTA (detenerse y revisar)
- `ABORTADO: existen guias con CarrierId huerfano.` → datos huérfanos (script 02). **No** crea la FK. Ver §7.
- `ABORTADO: existen combinaciones duplicadas en merchant.Inventory.` → duplicados (script 04). **No** crea el índice. Ver §7.
- Cualquier `Msg ... Level 16` (error real de T-SQL), p. ej.:
  - error de permisos (`ALTER DATABASE permission denied`) → ejecutar con login con permisos adecuados.
  - timeout/bloqueo en el `01` → quedan conexiones activas; revisar §2 y reintentar en ventana limpia.
- Que el script 01 **quede colgado** más de unos segundos → hay conexiones reteniendo la BD; el `ROLLBACK AFTER 5 SECONDS` debería resolverlo, pero si no, cancelar (Stop) y limpiar conexiones (§2).

---

## 7. Qué hacer si un script detecta huérfanos o duplicados

**Huérfanos (script 02):** el script lista las guías con `CarrierId` inexistente. Opciones de corrección (elige según el negocio), por ejemplo:
```sql
-- Inspeccionar
SELECT g.Id, g.GuideNumber, g.CarrierId
FROM [guide].[Guide] g
WHERE g.CarrierId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM [transport].[Carrier] c WHERE c.Id = g.CarrierId);

-- Opción A: poner el transportista en NULL (la columna es nullable)
-- UPDATE [guide].[Guide] SET CarrierId = NULL WHERE Id IN (<ids>);

-- Opción B: corregir el CarrierId al transportista correcto
-- UPDATE [guide].[Guide] SET CarrierId = <idCorrecto> WHERE Id = <id>;
```
Tras corregir, **re-ejecutar el `02`** → debe terminar con `FK_Guide_Carrier creada.`

**Duplicados (script 04):** el script lista las combinaciones `(StallId, ProductId, BrandId, ProductSizeId, LogisticUnitId)` repetidas. Hay que **consolidar** (mantener un registro de inventario por SKU/puesto y sumar/migrar stock), por ejemplo:
```sql
-- Inspeccionar duplicados
SELECT StallId, ProductId, BrandId, ProductSizeId, LogisticUnitId, COUNT(*) AS Duplicados
FROM [merchant].[Inventory]
GROUP BY StallId, ProductId, BrandId, ProductSizeId, LogisticUnitId
HAVING COUNT(*) > 1;
```
Consolidar con cuidado (revisar `InventoryMovement` que referencia esos `InventoryId`). Tras consolidar, **re-ejecutar el `04`**.

> Si no quieres tocar datos ahora, puedes **omitir** el script que aborta (02 o 04) y continuar con el resto; queda pendiente para cuando se limpien los datos. Los demás scripts son independientes.

---

## 8. Validar que los cambios quedaron aplicados (todo junto)
Ejecutar al final para confirmar el estado:
```sql
USE [MercadoMAX_DB];

-- Opciones de servidor
SELECT is_auto_close_on, is_read_committed_snapshot_on
FROM sys.databases WHERE name = N'MercadoMAX_DB';
-- Esperado: 0 , 1

-- FK
SELECT name FROM sys.foreign_keys WHERE name = N'FK_Guide_Carrier';
-- Esperado: 1 fila

-- Índices nuevos (esperado: 8 filas)
SELECT name FROM sys.indexes
WHERE name IN (
 'IX_SaleDetail_ProductId','IX_SaleDetail_BrandId','IX_SaleDetail_ProductSizeId','IX_SaleDetail_LogisticUnitId',
 'IX_GuideDetail_BrandId','IX_GuideDetail_ProductSizeId','IX_GuideDetail_LogisticUnitId',
 'IX_SettlementDetail_LogisticUnitId'
);

-- Índices Inventory (esperado: hasta 3) + único
SELECT name, is_unique FROM sys.indexes
WHERE object_id = OBJECT_ID(N'[merchant].[Inventory]')
  AND name IN ('IX_Inventory_BrandId','IX_Inventory_ProductSizeId','IX_Inventory_LogisticUnitId','UX_Inventory_Sku');
-- UX_Inventory_Sku debe tener is_unique = 1
```

---

## 9. Cómo ejecutar rollback (solo si fuera necesario)
- Para revertir **estructuras** (FK + índices + único): ejecutar `99_Rollback_Fase0.sql`. Es idempotente y seguro; no toca filas.
- Para revertir **opciones de servidor** (AUTO_CLOSE / RCSI): están **comentadas** al final del `99_`. Requiere de nuevo acceso exclusivo (`ROLLBACK AFTER 5 SECONDS`).
- ⚠️ **`READ_COMMITTED_SNAPSHOT` normalmente NO debe revertirse.** Es una mejora de concurrencia estándar y desactivarla puede **reintroducir bloqueos** entre lecturas y escrituras. Revertir RCSI solo está justificado ante una **razón técnica comprobada** (p. ej. un incremento medido de `tempdb`/version store que afecte la operación, o un comportamiento de aislamiento incompatible **demostrado** con datos). No lo desactives "por las dudas": si tienes la sospecha pero no la evidencia, **déjalo en ON** y diagnostica primero. `AUTO_CLOSE OFF` tampoco debería revertirse (es siempre lo recomendado en servidor).
- Ante cualquier resultado inesperado grave: **restaurar el backup** del paso §1.

---

## 10. Checklist final antes de iniciar Fase 1
- [ ] Backup `MercadoMAX_DB_PreFase0.bak` creado y verificado.
- [ ] `01` ejecutado: `is_auto_close_on = 0` y `is_read_committed_snapshot_on = 1`.
- [ ] `02` ejecutado: `FK_Guide_Carrier` existe (sin huérfanos pendientes), o decisión consciente de diferirlo con datos limpiados después.
- [ ] `03` ejecutado: los 8 índices nuevos existen.
- [ ] `04` ejecutado: `UX_Inventory_Sku` existe con `is_unique = 1`, o diferido por duplicados a consolidar.
- [ ] Ningún `Msg ... Level 16` sin resolver en la pestaña Mensajes.
- [ ] Microservicios MercadoMAX vuelven a arrancar y responden (smoke test de login + un listado).
- [ ] Sin bloqueos activos:
```sql
SELECT blocking_session_id, session_id, wait_type, wait_time
FROM sys.dm_exec_requests WHERE blocking_session_id <> 0;
-- Esperado: 0 filas
```
- [ ] Confirmado el estado con las consultas del §8.

> Con el checklist completo, la BD queda lista para la **Fase 1 — piloto Product Category**.
