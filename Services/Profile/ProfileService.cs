using EcosenaApp.Models.Profile;
using EcosenaApp.Services.Auth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EcosenaApp.Services.Profile;

public class ProfileService : IProfileService
{
    private readonly IAuthService _authService;
    private const string Url = "https://ecosena-api.onrender.com/api/Profile";

    public ProfileService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ProfileResDto?> GetProfileAsync()
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return null;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(Url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProfileResDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProfileService error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateProfileAsync(string email, DateOnly? fechaNacimiento,
        string? contraseña, string? confirmacion, Stream? foto, string? fileName)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

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
}
