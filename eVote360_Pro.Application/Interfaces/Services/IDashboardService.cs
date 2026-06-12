using eVote360_Pro.Application.DTOs.Voting.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Proporciona los indicadores y estadísticas clave para las pantallas de inicio.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Obtiene estadísticas generales del sistema (Totales).
        /// </summary>
        Task<DashboardStatisticsResponse> GetGeneralStatisticsAsync();

        /// <summary>
        /// Obtiene el reporte de resultados para el año electoral seleccionado en el Home del Admin.
        /// (Solo para elecciones finalizadas).
        /// </summary>
        Task<ResultReportResponse?> GetAdminDashboardAsync(int electoralYear);

        /// <summary>
        /// Obtiene un resumen del estado de la elección activa para el Home del Dirigente.
        /// </summary>
        Task<ResultReportResponse?> GetLeaderDashboardAsync();

        /// <summary>
        /// Obtiene las estadísticas para el dashboard del Dirigente (Candidatos, Alianzas, etc.).
        /// </summary>
        Task<LeaderDashboardStatisticsResponse> GetLeaderStatisticsAsync();
    }

    public record DashboardStatisticsResponse(
        int TotalElections,
        string? ActiveElectionName,
        int VoterParticipationCount,
        int TotalCitizens,
        int TotalParties,
        List<RecentCitizenDto> RecentCitizens,
        List<RecentPartyDto> RecentParties
    );

    public record RecentCitizenDto(string FullName, string IdentityDocument, string CreatedAt);
    public record RecentPartyDto(string Name, string Acronym, string LogoPath);

    public record LeaderDashboardStatisticsResponse(
        int ActiveCandidates,
        int InactiveCandidates,
        int ApprovedAlliances,
        int PendingAllianceRequestsReceived,
        int AssignedCandidates,
        List<RecentCandidateDto> RecentCandidates,
        List<AlliedPartyDto> AlliedParties
    );

    public record RecentCandidateDto(string FullName, string PositionName, string PhotoPath);
    public record AlliedPartyDto(string Name, string Acronym, string LogoPath);
}
