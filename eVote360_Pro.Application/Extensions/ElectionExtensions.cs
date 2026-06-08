using eVote360_Pro.Application.DTOs.Election.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class ElectionExtensions
    {
        /// <summary>
        /// Mapea una elección a su respuesta, calculando estados de activación complejos.
        /// </summary>
        public static ElectionResponse ToResponse(
            this Election election,
            bool? canActivate = null,
            IEnumerable<string>? missingParties = null
        )
        {
            var response = election.Adapt<ElectionResponse>();

            return response with
            {
                Status = election.Status.ToString(),
                CanActivate = canActivate ?? false,
                MissingParties = missingParties ?? Enumerable.Empty<string>(),
            };
        }

        public static IEnumerable<ElectionResponse> ToResponse(this IEnumerable<Election> elections)
        {
            return elections.Select(e => e.ToResponse());
        }
    }
}
