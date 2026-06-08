using eVote360_Pro.Application.DTOs.PoliticalParty.Requests;
using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona las agrupaciones políticas y sus identidades visuales.
    /// </summary>
    public interface IPoliticalPartyService
    {
        /// <summary>
        /// Obtiene todos los partidos registrados.
        /// </summary>
        Task<IEnumerable<PoliticalPartyResponse>> GetAllAsync();

        /// <summary>
        /// Obtiene un partido por su ID.
        /// </summary>
        Task<PoliticalPartyResponse?> GetByIdAsync(int id);

        /// <summary>
        /// Registra un nuevo partido político.
        /// </summary>
        Task<PoliticalPartyResponse> CreateAsync(CreatePoliticalPartyRequest request);

        /// <summary>
        /// Actualiza los datos de un partido existente.
        /// </summary>
        Task<PoliticalPartyResponse> UpdateAsync(UpdatePoliticalPartyRequest request);

        /// <summary>
        /// Activa o desactiva un partido político. Solo los partidos activos
        /// pueden ser asignados a puestos en la boleta.
        /// </summary>
        Task ToggleStatusAsync(int id, bool activate);
    }
}
