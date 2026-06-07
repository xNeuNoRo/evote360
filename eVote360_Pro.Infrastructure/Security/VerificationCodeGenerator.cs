using System.Security.Cryptography;
using System.Text;
using eVote360_Pro.Domain.Interfaces.Security;

namespace eVote360_Pro.Infrastructure.Security
{
    /// <summary>
    /// Implementación del generador de códigos de verificación.
    /// Produce códigos aleatorios seguros y legibles para procesos OTP.
    /// </summary>
    public class VerificationCodeGenerator : IVerificationCodeGenerator
    {
        private const string NumericChars = "0123456789";
        private const string AlphanumericChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string Generate(int length = 6, bool useAlphanumeric = false)
        {
            if (length <= 0)
                length = 6;

            var characterSet = useAlphanumeric ? AlphanumericChars : NumericChars;
            var result = new StringBuilder(length);

            // Generamos cada char de forma segura
            for (int i = 0; i < length; i++)
            {
                // Obtenemos un índice aleatorio dentro del rango del characterSet
                int index = RandomNumberGenerator.GetInt32(characterSet.Length);
                // Agregamos el carácter correspondiente al resultado
                result.Append(characterSet[index]);
            }

            // Devolvemos el código generado como string
            return result.ToString();
        }
    }
}
