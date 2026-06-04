using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Security
{
    public interface ITokenService
    {
        /// <summary>
        /// Genera un token de autenticación para un usuario dado
        /// </summary>
        string GenerateToken(User user);
    }
}
