using eVote360_Pro.Application.DTOs.Auth.Requests;
using eVote360_Pro.Application.DTOs.Auth.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona la seguridad y el acceso de los usuarios administrativos y políticos.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Valida las credenciales del usuario y retorna un token JWT con sus permisos.
        /// </summary>
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
