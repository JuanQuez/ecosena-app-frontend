namespace EcosenaApp.Models.Auth;

public class RegisterReqDto
{
    public string Documento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
    public string ConfirmacionContraseña { get; set; } = string.Empty;
}
