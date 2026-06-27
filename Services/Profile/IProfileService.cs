using EcosenaApp.Models.Profile;

namespace EcosenaApp.Services.Profile;

public interface IProfileService
{
    Task<ProfileResDto?> GetProfileAsync();
}
