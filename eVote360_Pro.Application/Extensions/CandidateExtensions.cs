using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class CandidateExtensions
    {
        public static CandidateResponse ToResponse(this Candidate candidate)
        {
            var response = candidate.Adapt<CandidateResponse>();

            return response with
            {
                FullName = $"{candidate.FirstName} {candidate.LastName}",
                PhotoUrl = candidate.PhotoPath,
                OriginalPartyName = candidate.OriginalParty?.Name ?? "N/A",
                IsImmutable = candidate.Votes.Any(),
            };
        }

        public static IEnumerable<CandidateResponse> ToResponse(
            this IEnumerable<Candidate> candidates
        )
        {
            return candidates.Select(c => c.ToResponse());
        }
    }
}
