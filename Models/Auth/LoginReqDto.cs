namespace EcosenaApp.Models.Auth;

public class LoginReqDto
{
    public string Documento { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
}
