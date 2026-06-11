using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Security
{
    /// <summary>
    /// Contrato para el servicio de generación de tokens de acceso (JWT).
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Genera un token que contiene la identidad y permisos del usuario.
        /// </summary>
        /// <param name="user">El usuario para el cual generar el token.</param>
        /// <returns>Un objeto TokenResponse con el JWT y la fecha de expiración.</returns>
        TokenResponse GenerateToken(User user);
    }
}
