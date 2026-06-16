using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un partido político dentro del sistema electoral.
    /// Esta entidad encapsula las reglas de inmutabilidad y validaciones de estado.
    /// </summary>
    public class PoliticalParty : ActivatableBaseEntity
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string Acronym { get; private set; } = null!;
        public string LogoPath { get; private set; } = null!;

        // Navigation properties
        public virtual ICollection<Candidate> Candidates { get; private set; } =
            new List<Candidate>();
        public virtual ICollection<CandidatePostAssignment> CandidatePostAssignments
        {
            get;
            private set;
        } = new List<CandidatePostAssignment>();
        public virtual ICollection<Vote> Votes { get; private set; } = new List<Vote>();
        public virtual ICollection<PoliticalAlliance> RequestedAlliances { get; private set; } =
            new List<PoliticalAlliance>();
        public virtual ICollection<PoliticalAlliance> ReceivedAlliances { get; private set; } =
            new List<PoliticalAlliance>();
        public virtual PoliticalLeaderAssignment? LeaderAssignment { get; private set; }

        // Constructor privado para EF Core y Factory Method
        private PoliticalParty() { }

        /// <summary>
        /// Crea una nueva instancia de un partido político con las validaciones iniciales.
        /// </summary>
        public static PoliticalParty Create(
            string name,
            string acronym,
            string logoPath,
            string? description = null,
            bool isActive = true
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "El nombre del partido es requerido.",
                    "PoliticalParty.NameRequired"
                );

            if (string.IsNullOrWhiteSpace(acronym))
                throw new DomainException(
                    "Las siglas del partido son requeridas.",
                    "PoliticalParty.AcronymRequired"
                );

            if (string.IsNullOrWhiteSpace(logoPath))
                throw new DomainException(
                    "El logo del partido es requerido al crear.",
                    "PoliticalParty.LogoRequired"
                );

            return new PoliticalParty
            {
                Name = name.Trim(),
                Acronym = acronym.Trim().ToUpperInvariant(),
                LogoPath = logoPath,
                Description = description?.Trim(),
                IsActive = isActive,
            };
        }

        /// <summary>
        /// Actualiza la información del partido.
        /// </summary>
        public void UpdateInformation(
            string name,
            string acronym,
            string? logoPath,
            string? description,
            bool hasParticipated
        )
        {
            if (hasParticipated)
            {
                if (!string.Equals(Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new DomainException(
                        "No se puede modificar el nombre de este partido político porque ya participó en una elección.",
                        "PoliticalParty.NameImmutable"
                    );
                }

                if (!string.Equals(Acronym, acronym.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new DomainException(
                        "No se pueden modificar las siglas de este partido político porque ya participó en una elección.",
                        "PoliticalParty.AcronymImmutable"
                    );
                }
            }

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "El nombre del partido es requerido.",
                    "PoliticalParty.NameRequired"
                );

            if (string.IsNullOrWhiteSpace(acronym))
                throw new DomainException(
                    "Las siglas del partido son requeridas.",
                    "PoliticalParty.AcronymRequired"
                );

            Name = name.Trim();
            Acronym = acronym.Trim().ToUpperInvariant();
            Description = description?.Trim();

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                LogoPath = logoPath;
            }
        }

        /// <summary>
        /// Desactiva el partido político.
        /// </summary>
        public void Deactivate(bool hasActiveCandidates, bool hasAssignedLeader)
        {
            if (hasActiveCandidates)
                throw new DomainException(
                    "No se puede desactivar un partido político que tiene candidatos activos registrados.",
                    "PoliticalParty.HasActiveCandidates"
                );

            if (hasAssignedLeader)
                throw new DomainException(
                    "No se puede desactivar un partido político que tiene un dirigente político asignado.",
                    "PoliticalParty.HasAssignedLeader"
                );

            IsActive = false;
        }

        /// <summary>
        /// Activa el partido político.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }
    }
}
