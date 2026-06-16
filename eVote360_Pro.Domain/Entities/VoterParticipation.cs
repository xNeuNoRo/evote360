using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Registra la participación de un ciudadano en una elección específica.
    /// </summary>
    public class VoterParticipation : BaseEntity
    {
        public int CitizenId { get; private set; }
        public Guid ElectionId { get; private set; }

        // Navigation properties
        public virtual Citizen Citizen { get; private set; } = null!;
        public virtual Election Election { get; private set; } = null!;

        // Constructor privado para EF Core y Factory Method
        private VoterParticipation() { }

        /// <summary>
        /// Crea un nuevo registro de participación validando la elegibilidad básica.
        /// </summary>
        public static VoterParticipation Create(
            int citizenId,
            Guid electionId,
            bool isCitizenActive,
            bool isElectionActive
        )
        {
            if (citizenId <= 0)
                throw new DomainException(
                    "El ciudadano es requerido.",
                    "Participation.CitizenRequired"
                );

            if (electionId == Guid.Empty)
                throw new DomainException(
                    "La elección es requerida.",
                    "Participation.ElectionRequired"
                );

            if (!isCitizenActive)
                throw new DomainException(
                    "Un ciudadano inactivo no puede participar en procesos de votación.",
                    "Participation.CitizenInactive"
                );

            if (!isElectionActive)
                throw new DomainException(
                    "Solo se puede registrar participación en elecciones con estado activo.",
                    "Participation.ElectionNotActive"
                );

            return new VoterParticipation { CitizenId = citizenId, ElectionId = electionId };
        }
    }
}
