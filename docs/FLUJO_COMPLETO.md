# FLUJO COMPLETO — MercadoMAX

Proceso end-to-end desde que el proveedor registra su carga hasta que el comerciante vende al cliente final.

---

## Actores del sistema

| Usuario | Rol | Correo |
|---|---|---|
| Sistema Administrador | `Admin` | admin@mercadomax.com |
| Juan Proveedor | `Supplier` | proveedor@mercadomax.com |
| Carlos Transportista | `Carrier` | transportista@mercadomax.com |
| Ana Recepcion | `ReceptionManager` | recepcion@mercadomax.com |
| Pedro Estibador | `Stevedore` | estibador@mercadomax.com |
| Maria Comerciante | `Merchant` | comerciante@mercadomax.com |

---

## Diagrama del flujo

```
[Supplier]          [Admin]            [Carrier]       [ReceptionManager]   [Stevedore]     [Merchant]
    │                  │                   │                   │                 │               │
    │ 1. Crea Guía     │                   │                   │                 │               │
    │──────────────────│                   │                   │                 │               │
    │                  │ 2. Config masters │                   │                 │               │
    │                  │ (productos,       │                   │                 │               │
    │                  │  pabellones,      │                   │                 │               │
    │                  │  puestos)         │                   │                 │               │
    │                  │                   │                   │                 │               │
    │                  │ 3. Asigna carrier │                   │                 │               │
    │──────────────────│───────────────────│                   │                 │               │
    │                  │                   │ 4. Registra camión│                 │               │
    │                  │                   │ y tarifa          │                 │               │
    │                  │                   │                   │                 │               │
    │                  │                   │ 5. Camión en ruta │                 │               │
    │                  │                   │───────────────────│                 │               │
    │                  │                   │                   │ 6. Crea         │               │
    │                  │                   │                   │ Recepción       │               │
    │                  │                   │                   │─────────────────│               │
    │                  │                   │                   │                 │ 7. Descarga   │
    │                  │                   │                   │                 │ y verifica    │
    │                  │                   │                   │─────────────────│               │
    │                  │                   │                   │ 8. Registra     │               │
    │                  │                   │                   │ faltantes/daños │               │
    │                  │                   │                   │                 │               │
    │                  │                   │ 9. Liquidación    │                 │               │
    │                  │───────────────────│                   │                 │               │
    │                  │                   │                   │                 │               │
    │                  │                   │                   │                 │ 10. Confirma  │
    │                  │                   │                   │                 │ stock         │
    │                  │                   │                   │                 │───────────────│
    │                  │                   │                   │                 │               │ 11. Vende al
    │                  │                   │                   │                 │               │ cliente
    │                  │ 12. Cuentas por   │                   │                 │               │
    │                  │ cobrar/pagar      │                   │                 │               │
```

---

## PASO 1 — Configuración inicial (Admin)

**Usuario:** `admin@mercadomax.com`  
**Permisos:** `Masters:Create`, `Masters:Update`, `Masters:List`

Antes de que cualquier operación ocurra, el Admin debe configurar los datos maestros:

| Qué registra | Módulo | Descripción |
|---|---|---|
| Categorías de productos | Maestros | Clasificación de productos |
| Productos | Maestros | Productos que se transportarán |
| Marcas | Maestros | Marcas de los productos |
| Presentaciones (tamaños) | Maestros | Unidades de medida / presentación |
| Unidades logísticas | Maestros | Caja, pallet, saco, etc. |
| Pabellones | Maestros | Zonas del mercado |
| Puestos (Stalls) | Maestros | Puntos de venta dentro de cada pabellón |
| Usuarios y roles | Auth | Crear y asignar roles a cada usuario |

---

## PASO 2 — El Proveedor crea la Guía de Remisión (Supplier)

**Usuario:** `proveedor@mercadomax.com`  
**Permisos:** `Guides:Create`, `Guides:Update`, `Masters:List` (para consultar productos/puestos)

La **Guía de Remisión** es el documento central del flujo. Representa un envío de mercadería.

**Qué registra en la Guía (`guide.Guide`):**
- `SupplierId` — quién envía
- `CarrierId` — qué transportista llevará la carga
- `TruckId` — qué camión
- `ShipmentDate` — fecha de despacho
- `EstimatedArrivalDate` — fecha estimada de llegada
- `GuideStatus` — estado: `Pending` → `InTransit` → `Delivered`
- `Observations` — notas adicionales

