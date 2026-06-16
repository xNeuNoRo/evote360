namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Proporciona acceso a la identidad e información del usuario autenticado en la sesión actual.
    /// Desacopla la lógica de negocio de la infraestructura de ASP.NET Core Identity/HttpContext.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// ID del usuario autenticado (Guid).
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// ID del partido político al que pertenece el usuario (si aplica).
        /// </summary>
        int? PartyId { get; }

        /// <summary>
        /// Indica si hay una sesión activa.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        string? FullName { get; }

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Nombre del rol principal del usuario.
        /// </summary>
        string? Role { get; }
    }
}
