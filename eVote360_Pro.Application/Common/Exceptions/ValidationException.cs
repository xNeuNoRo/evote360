using System.Collections.Generic;

namespace eVote360_Pro.Application.Common.Exceptions
{
    /// <summary>
    /// Excepción especializada para errores de validación de entrada.
    /// </summary>
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Uno o más errores de validación han ocurrido.")
        {
            Errors = errors;
        }
    }
}
