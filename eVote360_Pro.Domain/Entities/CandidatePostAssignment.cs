using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa la vinculación de un candidato con un puesto electivo dentro de un partido.
    /// Define la boleta electoral y soporta la lógica de candidatos aliados.
    /// </summary>
    public class CandidatePostAssignment : BaseEntity
    {
        public int CandidateId { get; private set; }
        public int PositionId { get; private set; }
        public int PartyId { get; private set; }
        public bool IsAlly { get; private set; }

        // Navigation properties
        public virtual Candidate Candidate { get; private set; } = null!;
        public virtual ElectivePosition Position { get; private set; } = null!;
        public virtual PoliticalParty Party { get; private set; } = null!;

        // Constructor privado para EF Core y Factory Method
        private CandidatePostAssignment() { }

        /// <summary>
        /// Crea una nueva asignación de candidato a puesto con validaciones de integridad.
        /// </summary>
        public static CandidatePostAssignment Create(
            int candidateId,
            int positionId,
            int partyId,
            bool isAlly,
            bool isCandidateActive,
            bool isPositionActive
        )
        {
            if (candidateId <= 0)
                throw new DomainException(
                    "El candidato es requerido.",
                    "Assignment.InvalidCandidate"
                );

            if (positionId <= 0)
                throw new DomainException(
                    "El puesto electivo es requerido.",
                    "Assignment.InvalidPosition"
                );

            if (partyId <= 0)
                throw new DomainException(
                    "El partido político es requerido.",
                    "Assignment.InvalidParty"
                );

            if (!isCandidateActive)
                throw new DomainException(
                    "Solo se pueden asignar candidatos activos.",
                    "Assignment.CandidateInactive"
                );

            if (!isPositionActive)
                throw new DomainException(
                    "Solo se pueden asignar puestos electivos activos.",
                    "Assignment.PositionInactive"
                );

            return new CandidatePostAssignment
            {
                CandidateId = candidateId,
                PositionId = positionId,
                PartyId = partyId,
                IsAlly = isAlly,
            };
        }
    }
}
