namespace EcosenaApp.Models.Recovery;

public class GetResetCodeReq
{
    public string Documento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
