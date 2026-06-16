using System;
using System.Collections.Generic;
using System.Linq;

namespace eVote360_Pro.Domain.Exceptions
{
    /// <summary>
    /// Excepción base para violaciones de reglas de negocio en el Dominio.
    /// Incluye un código de error único para facilitar el manejo en el Frontend y Middleware.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Código único que identifica el tipo de error (ej: 'PoliticalParty.AlreadyParticipated').
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Lista detallada de fallos si la excepción agrupa múltiples errores.
        /// </summary>
        public IEnumerable<string> Errors { get; }

        public DomainException(string message, string code) : base(message)
        {
            Code = code;
            Errors = Enumerable.Empty<string>();
        }

        public DomainException(string message, string code, IEnumerable<string> errors) : base(message)
        {
            Code = code;
            Errors = errors;
        }
    }
}