**Detalle de la Guía (`guide.GuideDetail`) — por cada producto:**
- `DestinationStallId` — puesto destino en el mercado
- `ProductId`, `BrandId`, `ProductSizeId`, `LogisticUnitId`
- `Quantity` — cantidad enviada
- `UnitPrice` — precio unitario del producto
- `TransportUnitPrice` — costo de transporte por unidad

> El proveedor puede crear, editar y eliminar sus propias guías mientras estén en estado `Pending`.

---

## PASO 3 — El Transportista registra su camión y tarifa (Carrier)

**Usuario:** `transportista@mercadomax.com`  
**Permisos:** `Transport:Create`, `Transport:Update`, `Guides:List`, `Guides:Read`

**Qué gestiona el transportista:**

| Entidad | Descripción |
|---|---|
| `transport.Carrier` | Su perfil: nombre, DNI, licencia, teléfono |
| `transport.Truck` | Sus camiones: placa, marca, modelo, capacidad |
| `transport.TransportRate` | Tarifas por ruta/tipo de carga |

- Puede **ver las guías** que le fueron asignadas (`Guides:List`, `Guides:Read`).
- Puede **ver el estado del transporte** y sus liquidaciones.

---

## PASO 4 — El Transportista sale en ruta

**Usuario:** `transportista@mercadomax.com`

Una vez que el proveedor confirma la guía, el estado cambia a `InTransit`. El transportista:
- Carga el camión con la mercadería de la guía.
- Parte hacia el mercado en la fecha indicada.

---

## PASO 5 — Recepción en el mercado (ReceptionManager)

**Usuario:** `recepcion@mercadomax.com`  
**Permisos:** `Reception:Create`, `Reception:Update`, `Guides:List`, `Guides:Update`

Cuando el camión llega al mercado, la encargada de recepción registra la llegada:

**Cabecera de Recepción (`reception.Reception`):**
- `GuideId` — guía de remisión que llega
- `ManagerId` — quien recibe
- `ReceptionDate` — fecha de recepción real
- `ArrivalTime` — hora de llegada
- `ReceptionStatus` — `Pending` → `InProgress` → `Completed`

**Detalle de Recepción (`reception.ReceptionDetail`) — por cada línea de la guía:**
- `GuideDetailId` — producto recibido
- `ReceivedQuantity` — cantidad realmente recibida
- `ShortageQuantity` — cantidad faltante
- `DamagedQuantity` — cantidad dañada

---

## PASO 6 — Descarga física (Stevedore)

**Usuario:** `estibador@mercadomax.com`  
**Permisos:** `Reception:List`, `Reception:Read`, `Guides:List`, `Guides:Read`

El estibador es el que **físicamente descarga la mercadería** del camión:
- Puede **consultar las guías** para saber qué productos vienen y a qué puestos van.
- Puede **ver las recepciones** para saber dónde colocar cada carga.
- No crea ni modifica registros — su rol es operativo/consulta.

---

## PASO 7 — Registro de faltantes y daños (ReceptionManager)

**Usuario:** `recepcion@mercadomax.com`  
**Permisos:** `Reception:Create`, `Reception:Update`

Si durante la descarga se detectan diferencias:

**Tabla `reception.Shortage`:**
- `ReceptionDetailId` — qué línea tiene el problema
- `IncidentType` — `Shortage` (faltante) | `Damaged` (daño)
- `Quantity` — cantidad afectada
- `Description` — descripción del incidente
- `EvidenceUrl` — foto/evidencia
- `ClaimStatus` — `Pending` → `Resolved`

La recepcionista **actualiza el estado de la guía** a `Delivered` al finalizar.

---

## PASO 8 — El Comerciante confirma su mercadería (Merchant)

**Usuario:** `comerciante@mercadomax.com`  
**Permisos:** `Inventory:List`, `Inventory:Update`, `Reception:List`, `Reception:Read`

Una vez recibida la carga en el puesto:

**Confirmación de recepción (`merchant.ReceptionConfirmation`):**
- `StallId` — su puesto
- `ReceptionDetailId` — línea que confirma
- `ConfirmedQuantity` — cantidad que realmente llegó a su puesto
- `IsConforming` — ¿está conforme?

