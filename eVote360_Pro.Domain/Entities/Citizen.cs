using System.Text.RegularExpressions;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.ValueObjects;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa a un ciudadano habilitado para participar en los procesos electorales.
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
        /// Crea una nueva instancia de un ciudadano validando el documento de identidad mediante un Value Object.
        /// </summary>
        public static Citizen Create(
            IdentityDocument identityDocument,
            string firstName,
            string lastName,
            string email,
            bool isActive = true
        )
        {
            ValidateBasicInfo(firstName, lastName, email);

            return new Citizen
            {
                IdentityDocument = identityDocument.Value,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                IsActive = isActive,
            };
        }

        /// <summary>
        /// Actualiza la información del ciudadano respetando la inmutabilidad del documento de identidad.
        /// </summary>
        public void UpdateInformation(
            IdentityDocument identityDocument,
            string firstName,
            string lastName,
            string email,
            bool hasParticipated
        )
        {
            ValidateBasicInfo(firstName, lastName, email);

            if (
                hasParticipated
                && !string.Equals(
                    IdentityDocument,
                    identityDocument.Value,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new DomainException(
                    "No se puede modificar el número de documento de identidad de este ciudadano porque ya participó en una elección.",
                    "Citizen.IdentityDocumentImmutable"
                );
            }

            IdentityDocument = identityDocument.Value;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;

        private static void ValidateBasicInfo(string firstName, string lastName, string email)
        {
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
