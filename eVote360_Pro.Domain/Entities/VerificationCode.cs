using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un código de verificación (OTP) enviado por correo electrónico.
    /// Encapsula las reglas de expiración y uso único.
    /// </summary>
    public class VerificationCode : BaseEntity
    {
        public int CitizenId { get; private set; }
        public int ElectionId { get; private set; }
        public string Code { get; private set; } = null!;
        public DateTime ExpirationDate { get; private set; }
        public bool IsUsed { get; private set; }

        // Navigation Properties
        public virtual Citizen Citizen { get; private set; } = null!;
        public virtual Election Election { get; private set; } = null!;

        // Constructor privado para EF Core y Factory Method
        private VerificationCode() { }

        /// <summary>
        /// Crea un nuevo código de verificación con un tiempo de expiración estándar.
        /// </summary>
        public static VerificationCode Create(
            int citizenId,
            int electionId,
            string code,
            int expiryMinutes = 5
        )
        {
            if (citizenId <= 0)
                throw new DomainException(
                    "El ciudadano es requerido.",
                    "VerificationCode.CitizenRequired"
                );

            if (electionId <= 0)
                throw new DomainException(
                    "La elección es requerida.",
                    "VerificationCode.ElectionRequired"
                );

            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException(
                    "El código no puede estar vacío.",
                    "VerificationCode.CodeRequired"
                );

            return new VerificationCode
            {
                CitizenId = citizenId,
                ElectionId = electionId,
                Code = code,
                ExpirationDate = DateTime.UtcNow.AddMinutes(expiryMinutes),
                IsUsed = false,
            };
        }

        /// <summary>
        /// Marca el código como utilizado tras validar que no haya expirado ni haya sido usado previamente.
        /// </summary>
        public void Use()
        {
            if (IsUsed)
                throw new DomainException(
                    "Este código de verificación ya fue utilizado.",
                    "VerificationCode.AlreadyUsed"
                );

            if (IsExpired)
                throw new DomainException(
                    "Este código de verificación ha expirado.",
                    "VerificationCode.Expired"
                );

            IsUsed = true;
        }

        /// <summary>
        /// Indica si el código ha sobrepasado su fecha de expiración.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > ExpirationDate;
    }
}
