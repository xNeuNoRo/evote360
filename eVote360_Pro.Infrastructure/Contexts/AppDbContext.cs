using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Constructor alternativo pa que el EFC en las migraciones no se queje
        /// de que no encuentra el IDateTimeProvider, ya que en las migraciones no se inyectan dependencias.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : this(options, new DateTimeProvider()) { }

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IDateTimeProvider dateTimeProvider
        )
            : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        // =====================================
        // DbSet de todas las entidades
        // =====================================
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PoliticalParty> PoliticalParties { get; set; }
        public DbSet<ElectivePosition> ElectivePositions { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidatePostAssignment> CandidatePostAssignments { get; set; }
        public DbSet<PoliticalLeaderAssignment> PoliticalLeaderAssignments { get; set; }
        public DbSet<PoliticalAlliance> PoliticalAlliances { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoterParticipation> VoterParticipations { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // En lugar de andar creando 1 por 1 cada configuración, buscamos todas
            // las clases que implementen IEntityTypeConfiguration<T> y las aplique automáticamente
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                var now = _dateTimeProvider.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
