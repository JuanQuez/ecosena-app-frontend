# 🌱 EcosenaApp

Aplicación móvil multiplataforma para la gestión de **reportes de incidentes ambientales** del SENA (Servicio Nacional de Aprendizaje, Colombia). Construida con **.NET MAUI**, permite a los aprendices reportar novedades ambientales en su entorno de formación y a los administradores hacerles seguimiento, además de gestionar un blog institucional de contenido ambiental.

---

## 📱 ¿Qué hace la app?

- **📝 Reportes ambientales:** los aprendices crean reportes con foto, descripción y ubicación (una de 12 zonas/ambientes predefinidos del centro de formación). Los administradores los visualizan, les hacen seguimiento y pueden avanzar su estado (`Pendiente → En progreso → Resuelto`) o penalizar al emisor en caso de reportes indebidos.
- **📰 Blog institucional:** entradas informativas sobre temas ambientales, con imagen de portada. Los administradores pueden crear, editar y eliminar entradas; todos los usuarios pueden leerlas.
- **📊 Panel de estadísticas:** los administradores ven porcentajes globales de reportes pendientes/en progreso/resueltos, y pueden exportar los reportes del mes a Excel.
- **👤 Perfil de usuario:** edición de datos personales, foto de perfil, correo, fecha de nacimiento y contraseña.
- **🔐 Roles diferenciados:** la app adapta la navegación y el contenido según el rol del usuario autenticado — `Administrador`, `Aprendiz`, `Penalizado` (con acceso restringido a reportes) e `Invitado` (modo lectura, sin pestaña de reportes).
- **📶 Detección de conectividad:** banner flotante que avisa cuando el dispositivo pierde o recupera la conexión a internet.

---

## 🛠️ Tecnologías

| Tecnología | Uso |
|---|---|
| **.NET MAUI** (net10.0) | Framework multiplataforma (Android, iOS, macOS Catalyst, Windows) |
| **CommunityToolkit.Mvvm** | Patrón MVVM con source generators |
| **CommunityToolkit.Maui** | Toasts, comportamientos de UI (tintado de íconos, etc.) |
| **C# / XAML** | Lógica de negocio y UI declarativa |
| **REST API** (ASP.NET Core, backend externo) | Persistencia de datos, autenticación JWT |

