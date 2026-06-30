namespace EcosenaApp.Models.Blog;

public class BlogListResDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Portada { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public string NombreRedactor { get; set; } = string.Empty;
}
