using System.Text.RegularExpressions;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa a un ciudadano habilitado para participar en los procesos electorales.
    /// Encapsula las reglas de inmutabilidad del documento de identidad y validaciones de formato.
    /// </summary>
    public class Citizen : ActivatableBaseEntity
    {
        public string IdentityDocument { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string Email { get; private set; } = null!;

        // Navigation properties
        public virtual ICollection<VerificationCode> VerificationCodes { get; private set; } =
            new List<VerificationCode>();
        public virtual ICollection<VoterParticipation> Participations { get; private set; } =
            new List<VoterParticipation>();

        // Constructor privado para EF Core y Factory Method
        private Citizen() { }

        /// <summary>
        /// Crea una nueva instancia de un ciudadano con validaciones básicas de formato.
        /// </summary>
        public static Citizen Create(
            string identityDocument,
            string firstName,
            string lastName,
            string email
        )
        {
            ValidateBasicInfo(identityDocument, firstName, lastName, email);

            return new Citizen
            {
                IdentityDocument = identityDocument.Trim(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                IsActive = true,
            };
        }

        /// <summary>
        /// Actualiza la información del ciudadano respetando la inmutabilidad del documento de identidad.
        /// </summary>
        public void UpdateInformation(
            string identityDocument,
            string firstName,
            string lastName,
            string email,
            bool hasParticipated
        )
        {
            ValidateBasicInfo(identityDocument, firstName, lastName, email);

            if (
                hasParticipated
                && !string.Equals(
                    IdentityDocument,
                    identityDocument.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                // Si ya participó, el documento de identidad es inmutable.
                throw new DomainException(
                    "No se puede modificar el número de documento de identidad de este ciudadano porque ya participó en una elección.",
                    "Citizen.IdentityDocumentImmutable"
                );
            }

            IdentityDocument = identityDocument.Trim();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Desactiva al ciudadano, impidiendo su participación en futuras votaciones.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Activa al ciudadano.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        private static void ValidateBasicInfo(
            string identityDocument,
            string firstName,
            string lastName,
            string email
        )
        {
            if (string.IsNullOrWhiteSpace(identityDocument))
                throw new DomainException(
                    "El documento de identidad es requerido.",
                    "Citizen.IdentityDocumentRequired"
                );

            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("El nombre es requerido.", "Citizen.FirstNameRequired");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("El apellido es requerido.", "Citizen.LastNameRequired");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException(
                    "El correo electrónico es requerido.",
                    "Citizen.EmailRequired"
                );

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new DomainException(
                    "El correo electrónico debe tener un formato válido.",
                    "Citizen.InvalidEmailFormat"
                );
        }
    }
}
