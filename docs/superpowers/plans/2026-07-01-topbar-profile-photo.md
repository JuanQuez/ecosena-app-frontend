# Foto de perfil real en el ícono del TopBar — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** El ícono de perfil del `TopBarView` muestra la foto real del usuario autenticado en vez del SVG genérico estático, y se actualiza automáticamente cada vez que `HostPage` vuelve a ser visible (login, relogin con otro rol, volver de editar perfil).

**Architecture:** `TopBarView` gana un método público `RefreshProfilePhotoAsync()` que resuelve `IUserSession`/`IProfileService` por service-locator (mismo patrón que `OnProfileTapped` ya usa) y actualiza el `Source` de un `Image` nombrado dentro de un avatar circular. `HostPage` (que ya tiene un `OnAppearing()` propio desde el fix de rol reciente) llama a ese método de forma incondicional en cada aparición.

**Tech Stack:** .NET MAUI, `IProfileService`/`IUserSession` ya existentes (sin cambios en la capa de servicios).

## Global Constraints

- No hay framework de tests automatizados en este proyecto — la verificación es `dotnet build -f net10.0-android` compilando limpio, más un script de verificación manual explícito.
- Sin foto real disponible (Invitado, `FotoPerfil` vacío, o falla la llamada a `GetProfileAsync()`) el ícono cae de vuelta a `icon_profile.svg` en silencio — sin `Toast` ni ningún otro aviso al usuario, es un detalle visual no crítico.
- `ProfileService.GetProfileAsync()` ya captura sus propias excepciones y devuelve `null` en caso de fallo — no agregar un `try/catch` adicional en `TopBarView`.
- No se toca la lógica de `OnProfileTapped` (navegación a `ProfilePage`, prompt de login para Invitado).
- No se implementa nada relacionado con la foto del emisor de un reporte — quedó descartado en el spec por falta de soporte del backend real.

---

### Task 1: `TopBarView` — avatar circular + `RefreshProfilePhotoAsync()`

**Files:**
- Modify: `Views/Controls/TopBarView.xaml`
- Modify: `Views/Controls/TopBarView.xaml.cs`

**Interfaces:**
- Consumes: `IUserSession.IsAuthenticated` (ya existente), `IProfileService.GetProfileAsync()` → `Task<ProfileResDto?>` con propiedad `string? FotoPerfil` (`Models/Profile/ProfileResDto.cs`, ya existente).
- Produces: `TopBarView.RefreshProfilePhotoAsync()` — método público `Task RefreshProfilePhotoAsync()`, sin parámetros. Task 2 lo consume desde `HostPage`.

- [ ] **Step 1: Reemplazar el ícono de perfil en `TopBarView.xaml`**

El archivo actual completo es:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="EcosenaApp.Views.Controls.TopBarView">

    <Border VerticalOptions="Start"
            ZIndex="10"
            Padding="25,10"
            StrokeThickness=".1"
            Stroke="{StaticResource WhiteBase}"
            StrokeShape="RoundRectangle 40" 
            Background="{StaticResource InnerShadow}">
            

        <Grid ColumnDefinitions="Auto, *, Auto"
              VerticalOptions="Center">

            <HorizontalStackLayout Grid.Column="0"
                                   Spacing="12"
                                   VerticalOptions="Center">
                <Image Source="icon_logo.svg"
                       HeightRequest="30"
                       WidthRequest="30"
                       VerticalOptions="Center" />

                <Label Text="ECOSENA"
                       Style="{StaticResource H2}"
                       TextColor="{StaticResource WhiteBase}"
                       VerticalOptions="Center"
                       FontSize="18" />
            </HorizontalStackLayout>

            <Border Grid.Column="2"
                    HorizontalOptions="End"
                    VerticalOptions="Center">
                <Image Source="icon_profile.svg"
                       WidthRequest="35"
                       HorizontalOptions="Center"
                       VerticalOptions="Center">
                    <Image.Behaviors>
                        <toolkit:IconTintColorBehavior TintColor="{StaticResource WhiteBase}" />
                    </Image.Behaviors>
                    <Image.GestureRecognizers>
                        <TapGestureRecognizer Tapped="OnProfileTapped"/>
                    </Image.GestureRecognizers>
                </Image>
            </Border>
        </Grid>
    </Border>
