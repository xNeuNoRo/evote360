using System;

namespace eVote360_Pro.Domain.Interfaces.Security
{
    /// <summary>
    /// Representa el resultado de la generación de un token de acceso.
    /// Encapsula el token serializado y su fecha de expiración calculada.
    /// </summary>
    public record TokenResponse(
        string Token,
        DateTime Expiration
    );
}
