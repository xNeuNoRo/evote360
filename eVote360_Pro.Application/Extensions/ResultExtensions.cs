using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class ResultExtensions
    {
        /// <summary>
        /// Construye el reporte de resultados para una elección.
        /// </summary>
        public static ResultReportResponse ToResultReport(
            this Election election,
            int totalVoters,
            List<PositionResultResponse> positionResults
        )
        {
            return new ResultReportResponse(
                election.Id,
                election.Name,
                totalVoters,
                positionResults
            );
        }

        /// <summary>
        /// Mapea los resultados agrupados de una posición a su DTO de salida.
        /// </summary>
        public static PositionResultResponse ToPositionResult(
            this ElectivePosition position,
            bool isTie,
            List<CandidateResultResponse> candidates
        )
        {
            return new PositionResultResponse(position.Id, position.Name, isTie, candidates);
        }
    }
}
