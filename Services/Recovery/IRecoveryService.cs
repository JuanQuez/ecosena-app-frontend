namespace EcosenaApp.Services.Recovery;

public interface IRecoveryService
{
    Task<bool> SolicitarCodigoAsync(string documento, string email);
    Task<bool> ReestablecerContraseñaAsync(string codigo, string nuevaContraseña, string confirmacion);
}
