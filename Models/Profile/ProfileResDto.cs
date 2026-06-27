namespace EcosenaApp.Models.Profile;

public class ProfileResDto
{
    public string? FotoPerfil { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int? Ficha { get; set; }
    public string? Programa { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;
}
