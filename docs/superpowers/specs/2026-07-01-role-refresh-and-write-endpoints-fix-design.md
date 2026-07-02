# Fix: ContentViews de rol no se actualizan + operaciones de escritura fallan — Design

## Contexto

Dos bugs reportados en el flujo principal de la app:

1. Al iniciar sesión con un rol distinto al que se usó la primera vez que se navegó a `HostPage` en la sesión de la app (p. ej. cerrar sesión como Aprendiz y volver a entrar como Administrador, o entrar como Invitado y luego loguearse), las `ContentView` de la sección "Reportar" y el hero de "Home" siguen mostrando las del rol anterior, y la visibilidad de la pestaña "Reportar" en el footer no se actualiza. El `ProfilePage` sí muestra los datos correctos (se navega con `Navigation.PushAsync`, instancia nueva cada vez).
2. Crear reporte, crear entrada de blog, editar entrada de blog y editar perfil siempre fallan con el toast de error correspondiente, tanto para Aprendiz como para Administrador.

## Causa raíz — Bug 1

`AppShell.xaml` declara `HostPage` como `ShellContent` (`Route="HostPage"`). MAUI Shell crea una única instancia de esa página y la reutiliza durante toda la vida de la app; `Shell.Current.GoToAsync("//HostPage")` (usado en login, guest-login y `App.OnStart`) navega a esa misma instancia cacheada, así que el constructor de `HostPage` — y el de `HomeContainerView`, que construye en su propio constructor — solo se ejecutan la primera vez que se navega a esa ruta en toda la sesión del proceso.

`IUserSession` (singleton) sí se actualiza correctamente en cada `SetFromToken`/`SetGuest`/`Clear`, pero nada vuelve a leer esa role después del primer build de `HostPage`:

- `HostPage()` constructor: llama `MainFootBar.SetReportTabVisible(role != "Invitado")` una sola vez.
- `HostPage._cache`: guarda la `View` de `"Report"` (creada por `CreateViewForKey`, que sí bifurca por rol) la primera vez que se pide esa key; `ShowSection` nunca vuelve a llamar `CreateViewForKey` para una key ya cacheada.
- `HomeContainerView.BuildHero()`: solo se ejecuta en el constructor de `HomeContainerView`; `HomeContainerView.Refresh()` (invocado por `RefreshIfSupported` en cada `ShowSection("Home")`) solo refresca el panel de blog reciente, no el hero.

No es un bug del refactor reciente a `IHttpClientFactory` ni de `UserSession` — es puramente que `HostPage` nunca vuelve a evaluar el rol tras su primera construcción.

## Causa raíz — Bug 2

Verificado contra el código fuente real del backend (`JoseDRestrepo/Ecosena-Api`, no solo `v1.json` — ver memoria `feedback_verify_against_real_backend`):

Los DTOs de los 4 endpoints de escritura afectados tienen una propiedad `IFormFile`:

- `ReportReqDto` (`POST /api/Report`): `Titulo`, `Descripcion`, `IdAmbiente`, `required IFormFile Foto`
- `PostEntradaReqDto` (`POST /api/Blog`): `Titulo`, `Contenido`, `IFormFile? Portada`
- `EditEntradaReqDto` (`PUT /api/Blog/{id}`): `Titulo`, `Contenido`, `IFormFile? Portada`
- `EditProfileReqDto` (`PUT /api/Profile`): `Email`, `FechaNacimiento`, `Contraseña`, `ConfirmacionContraseña`, `IFormFile? FotoPerfil`

Regla de inferencia de binding de ASP.NET Core `[ApiController]`: un parámetro de tipo complejo que contiene una propiedad `IFormFile`/`IFormFileCollection` se bindea completo con `[FromForm]` (lee `Request.Form`), no `[FromBody]` ni query string.

El cliente (`ReportService.PostReportAsync`, `BlogService.PostEntradaAsync`/`PutEntradaAsync`, `ProfileService.UpdateProfileAsync`) arma la URL con los campos de texto como query string (`?Titulo=...&Descripcion=...`) y solo agrega el archivo al `MultipartFormDataContent`. `[FromForm]` no lee query string, así que todos los campos de texto llegan `null`/vacíos/`0` al backend sin importar lo que se mande en la URL:

- Reporte: `Foto` es `required` → sin foto adjunta, `ModelState` inválido → 400 automático. Con foto, `Titulo`/`Descripcion`/`IdAmbiente` igual llegan vacíos/0.
- Blog/Perfil: sin atributos `[Required]` en el DTO, pero la capa de servicio del backend (constraints de datos, validación de negocio) rechaza título/email vacío, causando que el endpoint responda con un status no exitoso.