> El backend consumido por esta app es [Ecosena-Api](https://github.com/JoseDRestrepo/Ecosena-Api), desplegado en `https://ecosena-api.onrender.com/api/`.

---

## ✅ Requisitos previos

Antes de correr el proyecto en local necesitas tener instalado:

1. **[Visual Studio 2022](https://visualstudio.microsoft.com/vs/)** (versión 17.12 o superior) con la carga de trabajo **".NET Multi-platform App UI development"**, **o** el **[.NET 10 SDK](https://dotnet.microsoft.com/download)** si prefieres usar solo la terminal.
2. Los **workloads de MAUI** instalados vía CLI (si no usas el instalador de Visual Studio):
   ```bash
   dotnet workload install maui
   ```
3. Para correr en **Android**: un emulador configurado (Android Device Manager en Visual Studio, o Android Studio + AVD) o un dispositivo físico con depuración USB habilitada.
4. Para correr en **Windows**: no se necesita nada adicional, el modo `net10.0-windows` corre de forma nativa (empaquetado sin MSIX, `WindowsPackageType=None`).
5. Conexión a internet — la app consume el backend real en la nube, no hay modo offline con datos simulados.

> ℹ️ No es necesario levantar el backend en local: la app apunta directamente a la API ya desplegada (`https://ecosena-api.onrender.com/api/`).

---

## 🚀 Cómo correr el proyecto

### Opción 1 — Visual Studio 2022

1. Clona el repositorio y abre `EcosenaApp.slnx` (o `EcosenaApp.csproj`) en Visual Studio.
2. Espera a que se restauren los paquetes NuGet automáticamente.
3. En la barra de herramientas, selecciona el **framework de destino** (`net10.0-android`, `net10.0-windows...`, etc.) y el dispositivo/emulador donde quieres correr la app.
4. Presiona **▶ Run** (F5).

### Opción 2 — Línea de comandos

```bash
# Restaurar dependencias
dotnet restore

# Compilar para todos los targets soportados por tu sistema operativo
dotnet build

# Compilar para una plataforma específica
dotnet build -f net10.0-android
dotnet build -f net10.0-windows10.0.19041.0

# Ejecutar directamente en un emulador/dispositivo Android
dotnet run -f net10.0-android

# Ejecutar en Windows
dotnet run -f net10.0-windows10.0.19041.0
```

> 🧪 El proyecto no cuenta con pruebas automatizadas (no hay suite de tests configurada).

---

## 👥 Usuarios demo

La app se conecta al backend real en producción, así que puedes iniciar sesión de inmediato con las siguientes cuentas de prueba:

### 🎓 Usuario Aprendiz
Puede crear y consultar sus propios reportes ambientales, y leer el blog.

| Campo | Valor |
|---|---|
| **Documento** | `1000938619` |
| **Contraseña** | `juancho2405` |

### 🛡️ Usuario Administrador
Puede gestionar todos los reportes (avanzar estado, penalizar), administrar el blog (crear/editar/eliminar entradas) y ver estadísticas + exportar reportes a Excel.

| Campo | Valor |
|---|---|
| **Documento** | `1003627561` |
| **Contraseña** | `diana1234` |

> 💡 También puedes explorar la app como **Invitado** desde la pantalla de login, con acceso de solo lectura (sin la pestaña de Reportar).

---

## 🧭 Recorrido rápido por la app

1. **Splash → Login:** pantalla de bienvenida y formulario de inicio de sesión por número de documento y contraseña.
2. **Home:** portada dinámica según el rol, sección de blog reciente y noticias de interés SENA.
3. **Reportar:** (oculto para Invitado) formulario para crear un reporte con foto, descripción y ubicación — o, si eres Administrador, el listado completo con seguimiento y estadísticas.
4. **Blog:** listado de entradas ambientales, con creación/edición exclusiva para Administrador.
5. **Perfil:** datos personales, edición de perfil, cierre de sesión y accesos a soporte/privacidad/términos.

---

## 🏗️ Estructura del proyecto

```
EcosenaApp/
├── Models/          # DTOs de request/response del backend
├── Services/         # Capa de servicios HTTP (Auth, Blog, Report, Profile, Recovery)
├── ViewModels/       # Lógica de presentación (MVVM, CommunityToolkit.Mvvm)
├── Views/            # Páginas y controles XAML
│   ├── Auth/          # Login, registro, recuperación de contraseña
│   ├── Blog/          # Entradas del blog
│   ├── Host/           # HostPage: shell principal con tabs
│   └── Controls/       # Componentes reutilizables por rol/sección
├── Helpers/          # Datos hardcodeados (ambientes, noticias) y utilidades
└── Resources/        # Estilos, colores, fuentes, imágenes, spec OpenAPI
```

Arquitectura basada en **MVVM** con inyección de dependencias vía `MauiProgram.cs` y `IHttpClientFactory` para el consumo de la API. Más detalle en `CLAUDE.md`.

---

## 🌐 Backend

- **Base URL:** `https://ecosena-api.onrender.com/api/`
- **Código fuente del backend:** [JoseDRestrepo/Ecosena-Api](https://github.com/JoseDRestrepo/Ecosena-Api)
- **Documentación OpenAPI:** `Resources/Data/v1.json` (nota: puede estar desactualizada respecto al backend real desplegado)

---

## 🎨 Diseño

- **Color primario:** `#174F42` (verde oscuro institucional)
- **Tipografía:** Inter (6 pesos)
- Tokens de color y estilos reutilizables en `Resources/Styles/Colors.xaml` y `Resources/Styles/Styles.xaml`

---

**Hecho con 💚 para SENA — Servicio Nacional de Aprendizaje**
