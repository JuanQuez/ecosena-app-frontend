namespace EcosenaApp.Models.Recovery;

public class ResetPasswordReq
{
    public string Codigo { get; set; } = string.Empty;
    public string NuevaContraseña { get; set; } = string.Empty;
    public string ConfirmacionContraseña { get; set; } = string.Empty;
}
