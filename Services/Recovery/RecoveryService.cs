using EcosenaApp.Models.Recovery;
using EcosenaApp.Services.Http;
using System.Text;
using System.Text.Json;

namespace EcosenaApp.Services.Recovery;

public class RecoveryService : IRecoveryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string BaseUrl = "https://ecosena-api.onrender.com/api/Recovery";

    public RecoveryService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SolicitarCodigoAsync(string documento, string email)
    {
        try
        {
            var request = new GetResetCodeReq { Documento = documento, Email = email };
            var client = _httpClientFactory.CreateClient(HttpClientNames.Anonymous);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{BaseUrl}/solicitar", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RecoveryService.SolicitarCodigoAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ReestablecerContraseñaAsync(string codigo, string nuevaContraseña, string confirmacion)
    {
        try
        {
            var request = new ResetPasswordReq
            {
                Codigo = codigo,
                NuevaContraseña = nuevaContraseña,
                ConfirmacionContraseña = confirmacion
            };
            var client = _httpClientFactory.CreateClient(HttpClientNames.Anonymous);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{BaseUrl}/reestablecer", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RecoveryService.ReestablecerContraseñaAsync error: {ex.Message}");
            return false;
        }
    }
}
