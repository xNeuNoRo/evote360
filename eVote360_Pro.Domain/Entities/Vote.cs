using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un voto individual. Utiliza Guid para garantizar el anonimato y evitar correlación temporal.
    /// </summary>
    public class Vote : BaseEntity<Guid>
    {
        public Guid ElectionId { get; private set; }
        public int PositionId { get; private set; }
        public int? CandidateId { get; private set; }
        public int? PartyId { get; private set; }

        // Navigation Properties
        public virtual Election Election { get; private set; } = null!;
        public virtual ElectivePosition Position { get; private set; } = null!;
        public virtual Candidate? Candidate { get; private set; }
        public virtual PoliticalParty? Party { get; private set; }

        // Constructor privado para EF Core y Factory Method
        private Vote() { }

        /// <summary>
        /// Crea un nuevo voto con un ID aleatorio y validaciones de integridad.
        /// </summary>
        public static Vote Create(
            Guid electionId,
            int positionId,
            int? candidateId = null,
            int? partyId = null
        )
        {
            if (electionId == Guid.Empty)
                throw new DomainException("La elección es requerida.", "Vote.ElectionRequired");

            if (positionId <= 0)
                throw new DomainException(
                    "El puesto electivo es requerido.",
                    "Vote.PositionRequired"
                );

            // Lógica para opción "Ninguno" vs Voto a Candidato
            if (candidateId.HasValue && !partyId.HasValue)
                throw new DomainException(
                    "Un voto a un candidato debe estar asociado a un partido político.",
                    "Vote.PartyRequiredForCandidate"
                );

            if (!candidateId.HasValue && partyId.HasValue)
                throw new DomainException(
                    "No se puede emitir un voto a un partido sin un candidato seleccionado.",
                    "Vote.CandidateRequiredForParty"
                );

            return new Vote
            {
                Id = Guid.NewGuid(),
                ElectionId = electionId,
                PositionId = positionId,
                CandidateId = candidateId,
                PartyId = partyId,
            };
        }

        /// <summary>
        /// Indica si el voto corresponde a la opción "Ninguno".
        /// </summary>
        public bool IsNoneOption => !CandidateId.HasValue && !PartyId.HasValue;
    }
}
