using System.Text.RegularExpressions;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa una política de contraseña segura.
    /// Valida los requisitos de complejidad antes de permitir el hashing.
    /// </summary>
    public record PasswordPolicy
    {
        public string PlainText { get; init; }

        private PasswordPolicy(string plainText)
        {
            PlainText = plainText;
        }

        public static PasswordPolicy Create(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText) || plainText.Length < 8)
                throw new DomainException(
                    "La contraseña debe tener al menos 8 caracteres.",
                    "User.PasswordTooShort"
                );

            if (!Regex.IsMatch(plainText, @"^(?=.*[a-zA-Z])(?=.*\d).+$"))
                throw new DomainException(
                    "La contraseña debe contener al menos una letra y un número.",
                    "User.PasswordTooWeak"
                );

            return new PasswordPolicy(plainText);
        }
    }
}
