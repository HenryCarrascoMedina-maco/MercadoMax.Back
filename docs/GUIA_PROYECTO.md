# 📖 Guía del Proyecto MercadoMAX

## Índice

1. [Roles del Sistema](#1-roles-del-sistema)
2. [Permisos por Rol](#2-permisos-por-rol)
3. [Módulos del Sistema](#3-módulos-del-sistema)
4. [Resumen de Funcionalidades por Módulo](#4-resumen-de-funcionalidades-por-módulo)
5. [Quién Puede Hacer Qué](#5-quién-puede-hacer-qué)
6. [Navegación (Sidebar)](#6-navegación-sidebar)
7. [Rutas del Frontend](#7-rutas-del-frontend)

---

## 1. Roles del Sistema

| Rol | Descripción |
|-----|-------------|
| **Admin** | Acceso completo al sistema. Gestión de usuarios, roles, permisos y toda la configuración. |
| **Supplier** (Proveedor) | Crea guías de remisión, consulta estado de envíos e historial. |
| **Carrier** (Transportista) | Consulta guías asignadas, tarifas y liquidaciones. |
| **ReceptionManager** (Jefe de Recepción) | Registra llegadas, valida carga, registra faltantes. |
| **Merchant** (Comerciante) | Confirma recepción, gestiona inventario, ventas y deudas. |
| **Stevedore** (Estibador) | Solo lectura: consulta asignaciones de descarga. |

---

## 2. Permisos por Rol

### 2.1 Admin
> **TODOS los permisos del sistema** (Users, Masters, Guides, Transport, Reception, Inventory, Finance)

| Módulo | Listar | Ver | Crear | Editar | Eliminar |
|--------|:------:|:---:|:-----:|:------:|:--------:|
| Users | ✅ | ✅ | ✅ | ✅ | ✅ |
| Masters | ✅ | ✅ | ✅ | ✅ | ✅ |
| Guides | ✅ | ✅ | ✅ | ✅ | ✅ |
| Transport | ✅ | ✅ | ✅ | ✅ | ✅ |
| Reception | ✅ | ✅ | ✅ | ✅ | — |
| Inventory | ✅ | ✅ | — | ✅ | — |
| Finance | ✅ | ✅ | ✅ | ✅ | — |

### 2.2 Supplier (Proveedor)

| Módulo | Listar | Ver | Crear | Editar | Eliminar |
|--------|:------:|:---:|:-----:|:------:|:--------:|
| Masters | ✅ | ✅ | ❌ | ❌ | ❌ |
| Guides | ✅ | ✅ | ✅ | ✅ | ✅ |
| Transport | ✅ | ✅ | ❌ | ❌ | ❌ |

### 2.3 Carrier (Transportista)

| Módulo | Listar | Ver | Crear | Editar | Eliminar |
|--------|:------:|:---:|:-----:|:------:|:--------:|
| Transport | ✅ | ✅ | ✅ | ✅ | ✅ |
| Guides | ✅ | ✅ | ❌ | ❌ | ❌ |

### 2.4 ReceptionManager (Jefe de Recepción)

| Módulo | Listar | Ver | Crear | Editar | Eliminar |
|--------|:------:|:---:|:-----:|:------:|:--------:|
| Reception | ✅ | ✅ | ✅ | ✅ | — |
| Guides | ✅ | ✅ | ❌ | ✅ | ❌ |
| Masters | ✅ | ✅ | ❌ | ❌ | ❌ |

### 2.5 Merchant (Comerciante)

| Módulo | Listar | Ver | Crear | Editar | Eliminar |
|--------|:------:|:---:|:-----:|:------:|:--------:|
| Inventory | ✅ | ✅ | — | ✅ | — |
| Reception | ✅ | ✅ | ❌ | ❌ | ❌ |
| Finance | ✅ | ✅ | ✅ | ✅ | — |
| Masters | ✅ | ✅ | ❌ | ❌ | ❌ |
| Guides | ✅ | ✅ | ❌ | ❌ | ❌ |

### 2.6 Stevedore (Estibador)

| Módulo | Listar | Ver | Crear | Editar | Eliminar |
|--------|:------:|:---:|:-----:|:------:|:--------:|
| Reception | ✅ | ✅ | ❌ | ❌ | ❌ |
| Guides | ✅ | ✅ | ❌ | ❌ | ❌ |

---

## 3. Módulos del Sistema

### Backend — Microservicios (.NET 8)

| Servicio | Puerto | Esquema BD | Descripción |
|----------|--------|------------|-------------|
| **Auth API** | 5001 | `auth` | Autenticación (JWT), usuarios, roles, permisos |
| **Maestros API** | 5002 | `master` | Datos maestros: productos, categorías, marcas, proveedores, pabellones, puestos, unidades logísticas, presentaciones |
| **Guías API** | 5003 | `guide` | Guías de remisión y sus detalles |
| **Transporte API** | 5004 | `transport` | Transportistas, camiones, tarifas de transporte |
| **Recepción API** | 5005 | `reception` | Recepciones de mercadería y registro de faltantes |
| **Comerciante API** | 5006 | `merchant` | Inventario de puestos |

### Frontend — Angular 21

| Módulo | Componentes | Descripción |
|--------|-------------|-------------|
| **Auth** | Login, Register, Forgot/Reset Password, Users, Roles, Permissions | Autenticación y administración de accesos |
| **Maestros** | 8 listas CRUD | Gestión de datos maestros |
| **Guías** | Guide List | Gestión de guías de remisión |
| **Transporte** | Carrier, Truck, Transport Rate Lists | Gestión de transportistas y tarifas |
| **Recepción** | Reception, Shortage Lists | Registro de recepciones y faltantes |
| **Comerciante** | Inventory List | Gestión de inventario por puesto |
| **Dashboard** | Dashboard (Inicio) | Pantalla de bienvenida |

---

## 4. Resumen de Funcionalidades por Módulo

### 🔐 Auth — Autenticación y Administración

| Componente | Qué hace | Operaciones |
|------------|----------|-------------|
| **Login** | Formulario de inicio de sesión con email/contraseña | Login → JWT Token |
| **Register** | Auto-registro de nuevos usuarios | Crear cuenta |
| **Forgot Password** | Solicitar restablecimiento de contraseña por email | Enviar token |
| **Reset Password** | Restablecer contraseña con token | Cambiar password |
| **Usuarios** | Lista paginada con búsqueda. CRUD completo. Asignación de roles por usuario (toggle de roles) | Listar, Crear, Editar, Eliminar, Asignar roles |
| **Roles** | Vista de tarjetas con los roles del sistema. Expandible para ver permisos agrupados por módulo | Solo lectura |
| **Permisos** | Vista de tarjetas agrupadas por módulo mostrando todos los permisos | Solo lectura |

### 📦 Maestros — Datos Maestros

| Componente | Campos | Funcionalidad especial |
|------------|--------|----------------------|
| **Categorías** | nombre, descripción | Búsqueda, paginación |
| **Productos** | nombre, categoría, descripción | **Filtro por categoría** (dropdown), búsqueda |
| **Presentaciones** | nombre, producto | **Filtro por producto** (dropdown) |
| **Marcas** | nombre, proveedor, producto | **Filtro por proveedor** y **filtro por producto** |
| **Proveedores** | razón social, RUC, teléfono, dirección, provincia, departamento, contacto | Búsqueda |
| **Unidades Logísticas** | nombre, abreviatura | CRUD básico |
| **Pabellones** | nombre, categoría, ubicación | Búsqueda |
| **Puestos** | número, pabellón | **Filtro por pabellón**, búsqueda |

> Todos los maestros tienen: CRUD completo + paginación client-side (10/25/50/100) + modal para crear/editar + confirmación para eliminar.

### 📋 Guías — Guías de Remisión

| Componente | Qué hace |
|------------|----------|
| **Guías** | Crear guía (proveedor, puesto, fecha emisión, total unidades logísticas, observaciones). Listar con paginación server-side. **Filtro por estado**. **Cambio de estado**: Emitida → EnTransito → Recibida. **Anular guía** (Anulada). No se puede editar ni eliminar una guía creada. |

**Estados de guía**: `Emitida` → `EnTransito` → `Recibida` | `Anulada`

### 🚛 Transporte

| Componente | Campos | Qué hace |
|------------|--------|----------|
| **Transportistas** | nombre, apellido, DNI, teléfono, nro. licencia, usuario vinculado | CRUD completo + búsqueda |
| **Camiones** | placa, transportista, capacidad, marca, modelo | CRUD completo + búsqueda |
| **Tarifas** | transportista, unidad logística, origen, destino, precio unitario, fecha vigencia | CRUD completo |

### 📥 Recepción

| Componente | Qué hace |
|------------|----------|
| **Recepciones** | Crear recepción (guía, puesto, fecha, observaciones). Listar con paginación server-side. **Filtro por estado**. **Cambio de estado**: Pendiente → EnProceso → Completada/ConFaltantes. No se puede editar ni eliminar. |
| **Faltantes** | Registrar faltante (detalle de recepción, cantidad faltante, razón, evidencia). Listar con paginación server-side. **Filtro por estado**. **Cambio de estado de reclamo**: Registrado → EnRevision → Aceptado/Rechazado. No se puede editar ni eliminar. |

**Estados de recepción**: `Pendiente` → `EnProceso` → `Completada` | `ConFaltantes`
**Estados de reclamo**: `Registrado` → `EnRevision` → `Aceptado` | `Rechazado`

### 🏪 Comerciante — Inventario

| Componente | Qué hace |
|------------|----------|
| **Inventario** | Crear registro (puesto, producto, unidad logística, stock actual, stock mínimo, costo promedio). **Solo se puede editar el stock mínimo**. **Filtro de stock bajo** (toggle). No se puede eliminar. Paginación client-side. |

### 🏠 Inicio (Dashboard)

| Componente | Qué hace |
|------------|----------|
| **Inicio** | Pantalla de bienvenida. Muestra nombre del usuario y roles asignados. No hace llamadas a API de datos. |

---

## 5. Quién Puede Hacer Qué

### Matriz de acceso por funcionalidad

| Funcionalidad | Admin | Proveedor | Transportista | Jefe Recepción | Comerciante | Estibador |
|--------------|:-----:|:---------:|:-------------:|:--------------:|:-----------:|:---------:|
| **Gestionar usuarios** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Ver roles/permisos** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **CRUD Maestros** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Ver Maestros** | ✅ | ✅ | ❌ | ✅ | ✅ | ❌ |
| **Crear/editar guías** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Ver guías** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Actualizar estado guía** | ✅ | ✅ | ❌ | ✅ | ❌ | ❌ |
| **Anular guía** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| **CRUD Transportistas/Camiones/Tarifas** | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ |
| **Ver transporte** | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Registrar recepción** | ✅ | ❌ | ❌ | ✅ | ❌ | ❌ |
| **Ver recepciones** | ✅ | ❌ | ❌ | ✅ | ✅ | ✅ |
| **Registrar faltantes** | ✅ | ❌ | ❌ | ✅ | ❌ | ❌ |
| **Gestionar inventario** | ✅ | ❌ | ❌ | ❌ | ✅ | ❌ |
| **Gestionar finanzas** | ✅ | ❌ | ❌ | ❌ | ✅ | ❌ |

> ⚠️ **Nota**: Actualmente el frontend solo restringe el menú de Administración (Usuarios/Roles/Permisos) al rol Admin verificando `hasRole('Admin')` en el sidebar. Las demás rutas no tienen guard por rol — cualquier usuario autenticado puede acceder si escribe la URL directamente.

---

## 6. Navegación (Sidebar)

```
🏠 Inicio               → /inicio

📌 Administración  (solo Admin)
  👥 Usuarios            → /users
  🛡️ Roles              → /roles
  🔑 Permisos           → /permissions

📦 Maestros
  📂 Categorías          → /product-categories
  🥦 Productos           → /products
  📏 Presentaciones      → /product-sizes
  🏷️ Marcas             → /brands
  🚚 Proveedores         → /suppliers
  📦 Unidades Logísticas → /logistic-units
  🏛️ Pabellones         → /pavilions
  🏪 Puestos             → /stalls

📋 Guías
  📋 Guías de Remisión   → /guides

🚛 Transporte
  🚛 Transportistas      → /carriers
  🚍 Camiones            → /trucks
  💰 Tarifas             → /transport-rates

📥 Recepción
  📥 Recepciones         → /receptions
  ⚠️ Faltantes           → /shortages

🏪 Comerciante
  📊 Inventario          → /inventory
```

---

## 7. Rutas del Frontend

### Rutas públicas (sin autenticación)

| Ruta | Componente |
|------|-----------|
| `/login` | Login |
| `/register` | Registro |
| `/forgot-password` | Recuperar contraseña |
| `/reset-password` | Restablecer contraseña |

### Rutas protegidas (requieren JWT)

| Ruta | Componente | Paginación |
|------|-----------|------------|
| `/inicio` | Inicio (Dashboard) | — |
| `/users` | Usuarios | Server-side |
| `/roles` | Roles | — (tarjetas) |
| `/permissions` | Permisos | — (tarjetas) |
| `/product-categories` | Categorías | Client-side |
| `/products` | Productos | Client-side |
| `/product-sizes` | Presentaciones | Client-side |
| `/brands` | Marcas | Client-side |
| `/suppliers` | Proveedores | Client-side |
| `/logistic-units` | Unidades Logísticas | Client-side |
| `/pavilions` | Pabellones | Client-side |
| `/stalls` | Puestos | Client-side |
| `/guides` | Guías | Server-side |
| `/carriers` | Transportistas | Client-side |
| `/trucks` | Camiones | Client-side |
| `/transport-rates` | Tarifas | Client-side |
| `/receptions` | Recepciones | Server-side |
| `/shortages` | Faltantes | Server-side |
| `/inventory` | Inventario | Client-side |

---

## Credenciales de prueba

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| admin@mercadomax.com | Admin123! | Admin |
