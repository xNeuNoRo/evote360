using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class CandidateExtensions
    {
        public static CandidateResponse ToResponse(this Candidate candidate)
        {
            return candidate.Adapt<CandidateResponse>();
        }

        public static IEnumerable<CandidateResponse> ToResponse(this IEnumerable<Candidate> candidates)
        {
            return candidates.Select(c => c.ToResponse());
        }
    }
}
