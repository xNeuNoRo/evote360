using System.Data;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace eVote360_Pro.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task BeginTransactionAsync(
            CancellationToken cancellationToken = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
        )
        {
            if (HasActiveTransaction)
                return;

            _currentTransaction = await _context.Database.BeginTransactionAsync(
                isolationLevel,
                cancellationToken
            );
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Primero persistimos los cambios en el Change Tracker hacia la BD
                await _context.SaveChangesAsync(cancellationToken);

                // Si se inició una transacción explícita, la confirmamos en SQL Server
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
                await DisposeTransactionAsync();
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Libera los recursos de forma asíncrona.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeTransactionAsync();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos de forma síncrona.
        /// </summary>
        public void Dispose()
        {
            DisposeTransaction();
            GC.SuppressFinalize(this);
        }

        private async Task DisposeTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        private void DisposeTransaction()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
