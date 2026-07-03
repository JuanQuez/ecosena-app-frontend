namespace EcosenaApp.Helpers;

public record SenaNoticia(string Titulo, string Url);

public static class SenaNoticiasData
{
    public static readonly List<SenaNoticia> Lista = new()
    {
        new("Residuos Peligrosos", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/07/disposicion-correcta-de-residuos.html"),
        new("Convocatoria Diseño", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/07/potencie-sus-competencias-creativas-y.html"),
        new("Jornada Fumigación", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/07/jornada-de-fumigacion.html"),
        new("Cursos Ofimática", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/06/cursos-virtual-2026-en-ofimatica-y.html"),
        new("Minuto Verde", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/06/minuto-verde-dia-mundial-del-arbol.html"),
        new("Ahorra Energía", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/06/tips-para-ahorrar-energia.html"),
        new("Feria Tecnológica", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/06/ii-feria-tecnologica-ctma.html"),
        new("Diversidad Inclusión", "https://tecnologia-manufactura-avanzada.blogspot.com/2026/06/conferencia-diversidad-de-genero-e.html"),
    };
}