</ContentView>
```

Reemplazar por (se quita `xmlns:toolkit`, ya no se usa en este archivo, y se reemplaza el `<Border Grid.Column="2">` completo):

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="EcosenaApp.Views.Controls.TopBarView">

    <Border VerticalOptions="Start"
            ZIndex="10"
            Padding="25,10"
            StrokeThickness=".1"
            Stroke="{StaticResource WhiteBase}"
            StrokeShape="RoundRectangle 40" 
            Background="{StaticResource InnerShadow}">
            

        <Grid ColumnDefinitions="Auto, *, Auto"
              VerticalOptions="Center">

            <HorizontalStackLayout Grid.Column="0"
                                   Spacing="12"
                                   VerticalOptions="Center">
                <Image Source="icon_logo.svg"
                       HeightRequest="30"
                       WidthRequest="30"
                       VerticalOptions="Center" />

                <Label Text="ECOSENA"
                       Style="{StaticResource H2}"
                       TextColor="{StaticResource WhiteBase}"
                       VerticalOptions="Center"
                       FontSize="18" />
            </HorizontalStackLayout>

            <Border Grid.Column="2"
                    HorizontalOptions="End"
                    VerticalOptions="Center"
                    WidthRequest="35"
                    HeightRequest="35"
                    StrokeThickness="0"
                    BackgroundColor="{StaticResource WhiteBase}">
                <Border.StrokeShape>
                    <Ellipse/>
                </Border.StrokeShape>
                <Image x:Name="ProfileImage"
                       Source="icon_profile.svg"
                       Aspect="AspectFill">
                    <Image.GestureRecognizers>
                        <TapGestureRecognizer Tapped="OnProfileTapped"/>
                    </Image.GestureRecognizers>
                </Image>
            </Border>
        </Grid>
    </Border>
</ContentView>
```

- [ ] **Step 2: Agregar `RefreshProfilePhotoAsync()` en `TopBarView.xaml.cs`**

El archivo actual completo es:

```csharp
using CommunityToolkit.Maui.Alerts;
using EcosenaApp.Services.Session;

namespace EcosenaApp.Views.Controls;

public partial class TopBarView : ContentView
{
    public TopBarView()
    {
        InitializeComponent();
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        var userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();
        if (userSession != null && !userSession.IsAuthenticated)
        {
            await Toast.Make("Ingresa a tu cuenta.").Show();
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // Se envuelve en un NavigationPage propio para aislar el push/pop de ProfilePage/EditProfilePage
        // de la pila de Shell: un bug no resuelto de MAUI Shell (dotnet/maui#21570) provoca
        // "Ambiguous routes matched" al empujar 2+ páginas directamente sobre Shell y luego hacer Pop.
        var profilePage = new Views.Profile.ProfilePage();
        NavigationPage.SetHasNavigationBar(profilePage, false);
        await Navigation.PushAsync(new NavigationPage(profilePage));
    }
}
```

Reemplazar por:

```csharp
using CommunityToolkit.Maui.Alerts;
using EcosenaApp.Services.Profile;
using EcosenaApp.Services.Session;

namespace EcosenaApp.Views.Controls;

public partial class TopBarView : ContentView
{
    public TopBarView()
    {
        InitializeComponent();
    }

    public async Task RefreshProfilePhotoAsync()
    {
        var userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();
        if (userSession == null || !userSession.IsAuthenticated)
        {
            ProfileImage.Source = "icon_profile.svg";
            return;
        }

        var profileService = IPlatformApplication.Current?.Services.GetService<IProfileService>();
        var profile = profileService != null ? await profileService.GetProfileAsync() : null;

        ProfileImage.Source = !string.IsNullOrEmpty(profile?.FotoPerfil)
            ? ImageSource.FromUri(new Uri(profile.FotoPerfil))
            : "icon_profile.svg";
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        var userSession = IPlatformApplication.Current?.Services.GetService<IUserSession>();
        if (userSession != null && !userSession.IsAuthenticated)
        {
            await Toast.Make("Ingresa a tu cuenta.").Show();
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // Se envuelve en un NavigationPage propio para aislar el push/pop de ProfilePage/EditProfilePage
        // de la pila de Shell: un bug no resuelto de MAUI Shell (dotnet/maui#21570) provoca
        // "Ambiguous routes matched" al empujar 2+ páginas directamente sobre Shell y luego hacer Pop.
        var profilePage = new Views.Profile.ProfilePage();
        NavigationPage.SetHasNavigationBar(profilePage, false);
        await Navigation.PushAsync(new NavigationPage(profilePage));
    }
}
```

