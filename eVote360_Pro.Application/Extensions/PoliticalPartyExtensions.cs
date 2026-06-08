using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class PartyExtensions
    {
        public static PoliticalPartyResponse ToResponse(this PoliticalParty party)
        {
            var response = party.Adapt<PoliticalPartyResponse>();

            // El logo lo mapeamos tal cual, la capa Web se encargará de resolver la URL absoluta si es necesario
            return response with
            {
                LogoUrl = party.LogoPath,
                IsImmutable = party.Votes.Any() || party.CandidatePostAssignments.Any(),
                HasLeaderAssigned =
                    party.LeaderAssignment != null && party.LeaderAssignment.IsActive,
            };
        }

        public static IEnumerable<PoliticalPartyResponse> ToResponse(
            this IEnumerable<PoliticalParty> parties
        )
        {
            return parties.Select(p => p.ToResponse());
        }
    }
}
