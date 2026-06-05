using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa un documento de identidad dominicano válido.
    /// </summary>
    public record IdentityDocument
    {
        public string Value { get; init; }

        private IdentityDocument(string value)
        {
            Value = value;
        }

        public static IdentityDocument Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(
                    "El documento de identidad es requerido.",
                    "Citizen.IdentityDocumentRequired"
                );

            string cleaned = value.Replace("-", "").Replace(" ", "");

            if (!IsValidCedula(cleaned))
                throw new DomainException(
                    "El número de documento de identidad no es válido.",
                    "Citizen.InvalidIdentityDocument"
                );

            return new IdentityDocument(cleaned);
        }

        private static bool IsValidCedula(string cleaned)
        {
            if (cleaned.Length != 11 || !long.TryParse(cleaned, out _))
                return false;

            int sum = 0;
            int[] weights = { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };

            for (int i = 0; i < 10; i++)
            {
                int digit = int.Parse(cleaned[i].ToString()) * weights[i];
                if (digit > 9)
                    digit = (digit / 10) + (digit % 10);
                sum += digit;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == int.Parse(cleaned[10].ToString());
        }

        public override string ToString() => Value;
    }
}
