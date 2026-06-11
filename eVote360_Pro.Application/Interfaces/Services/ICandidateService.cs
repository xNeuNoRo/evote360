using eVote360_Pro.Application.DTOs.Candidate.Requests;
using eVote360_Pro.Application.DTOs.Candidate.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona los candidatos del partido.
    /// </summary>
    public interface ICandidateService
    {
        /// <summary>
        /// Obtiene candidatos. Si el usuario es dirigente, filtra solo los de su partido.
        /// </summary>
        Task<IEnumerable<CandidateResponse>> GetAllAsync();

        /// <summary>
        /// Obtiene un candidato por su ID.
        /// </summary>
        Task<CandidateResponse?> GetByIdAsync(int id);

        /// <summary>
        /// Registra un nuevo candidato.
        /// </summary>
        Task<CandidateResponse> CreateAsync(CreateCandidateRequest request);

        /// <summary>
        /// Actualiza los datos de un candidato existente.
        /// </summary>
        Task<CandidateResponse> UpdateAsync(UpdateCandidateRequest request);

        /// <summary>
        /// Activa o desactiva un candidato. Solo los candidatos activos pueden
        /// ser asignados a puestos en la boleta.
        /// </summary>
        Task ToggleStatusAsync(int id, bool activate);
    }
}
