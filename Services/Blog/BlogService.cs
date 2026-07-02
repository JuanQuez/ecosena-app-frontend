using EcosenaApp.Models.Blog;
using EcosenaApp.Services.Http;
using System.Text.Json;

namespace EcosenaApp.Services.Blog;

public class BlogService : IBlogService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string BaseUrl = "https://ecosena-api.onrender.com/api/Blog";

    public BlogService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<BlogListResDto>> GetEntradasAsync(string? titulo = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);
            var response = await client.GetAsync(BaseUrl);
            if (!response.IsSuccessStatusCode)
                return new List<BlogListResDto>();

            var json = await response.Content.ReadAsStringAsync();
            var entradas = JsonSerializer.Deserialize<List<BlogListResDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BlogListResDto>();

            if (string.IsNullOrWhiteSpace(titulo))
                return entradas;

            // El backend ignora el query param "titulo" (no lo implementa pese a documentarlo);
            // se filtra en el cliente para que la búsqueda funcione igual.
            return entradas
                .Where(e => e.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase))
                .ToList();
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
            var client = _httpClientFactory.CreateClient(HttpClientNames.Authenticated);
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
}
