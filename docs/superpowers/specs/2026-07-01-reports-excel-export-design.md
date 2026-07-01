# Export a Excel de reportes — Design

## Contexto

`CLAUDE.md` documenta `/ReportsExcel` como pendiente de decidir alcance antes de planear. El endpoint existe en el backend real (`JoseDRestrepo/Ecosena-Api`, `ReportController`) como ruta absoluta en la raíz del sitio, fuera de `/api/Report`, igual que `/Estadisticas`.

**Verificado contra el código fuente real del backend** (no solo `v1.json`, que puede estar desactualizado — ver memoria `feedback_verify_against_real_backend`):

- **Ruta:** `GET /ReportsExcel`
- **Auth:** `[Authorize(Roles = "Administrador")]` — requiere Bearer JWT de un usuario Administrador
- **Respuesta:** `File()` con `Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`, bytes del `.xlsx`, nombre de archivo en `Content-Disposition` generado por el servidor como `reportes_{yyyy_MM}.xlsx` (mes actual, `DateTime.UtcNow`)
- **Query params:** ninguno — el endpoint solo exporta el mes actual, sin filtros

**Decisión clave que resuelve el alcance:** como la autenticación es Bearer-header-only (no hay soporte de token por query string), abrir la URL en un navegador externo fallaría con 401 — el navegador no tiene el JWT de la app. La única vía viable es que la app pida el archivo con su propio `HttpClient` (mismo patrón que los demás servicios, con el Bearer ya inyectado) y luego lo entregue al usuario nativamente.

## Flujo

```
[Botón en ReportsAdminView]
        ↓
ReportsAdminViewModel.ExportarExcelCommand
        ↓
IReportService.ExportarExcelAsync()
        ↓ GET https://ecosena-api.onrender.com/ReportsExcel (Bearer)
   bytes .xlsx + filename (leído de Content-Disposition)
        ↓
Escribir a FileSystem.CacheDirectory (temporal, privado de la app — sin permisos de almacenamiento)
        ↓
Share.Default.RequestAsync(...) → share sheet nativo de Android
```

## Componentes

### `Services/Report/IReportService.cs` + `ReportService.cs`

Nuevo método:

```csharp
Task<(byte[] Bytes, string FileName)?> ExportarExcelAsync();
```

- `GET` a `https://ecosena-api.onrender.com/ReportsExcel` (constante absoluta, mismo patrón que `StatsUrl` del Plan 1) usando el `CreateClientAsync()` ya existente (Bearer automático).
- Nombre de archivo: `response.Content.Headers.ContentDisposition?.FileName`, con fallback a `$"reportes_{DateTime.Now:yyyy_MM}.xlsx"` si el header no viene (defensivo — el backend siempre lo manda, pero no debe romper si algún día no lo hace).
- Igual que el resto del servicio: catch-and-return-null en caso de excepción/status no exitoso, sin relanzar.

### `ViewModels/Report/ReportsAdminViewModel.cs`

Nuevo `[RelayCommand] ExportarExcelAsync()`:

- Reutiliza el `IsBusy` existente (deshabilita mientras corre, igual que `LoadAsync`).
- Llama a `IReportService.ExportarExcelAsync()`.
- Si falla (`null`): `Toast.Make("No se pudo exportar el Excel.")`.
- Si funciona: escribe los bytes a `Path.Combine(FileSystem.CacheDirectory, fileName)` y dispara `Share.Default.RequestAsync(new ShareFileRequest { Title = "Reportes", File = new ShareFile(path) })`.

### `Views/Controls/AdminContentViews/Reports/ReportsAdminView.xaml` + `.xaml.cs`

- Un botón/ícono pequeño (ícono de descarga/exportar, estilo consistente con los demás íconos SVG tinteados del design system) ubicado junto a la grilla de las 4 tarjetas de estadísticas.
- Sin lógica de rol adicional — toda la vista ya es exclusiva de `Administrador`.
- Wireado directo al `ExportarExcelCommand` del ViewModel (binding de comando, no evento de code-behind — sigue el patrón `[RelayCommand]` + `Command="{Binding ...}"` ya usado en otras vistas con `BindingContext` = su ViewModel).

## Manejo de errores

- Red/backend caído o respuesta no exitosa → `Toast` de error, mismo estilo que los demás mensajes de la vista (`"No se pudieron cargar los reportes."`, etc.).
- No se maneja permisos de almacenamiento: al escribir solo en `FileSystem.CacheDirectory` (privado de la app), Android no requiere `WRITE_EXTERNAL_STORAGE` ni diálogos de permiso — el share sheet es quien decide dónde termina el archivo (Drive, Descargas vía el gestor de archivos, WhatsApp, etc.).
- No se limpia el archivo temporal después de compartir — es aceptable dejarlo en caché (el sistema operativo la gestiona/purga; no es un directorio de usuario visible). Fuera de alcance de este spec.

## Fuera de alcance

- Filtros de fecha/rango — el backend solo exporta el mes actual, no hay query params que lo permitan.
- Exportación para roles distintos de Administrador — el backend la rechaza (403), y la UI de todos modos no expone reportes agregados a otros roles.
- Cache/reintento automático de la descarga — un fallo simplemente muestra el Toast; el usuario reintenta tocando el botón de nuevo.
