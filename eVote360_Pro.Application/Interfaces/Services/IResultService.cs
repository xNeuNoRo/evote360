using eVote360_Pro.Application.DTOs.Voting.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Calcula y reporta los resultados de los procesos electorales.
    /// </summary>
    public interface IResultService
    {
        /// <summary>
        /// Genera el reporte consolidado de una elección finalizada.
        /// Incluye porcentajes y detección de empates.
        /// </summary>
        Task<ResultReportResponse> GetReportAsync(Guid electionId);

        /// <summary>
        /// Proporciona datos rápidos para los Dashboards de la pantalla de inicio.
        /// </summary>
        Task<ResultReportResponse?> GetDashboardSummaryAsync();
    }
}
