# Fix ContentViews de rol + endpoints de escritura — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Corregir dos bugs de flujo principal: (1) `HostPage` no revalida el rol del usuario tras su primera construcción porque MAUI Shell cachea esa instancia, dejando `ContentView`s y footer con el rol viejo tras logout/login con otro rol; (2) crear reporte, crear/editar entrada de blog y editar perfil fallan siempre porque el cliente manda los campos de texto por query string mientras el backend real los espera como multipart form fields.

**Architecture:** Bug 1 se resuelve extrayendo la lógica de construcción dependiente de rol de `HostPage` a un método reusable (`ApplyRole()`) invocado tanto en el constructor como en un nuevo override de `OnAppearing()` que detecta cambios de rol comparando contra el último rol aplicado. `HomeContainerView.Refresh()` se extiende para reconstruir también el hero. Bug 2 se resuelve moviendo los campos de texto de la URL (query string) a partes `StringContent` dentro del `MultipartFormDataContent` ya existente en cada uno de los 3 servicios HTTP afectados, sin cambiar firmas públicas.

**Tech Stack:** .NET MAUI (net10.0-android primario), `CommunityToolkit.Mvvm`, `IHttpClientFactory` (`Services/Http/`).

## Global Constraints

- No hay framework de tests automatizados en este proyecto (ver `CLAUDE.md`) — cada tarea verifica con `dotnet build -f net10.0-android` (compila limpio) más un script de verificación manual explícito, en vez de tests unitarios.
- Los nombres de las partes `StringContent` dentro de cada `MultipartFormDataContent` deben coincidir exactamente con el nombre de la propiedad del DTO del backend real (`ReportReqDto`, `PostEntradaReqDto`, `EditEntradaReqDto`, `EditProfileReqDto` en `JoseDRestrepo/Ecosena-Api`): `Titulo`, `Descripcion`, `IdAmbiente`, `Contenido`, `Email`, `FechaNacimiento`, `Contraseña`, `ConfirmacionContraseña`, `Foto`, `Portada`, `FotoPerfil`.
- No se cambian firmas públicas de `IReportService`, `IBlogService`, `IProfileService` — solo la implementación interna de cómo se arma la petición HTTP.
- No se toca `ReportService.UpdateEstadoAsync` ni `PenalizarAsync` — no llevan DTO con campos de texto, no están afectados.
- Cada commit se hace por task completa (no mezclar tasks en un commit).

---

### Task 1: `HostPage` — extraer `ApplyRole()` y revalidar rol en `OnAppearing`

**Files:**
- Modify: `Views/Host/HostPage.xaml.cs` (archivo completo tiene 149 líneas; se toca el constructor, líneas 12-24, y se agrega método nuevo + override)

**Interfaces:**
- Consumes: `IUserSession.Role` (`Services/Session/IUserSession.cs`, ya existente — propiedad `string Role`), `FootBarView.SetReportTabVisible(bool)` (ya existente), `ShowSection(string key)` (ya existente en esta misma clase).
- Produces: método privado `void ApplyRole()` en `HostPage`, usado también por Task 2 no aplica (Task 2 es en otro archivo). Ningún otro archivo consume `ApplyRole()` directamente.

- [ ] **Step 1: Reemplazar el constructor y agregar `_lastRole`, `ApplyRole()` y `OnAppearing()`**

Editar `Views/Host/HostPage.xaml.cs`. El archivo actual (líneas 1-24) es:

```csharp
using EcosenaApp.Services.Session;
using EcosenaApp.Views.Controls;

namespace EcosenaApp.Views.Host;

public partial class HostPage : ContentPage
{
    private readonly Dictionary<string, View> _cache = new();
    private readonly Stack<string> _history = new();
    private readonly IUserSession? _userSession;

    public HostPage()
    {
        InitializeComponent();

        _userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();
        MainFootBar.SetReportTabVisible(_userSession?.Role != "Invitado");

        // Wire up the footer selection
        MainFootBar.SelectedIndexChanged += OnFooterSelectionChanged;

        // Set initial view
        ShowSection("Home");
    }
```

Reemplazar por:

```csharp
using EcosenaApp.Services.Session;
using EcosenaApp.Views.Controls;

namespace EcosenaApp.Views.Host;

public partial class HostPage : ContentPage
{
    private readonly Dictionary<string, View> _cache = new();
    private readonly Stack<string> _history = new();
    private readonly IUserSession? _userSession;
    private string? _lastRole;

    public HostPage()
    {
        InitializeComponent();

        _userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();

        // Wire up the footer selection
        MainFootBar.SelectedIndexChanged += OnFooterSelectionChanged;

        ApplyRole();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var role = _userSession?.Role ?? "Invitado";
        if (role != _lastRole)
        {
            ApplyRole();
        }
    }

    private void ApplyRole()
    {
        var role = _userSession?.Role ?? "Invitado";

        MainFootBar.SetReportTabVisible(role != "Invitado");
        _cache.Clear();
        _history.Clear();

        ShowSection("Home");

        _lastRole = role;
    }
```

