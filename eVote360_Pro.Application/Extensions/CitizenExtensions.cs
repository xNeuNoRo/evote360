using eVote360_Pro.Application.DTOs.Citizen.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class CitizenExtensions
    {
        /// <summary>
        /// Mapea un ciudadano a su DTO de salida, integrando banderas de estado electoral.
        /// </summary>
        /// <param name="citizen">La entidad ciudadano.</param>
        /// <param name="activeElectionId">ID de la elección activa actual para validar el voto.</param>
        /// <returns>DTo detallado del ciudadano.</returns>
        public static CitizenResponse ToResponse(
            this Citizen citizen,
            Guid? activeElectionId = null
        )
        {
            var response = citizen.Adapt<CitizenResponse>();

            bool hasVotedNow =
                activeElectionId.HasValue
                && citizen.Participations.Any(p => p.ElectionId == activeElectionId.Value);

            bool hasHistory = citizen.Participations.Any();

            return response with
            {
                FullName = $"{citizen.FirstName} {citizen.LastName}",
                HasVotedInActiveElection = hasVotedNow,
                IsIdentityDocumentImmutable = hasHistory,
            };
        }

        public static IEnumerable<CitizenResponse> ToResponse(
            this IEnumerable<Citizen> citizens,
            Guid? activeElectionId = null
        )
        {
            return citizens.Select(c => c.ToResponse(activeElectionId));
        }
    }
}
