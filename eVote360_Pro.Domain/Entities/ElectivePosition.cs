using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un cargo o posición electoral disponible para votación.
    /// Encapsula las reglas de inmutabilidad del nombre y restricciones de estado.
    /// </summary>
    public class ElectivePosition : ActivatableBaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;

        // Navigation properties
        public virtual ICollection<CandidatePostAssignment> CandidatePostAssignments
        {
            get;
            private set;
        } = new List<CandidatePostAssignment>();
        public virtual ICollection<Vote> Votes { get; private set; } = new List<Vote>();

        // Constructor privado para EF Core y Factory Method
        private ElectivePosition() { }

        /// <summary>
        /// Crea una nueva instancia de un puesto electivo con validaciones básicas.
        /// </summary>
        public static ElectivePosition Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "El nombre del puesto es requerido.",
                    "ElectivePosition.NameRequired"
                );

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException(
                    "La descripción del puesto es requerida.",
                    "ElectivePosition.DescriptionRequired"
                );

            return new ElectivePosition
            {
                Name = name.Trim(),
                Description = description.Trim(),
                IsActive = true,
            };
        }

        /// <summary>
        /// Actualiza la información del puesto respetando la inmutabilidad histórica del nombre.
        /// </summary>
        public void UpdateInformation(string name, string description, bool wasUsedInElection)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "El nombre del puesto es requerido.",
                    "ElectivePosition.NameRequired"
                );

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException(
                    "La descripción del puesto es requerida.",
                    "ElectivePosition.DescriptionRequired"
                );

            if (
                wasUsedInElection
                && !string.Equals(Name, name.Trim(), StringComparison.OrdinalIgnoreCase)
            )
            {
                // Si ya participó en una elección, el nombre es inmutable.
                throw new DomainException(
                    "No se puede modificar el nombre del puesto porque ya participó en una elección.",
                    "ElectivePosition.NameImmutable"
                );
            }

            Name = name.Trim();
            Description = description.Trim();
        }

        /// <summary>
        /// Desactiva el puesto validando que no tenga candidatos activos.
        /// </summary>
        public void Deactivate(bool hasActiveCandidates)
        {
            if (hasActiveCandidates)
                throw new DomainException(
                    "No se puede desactivar un puesto que tiene candidatos activos asignados.",
                    "ElectivePosition.HasActiveCandidates"
                );

            IsActive = false;
        }

        /// <summary>
        /// Activa el puesto electivo.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }
    }
}