El resto del archivo (`OnFooterSelectionChanged`, `ShowSection`, `CreateViewForKey`, `CreateHomeView`, `BuildPenalizadoView`, `RefreshIfSupported`, `TryGoBack`) queda sin cambios.

- [ ] **Step 2: Compilar**

Run: `dotnet build -f net10.0-android`
Expected: `Build succeeded.` sin errores en `HostPage.xaml.cs`.

- [ ] **Step 3: Verificación manual**

Con un emulador/dispositivo Android conectado (`dotnet run -f net10.0-android`), reproducir:

1. Loguear como Aprendiz → tocar pestaña "Reportar" → confirmar que se ve `ReportsUserView` (lista de reportes propios + botón crear).
2. Ir a Perfil (topbar) → "Cerrar sesión".
3. Loguear como Administrador (u otra cuenta con rol distinto).
4. Sin cerrar la app, tocar pestaña "Reportar" → **debe** verse `ReportsAdminView` (lista de todos los reportes + estadísticas), no la vista de Aprendiz.
5. Ir a Home → confirmar que el hero mostrado corresponde a Administrador (`HeroAdminView`), no al de Aprendiz.
6. Repetir el ciclo entrando como Invitado (botón "Invitado" en LoginPage) → confirmar que la pestaña "Reportar" desaparece del footer.

Si los 3 roles muestran el contenido correcto tras cada relogueo, el fix funciona.

- [ ] **Step 4: Commit**

```bash
git add "Views/Host/HostPage.xaml.cs"
git commit -m "fix: revalidar rol de usuario en HostPage.OnAppearing"
```

---

### Task 2: `HomeContainerView` — reconstruir el hero en `Refresh()`

**Files:**
- Modify: `Views/Controls/HomeContainerView.xaml.cs` (archivo completo, 55 líneas)

**Interfaces:**
- Consumes: nada nuevo — usa lo mismo que ya usaba `BuildHero()` (`IUserSession` vía service-locator).
- Produces: `HomeContainerView.Refresh()` ahora también reconstruye el hero, además de refrescar `RecentBlogPanel`. Task 1's `ApplyRole()` limpia `_cache` completo (por lo que de todas formas recrea `HomeContainerView` desde cero), pero este cambio es defensivo para cualquier código que en el futuro solo llame `Refresh()` sobre una instancia cacheada.

- [ ] **Step 1: Cambiar `Refresh()` para que también llame `BuildHero()`**

El archivo actual, línea 52, es:

```csharp
        public void Refresh() => RecentBlogPanel.Refresh();
```

Reemplazar por:

```csharp
        public void Refresh()
        {
            BuildHero();
            RecentBlogPanel.Refresh();
        }
```

El resto del archivo (constructor, `BuildHero()`, `CreateAdminHero()`, `CreateUserHero()`, `CreateGuestHero()`) queda sin cambios — `BuildHero()` ya es un método privado reusable, no requiere extracción adicional.

- [ ] **Step 2: Compilar**

Run: `dotnet build -f net10.0-android`
Expected: `Build succeeded.` sin errores en `HomeContainerView.xaml.cs`.

- [ ] **Step 3: Verificación manual**

Ya cubierta por el Step 3 de Task 1 (punto 5: el hero de Home cambia correctamente tras relogueo). No se requiere repetir un escenario adicional — este cambio es defensivo y su efecto visible coincide con el de Task 1.

- [ ] **Step 4: Commit**

```bash
git add "Views/Controls/HomeContainerView.xaml.cs"
git commit -m "fix: reconstruir hero de Home en Refresh()"
```

---

### Task 3: `ReportService.PostReportAsync` — campos de texto como multipart form fields

**Files:**
- Modify: `Services/Report/ReportService.cs:105-129`

**Interfaces:**
- Consumes: `HttpClientNames.Authenticated` (ya existente, sin cambios).
- Produces: `IReportService.PostReportAsync(string titulo, string descripcion, int idAmbiente, Stream? foto, string? fileName)` — firma pública sin cambios; solo cambia cómo arma la petición HTTP.

- [ ] **Step 1: Reemplazar el método**

El método actual (`Services/Report/ReportService.cs:105-129`) es:

