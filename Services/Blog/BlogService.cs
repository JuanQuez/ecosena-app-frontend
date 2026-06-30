using EcosenaApp.Models.Blog;
using EcosenaApp.Services.Auth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EcosenaApp.Services.Blog;

public class BlogService : IBlogService
{
    private readonly IAuthService _authService;
    private const string BaseUrl = "https://ecosena-api.onrender.com/api/Blog";

    public BlogService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<BlogListResDto>> GetEntradasAsync(string? titulo = null)
    {
        try
        {
            using var client = await CreateClientAsync();
            var url = string.IsNullOrWhiteSpace(titulo)
                ? BaseUrl
                : $"{BaseUrl}?titulo={Uri.EscapeDataString(titulo)}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<BlogListResDto>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BlogListResDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BlogListResDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.GetEntradasAsync error: {ex.Message}");
            return new List<BlogListResDto>();
        }
    }

    public async Task<EntradaResDto?> GetEntradaAsync(int id)
    {
        try
        {
            using var client = await CreateClientAsync();
            var response = await client.GetAsync($"{BaseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EntradaResDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BlogService.GetEntradaAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<EntradaResDto?> PostEntradaAsync(string titulo, string contenido, Stream? portada, string? fileName)
    {
        try
        {
            using var client = await CreateClientAsync();
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
            using var client = await CreateClientAsync();
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
            using var client = await CreateClientAsync();
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

    private async Task<HttpClient> CreateClientAsync()
    {
        var client = new HttpClient();
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
