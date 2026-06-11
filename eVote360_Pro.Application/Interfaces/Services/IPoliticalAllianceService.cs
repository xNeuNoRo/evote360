using eVote360_Pro.Application.DTOs.PoliticalAlliance.Requests;
using eVote360_Pro.Application.DTOs.PoliticalAlliance.Responses;
using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona las solicitudes y el estado de los pactos entre partidos.
    /// </summary>
    public interface IPoliticalAllianceService
    {
        /// <summary>
        /// Obtiene todas las alianzas donde el partido logueado sea solicitante o receptor.
        /// </summary>
        Task<IEnumerable<AllianceResponse>> GetMyAlliancesAsync();

        /// <summary>
        /// Inicia una propuesta de pacto con otro partido.
        /// </summary>
        Task RequestAllianceAsync(CreateAllianceRequest request);

        /// <summary>
        /// Acepta una propuesta de pacto recibida.
        /// </summary>
        Task AcceptAllianceAsync(int allianceId);

        /// <summary>
        /// Rechaza una propuesta de pacto recibida.
        /// </summary>
        Task RejectAllianceAsync(int allianceId);

        /// <summary>
        /// Elimina una alianza o solicitud.
        /// No se puede eliminar si ya tiene candidatos aliados asignados.
        /// </summary>
        Task DeleteAllianceAsync(int id);

        /// <summary>
        /// Obtiene la lista de partidos con los que se puede solicitar una alianza.
        /// (Excluye el propio partido y aquellos con los que ya existe un pacto).
        /// </summary>
        Task<IEnumerable<PoliticalPartyResponse>> GetAvailablePartiesForRequestAsync();
    }
}
