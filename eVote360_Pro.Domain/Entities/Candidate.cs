using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un candidato político en el sistema.
    /// Encapsula las reglas de integridad y las restricciones de edición post-electoral.
    /// </summary>
    public class Candidate : ActivatableBaseEntity
    {
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string PhotoPath { get; private set; } = null!;
        public int OriginalPartyId { get; private set; }

        // Navigation properties
        public virtual PoliticalParty OriginalParty { get; private set; } = null!;
        public virtual ICollection<CandidatePostAssignment> PostAssignments { get; private set; } =
            new List<CandidatePostAssignment>();
        public virtual ICollection<Vote> Votes { get; private set; } = new List<Vote>();

        // Constructor privado para EF Core y Factory Method
        private Candidate() { }

        /// <summary>
        /// Crea una nueva instancia de un candidato.
        /// </summary>
        public static Candidate Create(
            string firstName,
            string lastName,
            string photoPath,
            int originalPartyId
        )
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException(
                    "El nombre del candidato es requerido.",
                    "Candidate.FirstNameRequired"
                );

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException(
                    "El apellido del candidato es requerido.",
                    "Candidate.LastNameRequired"
                );

            if (string.IsNullOrWhiteSpace(photoPath))
                throw new DomainException(
                    "La foto del candidato es requerida al crear.",
                    "Candidate.PhotoRequired"
                );

            if (originalPartyId <= 0)
                throw new DomainException(
                    "El candidato debe estar asociado a un partido político válido.",
                    "Candidate.InvalidParty"
                );

            return new Candidate
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                PhotoPath = photoPath,
                OriginalPartyId = originalPartyId,
                IsActive = true,
            };
        }

        /// <summary>
        /// Actualiza la información del candidato.
        /// </summary>
        /// <param name="firstName">Nuevo nombre.</param>
        /// <param name="lastName">Nuevo apellido.</param>
        /// <param name="photoPath">Nueva ruta de foto (opcional).</param>
        /// <param name="hasParticipated">Indica si el candidato ya participó en una elección activa o finalizada.</param>
        public void UpdateInformation(
            string firstName,
            string lastName,
            string? photoPath,
            bool hasParticipated
        )
        {
            if (hasParticipated)
            {
                throw new DomainException(
                    "No se pueden modificar los datos principales de este candidato porque ya participó en una elección.",
                    "Candidate.AlreadyParticipated"
                );
            }

            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException(
                    "El nombre del candidato es requerido.",
                    "Candidate.FirstNameRequired"
                );

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException(
                    "El apellido del candidato es requerido.",
                    "Candidate.LastNameRequired"
                );

            FirstName = firstName.Trim();
            LastName = lastName.Trim();

            if (!string.IsNullOrWhiteSpace(photoPath))
            {
                PhotoPath = photoPath;
            }
        }

        /// <summary>
        /// Desactiva al candidato validando que no tenga asignaciones vigentes.
        /// </summary>
        /// <param name="hasActiveAssignment">Indica si el candidato tiene una asignación a un puesto electivo vigente.</param>
        public void Deactivate(bool hasActiveAssignment)
        {
            if (hasActiveAssignment)
                throw new DomainException(
                    "No se puede desactivar un candidato que está asignado a un puesto electivo vigente.",
                    "Candidate.HasActiveAssignment"
                );

            IsActive = false;
        }

        /// <summary>
        /// Activa al candidato.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }
    }
}
