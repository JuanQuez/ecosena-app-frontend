using EcosenaApp.Models.Auth;
using EcosenaApp.Services.Http;
using System.Text;
using System.Text.Json;

namespace EcosenaApp.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string TokenKey = "auth_token";
    private const string BaseUrl = "https://ecosena-api.onrender.com/api/Auth";

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LoginResDto?> LoginAsync(string documento, string contraseña)
    {
        try
        {
            var request = new LoginReqDto
            {
                Documento = documento,
                Contraseña = contraseña
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient(HttpClientNames.Anonymous);
            var response = await client.PostAsync($"{BaseUrl}/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<LoginResDto>(responseContent, options);

                if (result?.Jwt != null)
                {
                    await SaveTokenAsync(result.Jwt);
                    return result;
                }

                return null;
            }

            // El backend devuelve 423 (bloqueado) o 400 (credenciales) con distintos
            // subconjuntos de campos; se deserializa igual y se marca IsLocked por status code
            // porque ese campo no viene en el cuerpo de error.
            var error = JsonSerializer.Deserialize<LoginResDto>(responseContent, options);
            if (error != null)
                error.IsLocked = response.StatusCode == System.Net.HttpStatusCode.Locked;
            return error;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> RegisterAsync(RegisterReqDto request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient(HttpClientNames.Anonymous);
            var response = await client.PostAsync($"{BaseUrl}/register", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task LogoutAsync()
    {
        try
        {
            SecureStorage.Remove(TokenKey);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving token: {ex.Message}");
        }
    }
}
