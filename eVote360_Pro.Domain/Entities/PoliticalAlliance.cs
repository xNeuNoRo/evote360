using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa una solicitud o alianza formal entre dos partidos políticos.
    /// Encapsula las reglas de transición de estados y validaciones de integridad.
    /// </summary>
    public class PoliticalAlliance : BaseEntity
    {
        public int RequesterPartyId { get; private set; }
        public int ReceiverPartyId { get; private set; }
        public AllianceStatus Status { get; private set; }
        public DateTime? AcceptedAt { get; private set; }

        // Navigation properties
        public virtual PoliticalParty RequesterParty { get; private set; } = null!;
        public virtual PoliticalParty ReceiverParty { get; private set; } = null!;

        // Constructor privado para EF Core y Factory Method
        private PoliticalAlliance() { }

        /// <summary>
        /// Crea una nueva solicitud de alianza con validaciones de negocio.
        /// </summary>
        public static PoliticalAlliance Create(
            int requesterPartyId,
            int receiverPartyId,
            bool isReceiverActive
        )
        {
            if (requesterPartyId <= 0 || receiverPartyId <= 0)
                throw new DomainException(
                    "Los identificadores de los partidos deben ser válidos.",
                    "Alliance.InvalidParties"
                );

            if (requesterPartyId == receiverPartyId)
                throw new DomainException(
                    "No puede crear una solicitud de alianza hacia su propio partido político.",
                    "Alliance.SameParty"
                );

            if (!isReceiverActive)
                throw new DomainException(
                    "No puede crear una solicitud de alianza con un partido político inactivo.",
                    "Alliance.ReceiverInactive"
                );

            return new PoliticalAlliance
            {
                RequesterPartyId = requesterPartyId,
                ReceiverPartyId = receiverPartyId,
                Status = AllianceStatus.Pending,
            };
        }

        /// <summary>
        /// Acepta la solicitud de alianza política.
        /// </summary>
        public void Accept(bool isRequesterActive, bool isReceiverActive, DateTime currentTime)
        {
            if (Status != AllianceStatus.Pending)
                throw new DomainException(
                    "Esta solicitud de alianza ya fue respondida.",
                    "Alliance.AlreadyResponded"
                );

            if (!isRequesterActive || !isReceiverActive)
                throw new DomainException(
                    "Ambos partidos deben estar activos para aceptar la alianza.",
                    "Alliance.PartiesNotActive"
                );

            Status = AllianceStatus.Accepted;
            AcceptedAt = currentTime;
        }

        /// <summary>
        /// Rechaza la solicitud de alianza política.
        /// </summary>
        public void Reject()
        {
            if (Status != AllianceStatus.Pending)
                throw new DomainException(
                    "Esta solicitud de alianza ya fue respondida.",
                    "Alliance.AlreadyResponded"
                );

            Status = AllianceStatus.Rejected;
        }

        /// <summary>
        /// Valida si la alianza puede ser eliminada físicamente o terminada.
        /// </summary>
        /// <param name="hasAlliedAssignments">Indica si existen candidatos aliados asignados entre estos partidos.</param>
        public void EnsureCanBeDeleted(bool hasAlliedAssignments)
        {
            if (Status == AllianceStatus.Accepted && hasAlliedAssignments)
            {
                throw new DomainException(
                    "No se puede eliminar esta alianza porque existen candidatos aliados asignados entre estos partidos.",
                    "Alliance.HasActiveAssignments"
                );
            }
        }
    }
}
