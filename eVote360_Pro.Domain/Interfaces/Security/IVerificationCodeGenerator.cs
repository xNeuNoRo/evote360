namespace eVote360_Pro.Domain.Interfaces.Security
{
    public interface IVerificationCodeGenerator
    {
        /// <summary>
        /// Genera un codigo numerico o alfanumerico aleatorio de una longitud especificada
        /// </summary>
        string Generate(int length = 6, bool useAlphanumeric = false);
    }
}
