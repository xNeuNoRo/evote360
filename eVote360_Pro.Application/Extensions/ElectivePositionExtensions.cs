using eVote360_Pro.Application.DTOs.ElectivePosition.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class ElectivePositionExtensions
    {
        public static ElectivePositionResponse ToResponse(this ElectivePosition position)
        {
            return position.Adapt<ElectivePositionResponse>();
        }

        public static IEnumerable<ElectivePositionResponse> ToResponse(this IEnumerable<ElectivePosition> positions)
        {
            return positions.Select(p => p.ToResponse());
        }
    }
}
