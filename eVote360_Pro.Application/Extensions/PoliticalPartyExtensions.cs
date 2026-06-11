using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class PartyExtensions
    {
        public static PoliticalPartyResponse ToResponse(this PoliticalParty party)
        {
            return party.Adapt<PoliticalPartyResponse>();
        }

        public static IEnumerable<PoliticalPartyResponse> ToResponse(this IEnumerable<PoliticalParty> parties)
        {
            return parties.Select(p => p.ToResponse());
        }
    }
}
