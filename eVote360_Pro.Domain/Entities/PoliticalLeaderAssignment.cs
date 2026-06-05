using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa la vinculación 1:1 entre un usuario con rol Dirigente y un partido político.
    /// Encapsula las reglas de validación de roles y estados de activación.
    /// </summary>
    public class PoliticalLeaderAssignment : ActivatableBaseEntity
    {
        public int UserId { get; private set; }
        public int PartyId { get; private set; }

        // Navigation properties
        public virtual PoliticalParty Party { get; private set; } = null!;
        public virtual User User { get; private set; } = null!;

        // Constructor privado para EF Core y Factory Method
        private PoliticalLeaderAssignment() { }

        /// <summary>
        /// Crea una nueva asignación de dirigente con validaciones de rol y estado.
        /// </summary>
        public static PoliticalLeaderAssignment Create(
            int userId,
            int partyId,
            bool isUserActive,
            bool isUserDirigente,
            bool isPartyActive
        )
        {
            if (userId <= 0)
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
                UserId = userId,
                PartyId = partyId,
                IsActive = true,
            };
        }
    }
}