```csharp
    public async Task<ReportResDto?> PostReportAsync(string titulo, string descripcion, int idAmbiente, Stream? foto, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);
            var url = $"{BaseUrl}?Titulo={Uri.EscapeDataString(titulo)}&Descripcion={Uri.EscapeDataString(descripcion)}&IdAmbiente={idAmbiente}";

            using var content = new MultipartFormDataContent();
            if (foto != null)
                content.Add(new StreamContent(foto), "Foto", fileName ?? "foto.jpg");

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReportResDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ReportService.PostReportAsync error: {ex.Message}");
            return null;
        }
    }
```

Reemplazar por:

```csharp
    public async Task<ReportResDto?> PostReportAsync(string titulo, string descripcion, int idAmbiente, Stream? foto, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);

            using var content = new MultipartFormDataContent
            {
                { new StringContent(titulo), "Titulo" },
                { new StringContent(descripcion), "Descripcion" },
                { new StringContent(idAmbiente.ToString()), "IdAmbiente" },
            };
            if (foto != null)
                content.Add(new StreamContent(foto), "Foto", fileName ?? "foto.jpg");

            var response = await client.PostAsync(BaseUrl, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReportResDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ReportService.PostReportAsync error: {ex.Message}");
            return null;
        }
    }
```

- [ ] **Step 2: Compilar**

Run: `dotnet build -f net10.0-android`
Expected: `Build succeeded.` sin errores en `ReportService.cs`.

- [ ] **Step 3: Verificación manual**

1. Loguear como Aprendiz.
2. Ir a "Reportar" → tocar botón de crear reporte.
3. Completar Título, Descripción, elegir un Ambiente, adjuntar una foto (galería o cámara).
4. Enviar.
5. Esperado: toast **"Reporte enviado."** (no "No se pudo enviar el reporte."), y el reporte aparece en la lista propia al volver.

- [ ] **Step 4: Commit**

```bash
git add "Services/Report/ReportService.cs"
git commit -m "fix: enviar campos de texto de PostReportAsync como multipart form fields"
```

---

### Task 4: `BlogService` — `PostEntradaAsync` y `PutEntradaAsync` como multipart form fields

**Files:**
- Modify: `Services/Blog/BlogService.cs:66-128`

**Interfaces:**
- Consumes: `HttpClientNames.Authenticated` (sin cambios).
- Produces: `IBlogService.PostEntradaAsync(string titulo, string contenido, Stream? portada, string? fileName)` y `IBlogService.PutEntradaAsync(int id, string titulo, string contenido, Stream? portada, string? fileName)` — firmas públicas sin cambios. El helper privado `BuildPortadaContent(Stream?, string?)` se reemplaza por `BuildEntradaContent(string, string, Stream?, string?)`, usado solo dentro de esta clase.

- [ ] **Step 1: Reemplazar `PostEntradaAsync`, `PutEntradaAsync` y `BuildPortadaContent`**

El bloque actual (`Services/Blog/BlogService.cs:66-128`) es:

```csharp
    public async Task<EntradaResDto?> PostEntradaAsync(string titulo, string contenido, Stream? portada, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);
            var url = $"{BaseUrl}?Titulo={Uri.EscapeDataString(titulo)}&Contenido={Uri.EscapeDataString(contenido)}";

            using var content = BuildPortadaContent(portada, fileName);
            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EntradaResDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.PostEntradaAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> PutEntradaAsync(int id, string titulo, string contenido, Stream? portada, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);
            var url = $"{BaseUrl}/{id}?Titulo={Uri.EscapeDataString(titulo)}&Contenido={Uri.EscapeDataString(contenido)}";

            using var content = BuildPortadaContent(portada, fileName);
            var response = await client.PutAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.PutEntradaAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteEntradaAsync(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);
            var response = await client.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.DeleteEntradaAsync error: {ex.Message}");
            return false;
        }
    }

    private static MultipartFormDataContent BuildPortadaContent(Stream? portada, string? fileName)
    {
        var content = new MultipartFormDataContent();
        if (portada != null)
            content.Add(new StreamContent(portada), "Portada", fileName ?? "portada.jpg");
        return content;
    }
```

Reemplazar por:

```csharp
    public async Task<EntradaResDto?> PostEntradaAsync(string titulo, string contenido, Stream? portada, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);

            using var content = BuildEntradaContent(titulo, contenido, portada, fileName);
            var response = await client.PostAsync(BaseUrl, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EntradaResDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.PostEntradaAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> PutEntradaAsync(int id, string titulo, string contenido, Stream? portada, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);

            using var content = BuildEntradaContent(titulo, contenido, portada, fileName);
            var response = await client.PutAsync($"{BaseUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.PutEntradaAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteEntradaAsync(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);
            var response = await client.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.DeleteEntradaAsync error: {ex.Message}");
            return false;
        }
    }

    private static MultipartFormDataContent BuildEntradaContent(string titulo, string contenido, Stream? portada, string? fileName)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(titulo), "Titulo" },
            { new StringContent(contenido), "Contenido" },
        };
        if (portada != null)
            content.Add(new StreamContent(portada), "Portada", fileName ?? "portada.jpg");
        return content;
    }
```

