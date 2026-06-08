using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class LeaderAssignmentExtensions
    {
        public static LeaderAssignmentResponse ToResponse(this PoliticalLeaderAssignment assignment)
        {
            var response = assignment.Adapt<LeaderAssignmentResponse>();

            return response with
            {
                UserName = assignment.User?.Username ?? "N/A",
                UserFullName =
                    assignment.User != null
                        ? $"{assignment.User.FirstName} {assignment.User.LastName}"
                        : "N/A",
                PartyName = assignment.Party?.Name ?? "N/A",
                PartyAcronym = assignment.Party?.Acronym ?? "N/A",
            };
        }

        public static IEnumerable<LeaderAssignmentResponse> ToResponse(
            this IEnumerable<PoliticalLeaderAssignment> assignments
        )
        {
            return assignments.Select(a => a.ToResponse());
        }
    }
}
