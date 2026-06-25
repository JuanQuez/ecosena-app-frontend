using EcosenaApp.Models.Auth;

namespace EcosenaApp.Services.Auth;

public interface IAuthService
{
    Task<LoginResDto?> LoginAsync(string documento, string contraseña);
    Task<bool> RegisterAsync(RegisterReqDto request);
    Task<bool> IsAuthenticatedAsync();
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
}
