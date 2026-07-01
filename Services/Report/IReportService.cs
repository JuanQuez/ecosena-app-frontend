using EcosenaApp.Models.Report;

namespace EcosenaApp.Services.Report;

public interface IReportService
{
    Task<List<ReportListResDto>> GetAllReportsAsync();
    Task<List<ReportListResDto>> GetMyReportsAsync();
    Task<StatsReportDto?> GetEstadisticasAsync();
    Task<(byte[] Bytes, string FileName)?> ExportarExcelAsync();
    Task<ReportResDto?> GetReportAsync(int id);
    Task<ReportResDto?> PostReportAsync(string titulo, string descripcion, int idAmbiente, Stream? foto, string? fileName);
    Task<bool> UpdateEstadoAsync(int id);
    Task<bool> PenalizarAsync(int reporteId);
}
