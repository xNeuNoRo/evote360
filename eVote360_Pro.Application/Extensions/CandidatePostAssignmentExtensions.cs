using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class CandidatePostAssignmentExtensions
    {
        public static BallotAssignmentResponse ToResponse(this CandidatePostAssignment assignment)
        {
            return assignment.Adapt<BallotAssignmentResponse>();
        }

        public static IEnumerable<BallotAssignmentResponse> ToResponse(this IEnumerable<CandidatePostAssignment> assignments)
        {
            return assignments.Select(a => a.ToResponse());
        }
    }
}
