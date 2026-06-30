using EcosenaApp.Models.Blog;

namespace EcosenaApp.Services.Blog;

public interface IBlogService
{
    Task<List<BlogListResDto>> GetEntradasAsync(string? titulo = null);
    Task<EntradaResDto?> GetEntradaAsync(int id);
    Task<EntradaResDto?> PostEntradaAsync(string titulo, string contenido, Stream? portada, string? fileName);
    Task<bool> PutEntradaAsync(int id, string titulo, string contenido, Stream? portada, string? fileName);
    Task<bool> DeleteEntradaAsync(int id);
}
