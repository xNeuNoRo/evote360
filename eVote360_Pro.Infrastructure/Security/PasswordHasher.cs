using System.Security.Cryptography;
using System.Text;
using eVote360_Pro.Domain.Interfaces.Security;
using Konscious.Security.Cryptography;

namespace eVote360_Pro.Infrastructure.Security
{
    /// <summary>
    /// Implementación de IPasswordHasher utilizando el algoritmo Argon2id para el hashing de contraseñas.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        // El paralelismo del algoritmo argon2, degree 8 es el recomendado pero se puede ajustar por rendimiento
        private const int DegreeOfParallelism = 8;

        // Cantidad de memoria a usar en KB, 64MB es el recomendado pero se puede ajustar por rendimiento
        private const int MemorySize = 65536;

        // Cantidad de iteraciones, 4 es el recomendado pero se puede ajustar por rendimiento
        private const int Iterations = 4;

        // Tamaño del Salt, 16 bytes es el recomendado
        private const int SaltSize = 16;

        // Tamaño del Hash, 32 bytes es el recomendado
        private const int HashSize = 32;

        public string Hash(string password)
        {
            var salt = GenerateSalt();
            var hash = GenerateHash(password, salt);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public bool Verify(string password, string hashedPassword)
        {
            try
            {
                // El formato del hashedPassword es "salt:hash", ambos en Base64. Separamos el salt y el hash.
                var parts = hashedPassword.Split(':');

                // Si el formato no es correcto, no se puede verificar la contraseña
                if (parts.Length != 2)
                    return false;

                // Convertimos el salt y el hash de Base64 a byte arrays para poder compararlos
                var salt = Convert.FromBase64String(parts[0]);
                var expectedHash = Convert.FromBase64String(parts[1]);

                // Generamos el hash de la contraseña proporcionada usando el mismo salt
                var actualHash = GenerateHash(password, salt);

                // Comparamos el hash generado con el hash almacenado utilizando una
                // comparación de tiempo constante para evitar ataques de timing
                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Genera un salt criptográficamente seguro y un hash de la contraseña usando Argon2id.
        /// </summary>
        private static byte[] GenerateSalt()
        {
            var buffer = new byte[SaltSize];
            RandomNumberGenerator.Fill(buffer);
            return buffer;
        }

        /// <summary>
        /// Genera un hash de la contraseña utilizando el algoritmo Argon2id
        /// con el salt proporcionado y los parámetros de configuración definidos.
        /// </summary>
        private static byte[] GenerateHash(string password, byte[] salt)
        {
            // Obtenemos los bytes de la contraseña en UTF-8
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Creamos una instancia de Argon2id con la contraseña y configuramos los parámetros
            using var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                MemorySize = MemorySize,
                Iterations = Iterations,
            };

            // Generamos el hash de la contraseña y lo devolvemos
            return argon2.GetBytes(HashSize);
        }
    }
}
