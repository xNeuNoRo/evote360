using eVote360_Pro.Application.DTOs.Citizen.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Extensions
{
    public static class CitizenExtensions
    {
        /// <summary>
        /// Mapea un ciudadano a su DTO de salida utilizando la configuración centralizada de Mapster.
        /// Permite inyectar el contexto de la elección activa para calcular flags dinámicos.
        /// </summary>
        public static CitizenResponse ToResponse(this Citizen citizen, Guid? activeElectionId = null)
        {
            if (activeElectionId.HasValue)
            {
                return citizen.BuildAdapter()
                    .AddParameters("ActiveElectionId", activeElectionId.Value)
                    .AdaptToType<CitizenResponse>();
            }

            return citizen.Adapt<CitizenResponse>();
        }

        public static IEnumerable<CitizenResponse> ToResponse(this IEnumerable<Citizen> citizens, Guid? activeElectionId = null)
        {
            return citizens.Select(c => c.ToResponse(activeElectionId));
        }
    }
}
