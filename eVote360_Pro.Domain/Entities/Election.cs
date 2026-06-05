using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un proceso electoral formal y gestiona su ciclo de vida (Pendiente, Activa, Finalizada).
    /// Es el corazón del motor electoral y controla la inmutabilidad de los participantes.
    /// </summary>
    public class Election : ActivatableBaseEntity
    {
        public string Name { get; private set; } = null!;
        public DateTime RealizationDate { get; private set; }
        public ElectionStatus Status { get; private set; }

        // Navigation Properties
        public virtual ICollection<Vote> Votes { get; private set; } = new List<Vote>();
        public virtual ICollection<VoterParticipation> Participations { get; private set; } =
            new List<VoterParticipation>();
        public virtual ICollection<VerificationCode> VerificationCodes { get; private set; } =
            new List<VerificationCode>();

        // Constructor privado para EF Core y Factory Method
        private Election() { }

        /// <summary>
        /// Crea una nueva elección en estado pendiente con validaciones básicas.
        /// </summary>
        public static Election Create(string name, DateTime realizationDate, bool hasActiveElection)
        {
            if (hasActiveElection)
                throw new DomainException("No se puede crear una nueva elección mientras exista una elección activa.", "Election.ActiveAlreadyExists");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("El nombre de la elección es requerido.", "Election.NameRequired");

            return new Election
            {
                Name = name.Trim(),
                RealizationDate = realizationDate,
                Status = ElectionStatus.Pending,
                IsActive = true
            };
        }

        /// <summary>
        /// Actualiza la información básica de una elección siempre que esté pendiente.
        /// </summary>
        public void UpdateInformation(string name, DateTime realizationDate)
        {
            if (Status != ElectionStatus.Pending)
                throw new DomainException("Solo se puede editar una elección que se encuentre en estado pendiente.", "Election.NotPending");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("El nombre de la elección es requerido.", "Election.NameRequired");

            Name = name.Trim();
            RealizationDate = realizationDate;
        }

        /// <summary>
        /// Activa la elección permitiendo el inicio del proceso de votación.
        /// Valida la configuración electoral mínima requerida por el negocio.
        /// </summary>
        public void Activate(
            bool hasAnotherActiveElection,
            bool hasActivePositions,
            bool hasMinimumParties,
            IEnumerable<string>? partiesWithMissingCandidates = null)
        {
            if (Status != ElectionStatus.Pending)
                throw new DomainException("Solo se pueden activar elecciones en estado pendiente.", "Election.NotPending");

            if (hasAnotherActiveElection)
                throw new DomainException("No se puede activar esta elección porque ya existe una elección activa.", "Election.ActiveAlreadyExists");

            if (!hasActivePositions)
                throw new DomainException("No hay puestos electivos activos para activar esta elección.", "Election.NoActivePositions");

            if (!hasMinimumParties)
                throw new DomainException("No hay suficientes partidos políticos para activar esta elección.", "Election.NotEnoughParties");

            var missingList = partiesWithMissingCandidates?.ToList();
            if (missingList != null && missingList.Any())
            {
                // El documento pide un mensaje por cada partido, los agrupamos en los detalles de la excepción.
                throw new DomainException("Existen partidos políticos con candidatos incompletos para los puestos activos.", "Election.IncompleteConfiguration", missingList);
            }

            Status = ElectionStatus.Active;
        }

        /// <summary>
        /// Finaliza la elección, cerrando el proceso de votación y habilitando resultados.
        /// </summary>
        public void Finish()
        {
            if (Status != ElectionStatus.Active)
                throw new DomainException("Solo se pueden finalizar elecciones activas.", "Election.NotActive");

            Status = ElectionStatus.Finished;
        }

        /// <summary>
        /// Desactiva (eliminación lógica) la elección si está pendiente.
        /// </summary>
        public void Deactivate()
        {
            if (Status != ElectionStatus.Pending)
                throw new DomainException("Solo se pueden desactivar (eliminar) elecciones en estado pendiente.", "Election.CannotDeactivateNonPending");

            IsActive = false;
        }

        /// <summary>
        /// Activa nuevamente una elección previamente desactivada.
        /// </summary>
        public void ReActivate()
        {
            IsActive = true;
        }
    }
}
