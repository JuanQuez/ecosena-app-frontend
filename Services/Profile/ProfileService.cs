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
}
