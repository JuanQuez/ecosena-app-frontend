using EcosenaApp.Models.Profile;

namespace EcosenaApp.Services.Profile;

public interface IProfileService
{
    Task<ProfileResDto?> GetProfileAsync();
    Task<bool> UpdateProfileAsync(string email, DateOnly? fechaNacimiento,
        string? contraseña, string? confirmacion, Stream? foto, string? fileName);
}
