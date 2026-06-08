using eVote360_Pro.Application.DTOs.Voting.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Proporciona los indicadores y estadísticas clave para las pantallas de HOme.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Obtiene el reporte de resultados para el año electoral seleccionado
        /// </summary>
        Task<ResultReportResponse?> GetAdminDashboardAsync(int electoralYear);

        /// <summary>
        /// Obtiene un resumen del estado del partido para el Dirigente.
        /// </summary>
        Task<ResultReportResponse?> GetLeaderDashboardAsync();
    }
}
