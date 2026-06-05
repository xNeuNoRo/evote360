using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un voto individual emitido en una elección.
    /// Esta entidad es estrictamente anónima y no contiene referencias al ciudadano.
    /// </summary>
    public class Vote : BaseEntity
    {
        public int ElectionId { get; private set; }
        public int PositionId { get; private set; }

        /// <summary>
        /// ID del candidato seleccionado. Nulo si es la opción "Ninguno".
        /// </summary>
        public int? CandidateId { get; private set; }

        /// <summary>
        /// ID del partido por el cual se emitió el voto. Nulo si es la opción "Ninguno".
        /// </summary>
        public int? PartyId { get; private set; }

        // Navigation Properties
        public virtual Election Election { get; private set; } = null!;
        public virtual ElectivePosition Position { get; private set; } = null!;
        public virtual Candidate? Candidate { get; private set; }
        public virtual PoliticalParty? Party { get; private set; }

        // Constructor privado para EF Core y Factory Method
        private Vote() { }

        /// <summary>
        /// Crea un nuevo voto validando la integridad de la selección.
        /// Soporta tanto votos a candidatos/partidos como la opción "Ninguno".
        /// </summary>
        public static Vote Create(
            int electionId,
            int positionId,
            int? candidateId = null,
            int? partyId = null
        )
        {
            if (electionId <= 0)
                throw new DomainException("La elección es requerida.", "Vote.ElectionRequired");

            if (positionId <= 0)
                throw new DomainException(
                    "El puesto electivo es requerido.",
                    "Vote.PositionRequired"
                );

            // Lógica para opción "Ninguno" vs Voto a Candidato
            if (candidateId.HasValue && !partyId.HasValue)
            {
                throw new DomainException(
                    "Un voto a un candidato debe estar asociado a un partido político.",
                    "Vote.PartyRequiredForCandidate"
                );
            }

            if (!candidateId.HasValue && partyId.HasValue)
            {
                throw new DomainException(
                    "No se puede emitir un voto a un partido sin un candidato seleccionado.",
                    "Vote.CandidateRequiredForParty"
                );
            }

            return new Vote
            {
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
