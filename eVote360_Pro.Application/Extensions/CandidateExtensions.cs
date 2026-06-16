using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class CandidateExtensions
    {
        public static CandidateResponse ToResponse(this Candidate candidate)
        {
            return new CandidateResponse(
                candidate.Id,
                candidate.FirstName,
                candidate.LastName,
                $"{candidate.FirstName} {candidate.LastName}",
                candidate.PhotoPath,
                candidate.OriginalPartyId,
                candidate.OriginalParty?.Name ?? "N/A",
                candidate.OriginalParty?.Acronym ?? "N/A",
                candidate.OriginalParty?.LogoPath ?? "",
                candidate.IsActive,
                candidate.Votes?.Count ?? 0
            );
        }

        public static IEnumerable<CandidateResponse> ToResponse(this IEnumerable<Candidate> candidates)
        {
            return candidates.Select(c => c.ToResponse());
        }
    }
}
