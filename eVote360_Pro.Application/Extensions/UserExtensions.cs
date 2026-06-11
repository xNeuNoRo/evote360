using eVote360_Pro.Application.DTOs.User.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Mapea la entidad User a su DTO de salida utilizando la configuración centralizada de Mapster.
        /// </summary>
        public static UserResponse ToResponse(this User user)
        {
            return user.Adapt<UserResponse>();
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
