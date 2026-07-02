# Foto de perfil real en el ícono del TopBar — Design

## Contexto

El ícono de perfil del `TopBarView` (esquina superior derecha, usado para navegar a `ProfilePage`) es hoy un SVG genérico estático (`icon_profile.svg`), tinteado de blanco vía `IconTintColorBehavior`, sin relación con el usuario autenticado. Se pidió que muestre la foto real del usuario que inició sesión.

**Fuera de alcance (descartado durante brainstorming):** también se pidió mostrar la foto de perfil del emisor de un reporte en la vista de seguimiento/gestión (`ReportManagementAdminView`, solo Administrador). Verificado contra el backend real (`JoseDRestrepo/Ecosena-Api`): `ReportResDto` (`GET /api/Report/{id}`) solo trae `EmisorReporte` (nombre, string) y `Foto` (la foto del incidente, no la del emisor); no existe ningún endpoint donde un Administrador pueda consultar el perfil de otro usuario por id — se revisaron los 5 controllers reales del backend y ninguno lo expone. No es viable sin un cambio de backend (fuera de este repo), así que se descarta por completo de este spec.

## Diseño

### Visual

`TopBarView.xaml` reemplaza el `Image` plano + `IconTintColorBehavior` actual por el mismo patrón de avatar circular que ya usa `ProfilePage` (`Border` con `StrokeShape="Ellipse"`, sin tint, `Image` con `Aspect="AspectFill"`):

```xml
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
```

- Fondo `WhiteBase` circular detrás del ícono/foto, para que haya contraste contra el verde oscuro del TopBar tanto con el SVG genérico como con una foto real.
- `Source="icon_profile.svg"` queda como valor por defecto en el XAML (mismo archivo que ya se usa hoy, y el mismo que `ProfileViewModel` usa como fallback en `ProfilePage`) — se sobreescribe desde code-behind solo cuando hay una foto real que mostrar.
- El gesto de tap (`OnProfileTapped`, navega a `ProfilePage` o pide login si es Invitado) no cambia.

### Obtención y refresco de la foto

`TopBarView` gana un método público:

```csharp
public async Task RefreshProfilePhotoAsync()
```

Lógica:
1. Resuelve `IUserSession` por service-locator (mismo patrón que ya usa `OnProfileTapped`).
2. Si `!userSession.IsAuthenticated` (rol `Invitado`) → `ProfileImage.Source = "icon_profile.svg"`, retorna sin llamar al backend.
3. Si autenticado → resuelve `IProfileService` y llama `GetProfileAsync()`.
4. Si la llamada falla (`null`) o `FotoPerfil` viene vacío → `ProfileImage.Source = "icon_profile.svg"` (fallback silencioso, sin `Toast` — es un detalle visual, no bloquea ningún flujo).
5. Si hay `FotoPerfil` → `ProfileImage.Source = ImageSource.FromUri(new Uri(profile.FotoPerfil))`.

`ProfileService.GetProfileAsync()` ya captura sus propias excepciones internamente y devuelve `null` en caso de fallo (ver `Services/Profile/ProfileService.cs`), así que `RefreshProfilePhotoAsync()` no necesita su propio `try/catch` adicional — solo comprobar `null`/vacío.

### Cuándo se dispara el refresco

`HostPage.xaml` le agrega `x:Name="MainTopBar"` al `TopBarView` (hoy no tiene nombre asignado). `HostPage.OnAppearing()` (ya existente, agregado en el fix de rol reciente) gana una llamada fire-and-forget **incondicional** — no depende del chequeo de cambio de rol que ya hace `ApplyRole()`:

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

`OnAppearing()` de una `ContentPage` de Shell se dispara automáticamente cada vez que la página vuelve a ser la visible en la pila de navegación — esto ya cubre, sin necesidad de ningún hook adicional:
- Primer login, guest→login, logout→login con otro rol (casos que además disparan `ApplyRole()`).
- Volver de `ProfilePage`/`EditProfilePage` (pop de esa `NavigationPage` empujada desde `TopBarView.OnProfileTapped`) — cubre el caso de "edité mi foto y quiero verla reflejada sin re-loguearme", pedido explícitamente durante el brainstorming.

## Fuera de alcance

- Foto del emisor de un reporte en `ReportManagementAdminView` — descartado, requiere cambio de backend (ver Contexto).
- Cache local de la foto entre refrescos (p. ej. evitar la llamada HTTP si `FotoPerfil` no cambió) — cada `OnAppearing()` vuelve a pedir el perfil completo; es la misma frecuencia con la que `ProfilePage.OnAppearing()` ya lo hace hoy, así que no es una llamada nueva en términos de patrón, solo se agrega en un punto más (el TopBar).
- Manejo de error visible al usuario si la foto no carga — fallback silencioso al ícono genérico, consistente con que esto es un detalle visual no crítico.
