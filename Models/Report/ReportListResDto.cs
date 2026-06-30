namespace EcosenaApp.Models.Report;

public class ReportListResDto
{
    public int Id { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public EstadoReporte Estado { get; set; }
    public DateTime FechaEmision { get; set; }
}
