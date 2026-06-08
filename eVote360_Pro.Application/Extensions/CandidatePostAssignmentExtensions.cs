using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class CandidatePostAssignmentExtensions
    {
        public static BallotAssignmentResponse ToResponse(this CandidatePostAssignment assignment)
        {
            var response = assignment.Adapt<BallotAssignmentResponse>();

            return response with
            {
                PositionName = assignment.Position?.Name ?? "N/A",
                CandidateName =
                    assignment.Candidate != null
                        ? $"{assignment.Candidate.FirstName} {assignment.Candidate.LastName}"
                        : "N/A",
                CandidatePhotoUrl = assignment.Candidate?.PhotoPath ?? string.Empty,
                CandidateOriginalPartyName = assignment.Candidate?.OriginalParty?.Name ?? "N/A",
            };
        }

        public static IEnumerable<BallotAssignmentResponse> ToResponse(
            this IEnumerable<CandidatePostAssignment> assignments
        )
        {
            return assignments.Select(a => a.ToResponse());
        }
    }
}
