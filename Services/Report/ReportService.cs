using EcosenaApp.Models.Report;
using EcosenaApp.Services.Auth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EcosenaApp.Services.Report;

public class ReportService : IReportService
{
    private readonly IAuthService _authService;
    private const string BaseUrl = "https://ecosena-api.onrender.com/api/Report";

    public ReportService(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<List<ReportListResDto>> GetAllReportsAsync() => GetListAsync($"{BaseUrl}/AllReports");

    public Task<List<ReportListResDto>> GetMyReportsAsync() => GetListAsync($"{BaseUrl}/Reports");

    private async Task<List<ReportListResDto>> GetListAsync(string url)
    {
        try
        {
            using var client = await CreateClientAsync();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<ReportListResDto>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ReportListResDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ReportListResDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ReportService.GetListAsync error: {ex.Message}");
            return new List<ReportListResDto>();
        }
    }

    public async Task<ReportResDto?> GetReportAsync(int id)
    {
        try
        {
            using var client = await CreateClientAsync();
            var response = await client.GetAsync($"{BaseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReportResDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ReportService.GetReportAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<ReportResDto?> PostReportAsync(string titulo, string descripcion, int idAmbiente, Stream? foto, string? fileName)
    {
        try
        {
            using var client = await CreateClientAsync();
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

    public async Task<bool> UpdateEstadoAsync(int id)
    {
        try
        {
            using var client = await CreateClientAsync();
            var response = await client.PutAsync($"{BaseUrl}/{id}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ReportService.UpdateEstadoAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> PenalizarAsync(int reporteId)
    {
        try
        {
            using var client = await CreateClientAsync();
            var response = await client.DeleteAsync($"{BaseUrl}/{reporteId}/penalizar");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ReportService.PenalizarAsync error: {ex.Message}");
            return false;
        }
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
