using eVote360_Pro.Application.DTOs.PoliticalAlliance.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class AllianceExtensions
    {
        public static AllianceResponse ToResponse(this PoliticalAlliance alliance)
        {
            var response = alliance.Adapt<AllianceResponse>();

            return response with
            {
                RequesterPartyName = alliance.RequesterParty?.Name ?? "N/A",
                ReceiverPartyName = alliance.ReceiverParty?.Name ?? "N/A",
            };
        }

        public static IEnumerable<AllianceResponse> ToResponse(
            this IEnumerable<PoliticalAlliance> alliances
        )
        {
            return alliances.Select(a => a.ToResponse());
        }
    }
}
