using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de códigos de verificación.
    /// Gestiona la seguridad del acceso OTP del elector.
    /// </summary>
    public class VerificationCodeRepository
        : GenericRepository<VerificationCode, Guid>,
            IVerificationCodeRepository
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public VerificationCodeRepository(AppDbContext context, IDateTimeProvider dateTimeProvider)
            : base(context)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<VerificationCode?> GetValidCodeAsync(
            int citizenId,
            Guid electionId,
            string code
        )
        {
            return await _dbSet.FirstOrDefaultAsync(v =>
                v.CitizenId == citizenId
                && v.ElectionId == electionId
                && v.Code == code
                && !v.IsUsed
            );
        }

        public async Task InvalidatePreviousCodesAsync(int citizenId, Guid electionId)
        {
            // Marcamos todos los códigos previos como usados para que solo el último sea válido
            var previousCodes = await _dbSet
                .Where(v => v.CitizenId == citizenId && v.ElectionId == electionId && !v.IsUsed)
                .ToListAsync();

            foreach (var code in previousCodes)
            {
                code.Use(_dateTimeProvider.UtcNow);
            }

            // El guardado se lo delegamos al UnitOfWork.SaveChangesAsync()
        }

        public async Task<int> CountRecentRequestsAsync(
            int citizenId,
            Guid electionId,
            DateTime since
        )
        {
            return await _dbSet.CountAsync(v =>
                v.CitizenId == citizenId && v.ElectionId == electionId && v.CreatedAt >= since
            );
        }
    }
}
