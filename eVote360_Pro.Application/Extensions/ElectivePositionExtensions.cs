using eVote360_Pro.Application.DTOs.ElectivePosition.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class PositionExtensions
    {
        public static ElectivePositionResponse ToResponse(this ElectivePosition position)
        {
            var response = position.Adapt<ElectivePositionResponse>();

            return response with
            {
                IsImmutable = position.Votes.Any() || position.CandidatePostAssignments.Any(),
            };
        }

        public static IEnumerable<ElectivePositionResponse> ToResponse(
            this IEnumerable<ElectivePosition> positions
        )
        {
            return positions.Select(p => p.ToResponse());
        }
    }
}