- [ ] **Step 3: Compilar**

Run: `dotnet build -f net10.0-android`
Expected: `Build succeeded.` sin errores en `TopBarView.xaml`/`TopBarView.xaml.cs`.

- [ ] **Step 4: Verificación manual**

Este método aún no lo llama nadie (eso es Task 2) — para probarlo de forma aislada, agregar temporalmente una línea `_ = RefreshProfilePhotoAsync();` al final del constructor `TopBarView()`, correr `dotnet run -f net10.0-android`, entrar a la app y:

1. Como Invitado: el ícono se ve igual que hoy (SVG genérico dentro del círculo blanco).
2. Loguear con una cuenta que ya tenga `FotoPerfil` configurado (verificar en Editar Perfil que tenga una foto subida): el ícono del TopBar debe cambiar a esa foto real, recortada en círculo.
3. Loguear con una cuenta sin foto de perfil: el ícono se mantiene en el SVG genérico.

Quitar la línea temporal del constructor antes de continuar a Task 2 (la llamada real se agrega ahí, desde `HostPage`).

- [ ] **Step 5: Commit**

```bash
git add "Views/Controls/TopBarView.xaml" "Views/Controls/TopBarView.xaml.cs"
git commit -m "feat: TopBarView muestra la foto real de perfil del usuario autenticado"
```

---

### Task 2: `HostPage` — refrescar la foto del TopBar en cada aparición

**Files:**
- Modify: `Views/Host/HostPage.xaml`
- Modify: `Views/Host/HostPage.xaml.cs:25-34`

**Interfaces:**
- Consumes: `TopBarView.RefreshProfilePhotoAsync()` (Task 1, `Task RefreshProfilePhotoAsync()`, sin parámetros).
- Produces: nada consumido por otras tareas — es la última del plan.

- [ ] **Step 1: Nombrar el `TopBarView` en `HostPage.xaml`**

La línea actual (`Views/Host/HostPage.xaml`) es:

```xml
        <controls:TopBarView Grid.Row="0" />
```

Reemplazar por:

```xml
        <controls:TopBarView x:Name="MainTopBar" Grid.Row="0" />
```

- [ ] **Step 2: Llamar `RefreshProfilePhotoAsync()` en `OnAppearing()`**

El método actual (`Views/Host/HostPage.xaml.cs:25-34`) es:

```csharp
    protected override void OnAppearing()
    {
        base.OnAppearing();

        var role = _userSession?.Role ?? "Invitado";
        if (role != _lastRole)
        {
            ApplyRole();
        }
    }
```

Reemplazar por:

```csharp
    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = MainTopBar.RefreshProfilePhotoAsync();

        var role = _userSession?.Role ?? "Invitado";
        if (role != _lastRole)
        {
            ApplyRole();
        }
    }
```

El resto del archivo (`ApplyRole()`, `OnFooterSelectionChanged`, `ShowSection`, etc.) queda sin cambios.

- [ ] **Step 3: Compilar**

Run: `dotnet build -f net10.0-android`
Expected: `Build succeeded.` sin errores en `HostPage.xaml`/`HostPage.xaml.cs`.

- [ ] **Step 4: Verificación manual**

Con `dotnet run -f net10.0-android`:

1. Loguear con una cuenta que tenga `FotoPerfil` configurado → el ícono del TopBar debe mostrar esa foto apenas se carga `HostPage` (sin necesidad de tocar nada más).
2. Ir a Perfil → Editar perfil → cambiar la foto → Guardar → volver (botón atrás) hasta `HostPage` → el ícono del TopBar debe reflejar la foto nueva, sin cerrar sesión.
3. Cerrar sesión y entrar como Invitado → el ícono vuelve al SVG genérico.
4. Cerrar sesión y loguear con una cuenta distinta (rol distinto) → el ícono muestra la foto de la cuenta nueva, no la de la anterior.

- [ ] **Step 5: Commit**

```bash
git add "Views/Host/HostPage.xaml" "Views/Host/HostPage.xaml.cs"
git commit -m "feat: refrescar la foto del TopBar en cada aparición de HostPage"
```
