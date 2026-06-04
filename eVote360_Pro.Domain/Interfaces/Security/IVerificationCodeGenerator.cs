namespace eVote360_Pro.Domain.Interfaces.Security
{
    /// <summary>
    /// Contrato para la generación de códigos de verificación seguros (OTP).
    /// </summary>
    public interface IVerificationCodeGenerator
    {
        /// <summary>
        /// Genera un codigo numerico o alfanumerico aleatorio de una longitud especificada
        /// </summary>
        string Generate(int length = 6, bool useAlphanumeric = false);
    }
}
