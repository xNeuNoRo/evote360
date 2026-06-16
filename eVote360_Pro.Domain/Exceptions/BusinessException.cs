namespace eVote360_Pro.Domain.Exceptions
{
    /// <summary>
    /// Excepción base para errores de lógica de aplicación y orquestación.
    /// Complementa a DomainException para fallos que no ocurren dentro de una entidad.
    /// </summary>
    public class BusinessException : Exception
    {
        public string Code { get; }
        public IEnumerable<string> Errors { get; }

        public BusinessException(string message, string code)
            : base(message)
        {
            Code = code;
            Errors = Enumerable.Empty<string>();
        }

        public BusinessException(string message, string code, IEnumerable<string> errors)
            : base(message)
        {
            Code = code;
            Errors = errors;
        }
    }
}
