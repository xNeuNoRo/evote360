using eVote360_Pro.Application.DTOs.User.Requests;
using eVote360_Pro.Application.DTOs.User.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona el mantenimiento de personal administrativo y político.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Obtiene todos los usuarios registrados, con sus roles y estados de actividad.
        /// </summary>
        Task<IEnumerable<UserResponse>> GetAllAsync();

        /// <summary>
        /// Obtiene un usuario por su ID, incluyendo su rol y estado de actividad.
        /// </summary>
        Task<UserResponse?> GetByIdAsync(Guid id);

        /// <summary>
        /// Registra un nuevo usuario con rol y estado de actividad.
        /// </summary>
        Task<UserResponse> CreateAsync(CreateUserRequest request);

        /// <summary>
        /// Actualiza los datos de un usuario existente, incluyendo su rol y estado de actividad.
        /// </summary>
        Task<UserResponse> UpdateAsync(UpdateUserRequest request);

        /// <summary>
        /// Cambia el estado de actividad.
        /// </summary>
        Task ToggleStatusAsync(Guid id, bool activate);
    }
}
