using eVote360_Pro.Application.DTOs.PoliticalAlliance.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class PoliticalAllianceExtensions
    {
        public static AllianceResponse ToResponse(this PoliticalAlliance alliance)
        {
            return alliance.Adapt<AllianceResponse>();
        }

        public static IEnumerable<AllianceResponse> ToResponse(this IEnumerable<PoliticalAlliance> alliances)
        {
            return alliances.Select(a => a.ToResponse());
        }
    }
}
