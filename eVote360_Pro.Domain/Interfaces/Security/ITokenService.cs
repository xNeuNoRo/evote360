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
        string GenerateToken(User user);
    }
}
