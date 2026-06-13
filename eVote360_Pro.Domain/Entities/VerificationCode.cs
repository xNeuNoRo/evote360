using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un código de verificación (OTP) con ID seguro y lógica de tiempo externa.
    /// </summary>
    public class VerificationCode : BaseEntity<Guid>
    {
        public int CitizenId { get; private set; }
        public Guid ElectionId { get; private set; }
        public string Code { get; private set; } = null!;
        public DateTime ExpirationDate { get; private set; }
        public bool IsUsed { get; private set; }

        // Navigation Properties
        public virtual Citizen Citizen { get; private set; } = null!;
        public virtual Election Election { get; private set; } = null!;

        // Constructor privado para EF Core y Factory Method
        private VerificationCode() { }

        /// <summary>
        /// Crea un nuevo código de verificación.
        /// </summary>
        public static VerificationCode Create(
            int citizenId,
            Guid electionId,
            string code,
            DateTime currentTime,
            int expiryMinutes = 5
        )
        {
            if (citizenId <= 0)
                throw new DomainException(
                    "El ciudadano es requerido.",
                    "VerificationCode.CitizenRequired"
                );

            if (electionId == Guid.Empty)
                throw new DomainException(
                    "La elección es requerida.",
                    "VerificationCode.ElectionRequired"
                );

            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException(
                    "El código es requerido.",
                    "VerificationCode.CodeRequired"
                );

            return new VerificationCode
            {
                Id = Guid.NewGuid(),
                CitizenId = citizenId,
                ElectionId = electionId,
                Code = code,
                ExpirationDate = currentTime.AddMinutes(expiryMinutes),
                IsUsed = false,
            };
        }

        /// <summary>
        /// Marca el código como utilizado tras validar la expiración con el tiempo actual.
        /// </summary>
        public void Use(DateTime currentTime)
        {
            if (IsUsed)
                throw new DomainException(
                    "Este código ya fue utilizado.",
                    "VerificationCode.AlreadyUsed"
                );

            if (currentTime > ExpirationDate)
                throw new DomainException("Este código ha expirado.", "VerificationCode.Expired");

            IsUsed = true;
        }

        /// <summary>
        /// Invalida el código forzosamente sin validaciones de tiempo.
        /// Útil para descartar códigos previos cuando se genera uno nuevo.
        /// </summary>
        public void Invalidate()
        {
            IsUsed = true;
        }
    }
}
