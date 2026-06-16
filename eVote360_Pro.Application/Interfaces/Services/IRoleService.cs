using eVote360_Pro.Application.DTOs.Role.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Proporciona acceso a los roles autorizados del sistema.
    /// </summary>
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetRolesAsync();
    }
}
