using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Application.Extensions
{
    public static class BallotExtensions
    {
        /// <summary>
        /// Proyecta la configuración electoral actual en la estructura de la boleta digital.
        /// </summary>
        public static VoterBallotResponse ToVoterBallot(
            this Election election,
            List<BallotPositionResponse> positions
        )
        {
            return new VoterBallotResponse(election.Id, election.Name, positions);
        }

        /// <summary>
        /// Mapea un cargo y sus candidatos asignados a una sección de la boleta.
        /// </summary>
        public static BallotPositionResponse ToBallotPosition(
            this ElectivePosition position,
            List<BallotCandidateResponse> candidates
        )
        {
            return new BallotPositionResponse(
                position.Id,
                position.Name,
                position.Description,
                candidates
            );
        }
    }
}
