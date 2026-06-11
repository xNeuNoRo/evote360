using eVote360_Pro.Application.DTOs.Role.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class RoleExtensions
    {
        public static RoleResponse ToResponse(this Role role)
        {
            return role.Adapt<RoleResponse>();
        }

        public static IEnumerable<RoleResponse> ToResponse(this IEnumerable<Role> roles)
        {
            return roles.Select(r => r.ToResponse());
        }
    }
}
