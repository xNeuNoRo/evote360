using eVote360_Pro.Application.DTOs.Election.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class ElectionExtensions
    {
        public static ElectionResponse ToResponse(
            this Election election, 
            bool? canActivate = null, 
            IEnumerable<string>? missingParties = null)
        {
            var adapter = election.BuildAdapter();

            if (canActivate.HasValue)
                adapter.AddParameters("CanActivate", canActivate.Value);

            if (missingParties != null)
                adapter.AddParameters("MissingParties", missingParties);

            return adapter.AdaptToType<ElectionResponse>();
        }

        public static IEnumerable<ElectionResponse> ToResponse(this IEnumerable<Election> elections)
        {
            return elections.Select(e => e.ToResponse());
        }
    }
}
