namespace eVote360_Pro.Domain.Interfaces.Security
{
    /// <summary>
    /// Contrato para el hashing seguro de credenciales
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Crea un hash seguro a partir de un string
        /// </summary>
        string Hash(string password);

        /// <summary>
        /// Verifica si un string coincide con un hash previamente generado
        /// </summary>
        bool Verify(string password, string hashedPassword);
    }
}