- [ ] **Step 2: Compilar**

Run: `dotnet build -f net10.0-android`
Expected: `Build succeeded.` sin errores en `BlogService.cs`.

- [ ] **Step 3: Verificación manual**

1. Loguear como Administrador.
2. Ir a "Blog" → tocar botón de crear entrada nueva.
3. Completar Título y Contenido, adjuntar portada.
4. Publicar → esperado: toast **"Entrada publicada."**, la entrada aparece en la lista.
5. Abrir esa misma entrada → "Editar" → cambiar el Título.
6. Guardar → esperado: toast **"Cambios guardados."**, el título nuevo se refleja al volver al detalle.

- [ ] **Step 4: Commit**

```bash
git add "Services/Blog/BlogService.cs"
git commit -m "fix: enviar Titulo/Contenido de PostEntradaAsync y PutEntradaAsync como multipart form fields"
```

---

### Task 5: `ProfileService.UpdateProfileAsync` — campos de texto como multipart form fields

**Files:**
- Modify: `Services/Profile/ProfileService.cs:37-62`

**Interfaces:**
- Consumes: `HttpClientNames.Authenticated` (sin cambios).
- Produces: `IProfileService.UpdateProfileAsync(string email, DateOnly? fechaNacimiento, string? contraseña, string? confirmacion, Stream? foto, string? fileName)` — firma pública sin cambios.

- [ ] **Step 1: Reemplazar el método**

El método actual (`Services/Profile/ProfileService.cs:37-62`) es:

```csharp
    public async Task<bool> UpdateProfileAsync(string email, DateOnly? fechaNacimiento,
        string? contraseña, string? confirmacion, Stream? foto, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);

            var query = $"Email={Uri.EscapeDataString(email)}";
            if (fechaNacimiento.HasValue)
                query += $"&FechaNacimiento={fechaNacimiento.Value:yyyy-MM-dd}";
            if (!string.IsNullOrEmpty(contraseña))
                query += $"&Contraseña={Uri.EscapeDataString(contraseña)}&ConfirmacionContraseña={Uri.EscapeDataString(confirmacion ?? string.Empty)}";

            using var content = new MultipartFormDataContent();
            if (foto != null)
                content.Add(new StreamContent(foto), "FotoPerfil", fileName ?? "foto.jpg");

            var response = await client.PutAsync($"{Url}?{query}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProfileService.UpdateProfileAsync error: {ex.Message}");
            return false;
        }
    }
```

Reemplazar por:

```csharp
    public async Task<bool> UpdateProfileAsync(string email, DateOnly? fechaNacimiento,
        string? contraseña, string? confirmacion, Stream? foto, string? fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);

            using var content = new MultipartFormDataContent
            {
                { new StringContent(email), "Email" },
            };
            if (fechaNacimiento.HasValue)
                content.Add(new StringContent(fechaNacimiento.Value.ToString("yyyy-MM-dd")), "FechaNacimiento");
            if (!string.IsNullOrEmpty(contraseña))
            {
                content.Add(new StringContent(contraseña), "Contraseña");
                content.Add(new StringContent(confirmacion ?? string.Empty), "ConfirmacionContraseña");
            }
            if (foto != null)
                content.Add(new StreamContent(foto), "FotoPerfil", fileName ?? "foto.jpg");

            var response = await client.PutAsync(Url, content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProfileService.UpdateProfileAsync error: {ex.Message}");
            return false;
        }
    }
```

- [ ] **Step 2: Compilar**

Run: `dotnet build -f net10.0-android`
Expected: `Build succeeded.` sin errores en `ProfileService.cs`.

- [ ] **Step 3: Verificación manual**

1. Loguear (Aprendiz o Administrador).
2. Ir a Perfil (topbar) → "Editar perfil".
3. Cambiar el Email (o la fecha de nacimiento).
4. Guardar sin tocar la foto ni la contraseña.
5. Esperado: toast **"Perfil actualizado."**, y al volver a Perfil se ve el Email nuevo.
6. Repetir cambiando también la contraseña (con confirmación igual) → mismo resultado esperado.

- [ ] **Step 4: Commit**

```bash
git add "Services/Profile/ProfileService.cs"
git commit -m "fix: enviar campos de texto de UpdateProfileAsync como multipart form fields"
```
