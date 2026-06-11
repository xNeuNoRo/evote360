using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Vinculación 1:1 entre Usuario y Partido.
    /// </summary>
    public class PoliticalLeaderAssignment : BaseEntity<Guid>
    {
        public int PartyId { get; private set; }

        // Navigation property
        public virtual PoliticalParty Party { get; private set; } = null!;
        public virtual User User { get; private set; } = null!;

        // Constructor privado para EF Core y Factory Method
        private PoliticalLeaderAssignment() { }

        /// <summary>
        /// Crea una nueva asignación de dirigente con validaciones de rol y estado.
        /// </summary>
        public static PoliticalLeaderAssignment Create(
            Guid userId,
            int partyId,
            bool isUserActive,
            bool isUserDirigente,
            bool isPartyActive
        )
        {
            if (userId == Guid.Empty)
                throw new DomainException("El usuario es requerido.", "Assignment.InvalidUser");

            if (partyId <= 0)
                throw new DomainException(
                    "El partido político es requerido.",
                    "Assignment.InvalidParty"
                );

            if (!isUserActive)
                throw new DomainException(
                    "El usuario seleccionado no está activo.",
                    "Assignment.UserInactive"
                );

            if (!isUserDirigente)
                throw new DomainException(
                    "El usuario seleccionado no tiene el rol de dirigente político.",
                    "Assignment.InvalidRole"
                );

            if (!isPartyActive)
                throw new DomainException(
                    "El partido político seleccionado no está activo.",
                    "Assignment.PartyInactive"
                );

            return new PoliticalLeaderAssignment
            {
                Id = userId, // PK es el FK hacia User
                PartyId = partyId
            };
        }
    }
}
