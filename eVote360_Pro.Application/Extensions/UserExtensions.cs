using eVote360_Pro.Application.DTOs.User.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Aplana la entidad User a su DTO de salida, resolviendo nombres de roles y partidos asignados.
        /// </summary>
        public static UserResponse ToResponse(this User user)
        {
            var response = user.Adapt<UserResponse>();

            response = response with
            {
                FullName = $"{user.FirstName} {user.LastName}",
                RoleName = user.Role?.Name ?? "N/A",
                AssignedPartyId = user.LeaderAssignment?.PartyId,
                AssignedPartyName = user.LeaderAssignment?.Party?.Name,
            };

            return response;
        }

        /// <summary>
        /// Mapea una colección de usuarios a sus respectivos DTOs de respuesta.
        /// </summary>
        public static IEnumerable<UserResponse> ToResponse(this IEnumerable<User> users)
        {
            return users.Select(u => u.ToResponse());
        }
    }
}
