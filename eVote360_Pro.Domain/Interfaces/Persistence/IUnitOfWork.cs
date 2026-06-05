using System.Data;

namespace eVote360_Pro.Domain.Interfaces.Persistence
{
    /// <summary>
    /// Define el contrato para la gestión de transacciones y persistencia atómica.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Indica si hay una transacción activa en curso.
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// Inicia una nueva transacción con el nivel de aislamiento especificado. Si ya hay una transacción activa, se reutilizará esa transacción en lugar de iniciar una nueva.
        /// </summary>
        Task BeginTransactionAsync(
            CancellationToken cancellationToken = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted // ReadCommitted es el default ideal
        );

        /// <summary>
        /// Confirma todos los cambios realizados en el contexto y completa la transacción
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Revierte todos los cambios realizados en el contexto y cancela la transacción
        /// </summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Persiste los cambios realizados en la bd sin necesidad de manejar transacciones explícitas.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
