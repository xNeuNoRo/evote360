using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Requests;
using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona la asignación de mandos administrativos sobre los partidos políticos.
    /// </summary>
    public interface IPoliticalLeaderAssignmentService
    {
        Task<IEnumerable<LeaderAssignmentResponse>> GetAllAsync();

        /// <summary>
        /// Vincula un usuario administrador al mando de un partido.
        /// El usuario debe tener rol 'Dirigente político' y el partido debe estar activo.
        /// </summary>
        Task CreateAssignmentAsync(SaveLeaderAssignmentRequest request);

        /// <summary>
        /// Remueve la vinculación de mando.
        /// </summary>
        Task RemoveAssignmentAsync(Guid userId);
    }
}