No es un bug introducido por la migración reciente a `IHttpClientFactory` — los diffs de esos commits son 1:1 en comportamiento (mismo patrón de armado de URL y `MultipartFormDataContent` antes y después). El bug es preexistente al refactor.

## Fix — Bug 1: `HostPage` detecta cambio de rol en `OnAppearing`

- Agregar campo privado `_lastRole` a `HostPage`, inicializado junto con `_userSession` en el constructor.
- Extraer a un método privado `ApplyRole()` la lógica que hoy vive dispersa en el constructor:
  - `MainFootBar.SetReportTabVisible(role != "Invitado")`
  - `_cache.Clear()`
  - `_history.Clear()`
  - `ShowSection("Home")`
  - actualizar `_lastRole = role`
- El constructor pasa a llamar `ApplyRole()` en vez de tener esa lógica inline.
- Override `OnAppearing()`: leer `_userSession?.Role ?? "Invitado"`; si es distinto de `_lastRole`, llamar `ApplyRole()`. Si es igual, no tocar nada (evita recrear vistas innecesariamente cada vez que la página reaparece, p. ej. al volver de `ProfilePage`).
- `HomeContainerView`: extraer `BuildHero()` para que también se pueda re-invocar; agregar su llamada dentro de `Refresh()` (además de `RecentBlogPanel.Refresh()`), así cualquier futura vía que solo llame `Refresh()` sobre una instancia cacheada de `HomeContainerView` también corrige el hero. Como `ApplyRole()` limpia `_cache` por completo, `HomeContainerView` se recrea de cero de todas formas — este cambio es defensivo para no depender únicamente de esa limpieza total.

Efecto: cualquier ruta que termine en `GoToAsync("//HostPage")` con un rol distinto al de la última vez (login fresco, guest→login, logout→login con otro rol) dispara `OnAppearing` → detecta el cambio → reconstruye footer, Home (hero correcto) y Report (vista correcta por rol) desde cero.

## Fix — Bug 2: campos de texto como multipart form fields

En los 3 servicios, mover cada campo de texto de la query string a una parte `StringContent` dentro del `MultipartFormDataContent` ya existente, usando como nombre de parte el nombre exacto de la propiedad del DTO del backend. La URL de la petición queda solo con el path (sin `?...`).

### `ReportService.PostReportAsync`

```csharp
var content = new MultipartFormDataContent
{
    { new StringContent(titulo), "Titulo" },
    { new StringContent(descripcion), "Descripcion" },
    { new StringContent(idAmbiente.ToString()), "IdAmbiente" },
};
if (foto != null)
    content.Add(new StreamContent(foto), "Foto", fileName ?? "foto.jpg");

var response = await client.PostAsync(BaseUrl, content);
```

### `BlogService.PostEntradaAsync` / `PutEntradaAsync`

Mismo patrón: `Titulo` y `Contenido` como `StringContent` dentro del `MultipartFormDataContent` que ya arma `BuildPortadaContent`; la URL pasa a ser `BaseUrl` (POST) / `$"{BaseUrl}/{id}"` (PUT), sin query string.

### `ProfileService.UpdateProfileAsync`

`Email` y `FechaNacimiento` (formato `yyyy-MM-dd`, mismo formato que ya se usaba en la query) siempre como `StringContent`; `Contraseña`/`ConfirmacionContraseña` solo se agregan si `contraseña` no es null/vacío (mismo comportamiento condicional que existe hoy). `FotoPerfil` sigue como archivo. URL pasa a ser `Url` sin query string.

## Fuera de alcance

- No se cambia el contrato del backend ni se abre issue en `Ecosena-Api` — el fix es enteramente del lado cliente, adaptándose al binding real (`[FromForm]`) que el backend ya usa.
- No se agrega logging/captura de status code o cuerpo de respuesta en los métodos de escritura — los mensajes de error actuales ("No se pudo crear/actualizar...") se mantienen igual; diagnosticar fallos futuros más específicos queda fuera de este spec.
- `ReportService.UpdateEstadoAsync` (avanzar estado) y `PenalizarAsync` no se tocan — no llevan DTO con campos de texto, ya funcionan.
- No se revisan otros posibles roles/vistas cacheadas más allá de Home/Report/footer — `BlogContainerView` y `ReportsAdminView`/`ReportsUserView` ya se recrean al limpiar `_cache` completo, y su lógica interna de admin (botones condicionados a `IUserSession.Role`) ya se evalúa dinámicamente vía binding, no en construcción.
