namespace eVote360_Pro.Domain.Exceptions
{
    /// <summary>
    /// Excepción específica para fallos de reglas de negocio asociados a un campo o propiedad específica.
    /// Útil para devolver errores que el frontend puede mapear a un input específico.
    /// </summary>
    public class ValidationBusinessException : BusinessException
    {
        public string PropertyName { get; }

        public ValidationBusinessException(string propertyName, string message, string code)
            : base(message, code)
        {
            PropertyName = propertyName;
        }
    }
}
