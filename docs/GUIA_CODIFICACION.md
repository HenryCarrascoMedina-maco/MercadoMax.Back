# Guía de Codificación — MercadoMAX

Reglas obligatorias que todo el código del proyecto debe seguir.  
Aplica a Backend (.NET 10) y Frontend (Angular 21).

---

## 1. Sin Hardcoding

**Nunca** escribir texto visible para el usuario directamente en el código.

### Frontend (Angular)
Todo texto mostrado en pantalla va en los archivos de traducción:
- `Frontend/mercado-max/public/i18n/es.json`
- `Frontend/mercado-max/public/i18n/en.json`

Cada componente usa Transloco con `*transloco="let t"` y llama `t('clave.subClave')`.

```html
<!-- ❌ MAL -->
<label>Fecha de despacho *</label>
<button title="Ver detalle">🔍</button>

<!-- ✅ BIEN -->
<label>{{ t('guidesPage.labelShipmentDate') }} *</label>
<button [title]="t('guidesPage.actionView')">🔍</button>
```

Para estados dinámicos (enums), construir la clave con el valor del enum:
```html
<!-- ✅ BIEN — 'guidesPage.statusPending', 'guidesPage.statusInTransit', etc. -->
{{ t('guidesPage.status' + item.guideStatus) }}
```

**Nunca** usar un método TypeScript que devuelva strings hardcodeados:
```typescript
// ❌ MAL
statusLabel(status: string): string {
  return { 'Pending': 'Pendiente', 'InTransit': 'En Tránsito' }[status];
}

// ✅ BIEN — mover la lógica al template con t()
```

### Backend (.NET)
Nunca hardcodear mensajes de error, nombres de roles o cualquier constante de dominio
directamente en los controladores o servicios. Usar:
- `MercadoMAX.Shared` para constantes reutilizables.
- Mensajes de respuesta deben venir de los Stored Procedures (parámetro `@Message` en `SpResult`).

---

## 2. Carpeta de Uso Compartido / Reutilizable

### Backend → `MercadoMAX.Shared/`
Todo lo que se usa en **2 o más proyectos** va aquí:

| Carpeta | Contenido |
|---|---|
| `MercadoMAX.Shared/DTOs/` | `ApiResponse<T>`, `SpResult` |
| `MercadoMAX.Shared/Authorization/` | `PermissionAuthorizationHandler`, `PermissionRequirement`, `PermissionAuthorizationAttribute` |
| `MercadoMAX.Shared/Data/` | Configuraciones de Dapper u otros helpers de acceso a datos |

Agregar una referencia al proyecto Shared desde los APIs que lo necesiten (ya está configurado en el `.slnx`).

### Frontend → `src/app/shared/`
Todo componente, pipe, directiva o servicio que se usa en **2 o más módulos/features** va aquí:

```
src/app/shared/
  components/
    pagination/          ← componente de paginación reutilizable
    lang-selector/       ← selector de idioma
```

Regla: si un componente existe solo en un feature, vive en ese feature. En cuanto se reutiliza, se mueve a `shared/`.

---

## 3. Convenciones de Nombrado

### Backend
| Elemento | Convención | Ejemplo |
|---|---|---|
| Controlador | `{Entidad}Controller` | `GuideController` |
| Servicio | `I{Entidad}Service` / `{Entidad}Service` | `IGuideService` / `GuideService` |
| Repositorio | `I{Entidad}Repository` / `{Entidad}Repository` | `IGuideRepository` |
| DTO Request | `Create{Entidad}Request`, `Update{Entidad}Request` | `CreateGuideRequest` |
| DTO Response | `{Entidad}Response` | `GuideResponse` |
| Stored Procedure | `SP_{VERBO}_{ENTIDAD}` | `SP_CREATE_GUIDE`, `SP_LIST_GUIDE` |

### Frontend
| Elemento | Convención | Ejemplo |
|---|---|---|
| Componente | `{entidad}-list`, `{entidad}-form` | `guide-list`, `user-form` |
| Servicio | `{Entidad}Service` en `core/services/` | `GuideService` |
| Modelo | `{Entidad}Response`, `Create{Entidad}Request` | `GuideResponse` |
| Clave i18n | `{modulePage}.{clave}` | `guidesPage.supplier` |

---

## 4. Patrón de Repositorio (Backend)

Todos los repositorios usan **Dapper + Stored Procedures exclusivamente**. No se usa LINQ to SQL ni Entity Framework.

```csharp
// ✅ BIEN
public async Task<ApiResponse<int>> CreateAsync(CreateGuideRequest req)
{
    var result = await _db.QueryFirstOrDefaultAsync<SpResult>(
        "EXEC sp_guide.SP_CREATE_GUIDE @SupplierId, @ShipmentDate, ...",
        req);
    return ApiResponse<int>.FromSpResult(result, result?.Id ?? 0);
}
```

---

## 5. Patrón de Componente (Frontend)

Todos los componentes de lista siguen este patrón:

```typescript
@Component({
  selector: 'app-{entidad}-list',
  standalone: true,
  imports: [ReactiveFormsModule, DatePipe, TranslocoModule /*, otros pipes necesarios */],
  templateUrl: './{entidad}-list.component.html',
  styleUrl: './{entidad}-list.component.scss'
})
export class {Entidad}ListComponent implements OnInit {
  // Estado con Signals
  items = signal<{Entidad}Response[]>([]);
  loading = signal(false);
  message = signal<{ text: string; type: 'success' | 'error' } | null>(null);

  // Inyección de dependencias
  constructor(private svc: {Entidad}Service, private auth: AuthService) {}
}
```

- **Signals API** para estado reactivo (no `BehaviorSubject` ni variables planas para estado UI).
- **No NgModule** — todos los componentes son `standalone: true`.
- **No `any`** en TypeScript — usar los tipos definidos en `core/models/`.

---

## 6. Permisos y Seguridad

### Backend
Todos los endpoints que no son públicos llevan el atributo:
```csharp
[PermissionAuthorization("Modulo", "Accion")]
```
Los módulos y acciones disponibles se definen en `auth.Permission` y se gestionan desde el admin.

### Frontend
La visibilidad de secciones del sidebar y rutas se controla mediante los permisos del JWT decodificado en `AuthService`. No se duplica lógica de permisos en los componentes individuales.

---

## 7. Scripts de Base de Datos

Todos los cambios a la BD se documentan en `ScriptBD/` con numeración secuencial:

```
01_CreateDatabase.sql
02_Tables_Auth.sql
...
20_SeedData_Users.sql   ← agregar el número siguiente disponible
```

- Los scripts deben ser **idempotentes** (`IF NOT EXISTS`, `IF OBJECT_ID IS NULL`).
- Los Stored Procedures usan `CREATE OR ALTER PROCEDURE`.
- Nunca modificar un script ya ejecutado en producción — crear uno nuevo numerado.

---

## 8. Docker y Configuración

- Las cadenas de conexión y secretos van en `appsettings.Development.json` (local) y variables de entorno en producción/Docker. **Nunca en el código fuente**.
- Los puertos de los servicios están documentados en `PLAN_PROYECTO.md` y en `docker-compose.yml`.
