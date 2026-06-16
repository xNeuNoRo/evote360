using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class PoliticalLeaderAssignmentExtensions
    {
        public static LeaderAssignmentResponse ToResponse(this PoliticalLeaderAssignment assignment)
        {
            return assignment.Adapt<LeaderAssignmentResponse>();
        }

        public static IEnumerable<LeaderAssignmentResponse> ToResponse(this IEnumerable<PoliticalLeaderAssignment> assignments)
        {
            return assignments.Select(a => a.ToResponse());
        }
    }
}