**Inventario (`merchant.Inventory`) — se actualiza automáticamente:**
- `StallId`, `ProductId`, `BrandId`, `ProductSizeId`, `LogisticUnitId`
- `CurrentStock` — stock actual disponible
- `MinimumStock` — stock mínimo de alerta

---

## PASO 9 — El Comerciante vende al cliente (Merchant)

**Usuario:** `comerciante@mercadomax.com`  
**Permisos:** `Finance:Create`, `Finance:List`, `Finance:Read`

Con el stock disponible, el comerciante registra sus ventas:

**Venta (`finance.Sale`):**
- `StallId` — puesto del comerciante
- `CustomerName` — nombre del cliente
- `SaleDate` — fecha y hora
- `TotalAmount` — monto total
- `PaymentType` — `Cash` | `Card` | `Transfer`
- `SaleStatus` — `Pending` | `Completed` | `Void`

**Detalle de Venta (`finance.SaleDetail`) — por producto vendido:**
- Producto, marca, presentación, unidad logística
- `Quantity`, `UnitPrice`

---

## PASO 10 — Liquidación del transportista (Admin / Carrier)

**Usuario:** `admin@mercadomax.com` o `transportista@mercadomax.com`  
**Permisos:** `Transport:Create`, `Transport:Update`

Una vez completada la entrega, se genera la liquidación del viaje:

**Liquidación (`transport.TransportSettlement`):**
- `CarrierId`, `TruckId`
- `TripDate` — fecha del viaje
- `TotalUnits` — total de unidades transportadas
- `TotalAmount` — monto a pagar al transportista
- `SettlementStatus` — `Pending` → `Paid`

**Detalle de liquidación (`transport.SettlementDetail`):**
- Desglosa por guía/producto el cálculo del pago.

---

## PASO 11 — Cuentas por pagar del comerciante (Merchant / Admin)

**Usuario:** `comerciante@mercadomax.com` o `admin@mercadomax.com`  
**Permisos:** `Finance:List`, `Finance:Read`, `Finance:Update`

El comerciante recibe mercadería a crédito. Se genera automáticamente una cuenta por pagar:

**Cuenta por pagar (`finance.AccountPayable`):**
- `StallId` — puesto deudor
- `SupplierId` — a quién se le debe
- `GuideId` — guía que origina la deuda
- `TotalAmount` — monto total
- `PaidAmount` — cuánto ha pagado
- `Balance` — saldo pendiente
- `DueDate` — fecha de vencimiento
- `AccountStatus` — `Pending` | `PartialPaid` | `Paid`

**Pagos (`finance.Payment`):**
- Registra cada abono parcial o total.

---

## PASO 12 — Supervisión global (Admin)

**Usuario:** `admin@mercadomax.com`  
**Permisos:** Todos los módulos

El administrador puede ver el **Dashboard** con:
- Totales de usuarios, roles, proveedores, transportistas, puestos
- Guías activas, recepciones, faltantes, liquidaciones
- Ventas totales y cuentas por cobrar/pagar

---

## Resumen del flujo por actor

```
Admin          → Configura maestros, usuarios, supervisa todo
     ↓
Supplier       → Crea Guía de Remisión con productos y destinos
     ↓
Carrier        → Registra camión, acepta la guía, sale en ruta
     ↓
ReceptionMgr   → Recibe el camión, registra cantidades, faltantes y daños
     ↓
Stevedore      → Descarga físicamente la mercadería (rol operativo)
     ↓
Merchant       → Confirma su mercadería, actualiza stock, vende al cliente
     ↓
Admin/Carrier  → Liquidación del viaje al transportista
     ↓
Admin/Merchant → Gestión de cuentas por pagar al proveedor
```

---

## Estados de los documentos

| Documento | Estados |
|---|---|
| Guía de Remisión | `Pending` → `InTransit` → `Delivered` |
| Recepción | `Pending` → `InProgress` → `Completed` |
| Faltante/Reclamo | `Pending` → `Resolved` |
| Liquidación | `Pending` → `Paid` |
| Venta | `Pending` → `Completed` / `Void` |
| Cuenta por pagar | `Pending` → `PartialPaid` → `Paid` |
