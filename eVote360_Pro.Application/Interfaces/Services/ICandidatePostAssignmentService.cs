using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Requests;
using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Orquesta la conformación de la boleta electoral de un partido político.
    /// </summary>
    public interface ICandidatePostAssignmentService
    {
        /// <summary>
        /// Obtiene la boleta configurada actualmente para el partido logueado.
        /// </summary>
        Task<IEnumerable<BallotAssignmentResponse>> GetMyBallotAsync();

        /// <summary>
        /// Asigna un candidato (propio o aliado) a un puesto activo.
        /// Si es aliado, debe aspirar al mismo puesto que en su partido origen.
        /// </summary>
        Task SaveAssignmentAsync(SaveBallotAssignmentRequest request);

        /// <summary>
        /// Remueve a un candidato de una posición en la boleta.
        /// </summary>
        Task RemoveAssignmentAsync(int id);

        /// <summary>
        /// Obtiene los candidatos (propios o de partidos aliados) que aún no están asignados a ningún puesto.
        /// </summary>
        Task<IEnumerable<CandidateResponse>> GetAvailableCandidatesAsync();
    }
}
