using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using eVote360_Pro.Domain.Entities;

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
        public DbSet<PoliticalParties> PoliticalParties { get; set; }
        public DbSet<ElectivePositions> ElectivePositions { get; set; }
        public DbSet<Candidates> Candidates { get; set; }
        public DbSet<PoliticalLeaderAssignments> PoliticalLeaderAssignments { get; set; }
        public DbSet<PoliticalAlliances> PoliticalAlliances { get; set; }
        public DbSet<CandidatePostAssignments> CandidatePostAssignments { get; set; }

        /// <summary>
        /// Metodo que se ejecuta cada vez que se llama a SaveChangesAsync en el contexto de la base de datos.
        /// Este método se encarga de actualizar automáticamente las propiedades de auditoría (CreatedAt y UpdatedAt)
        /// de las entidades que heredan de BaseEntity.
        /// </summary>
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            // Filtramos todas las entidades que hereden de BaseEntity y hayan sido modificadas o agregadas
            var entries = ChangeTracker.Entries<BaseEntity>();

            // Por cada entidad que hereda del BaseEntity en el ChangeTracker de EF Core
            foreach (var entry in entries)
            {
                // Dependiendo del estado de la entidad (Added o Modified), actualizamos las propiedades de auditoría
                switch (entry.State)
                {
                    // Si la entidad es nueva (Added), asignamos la fecha de creación (CreatedAt) con la fecha y hora actual en UTC
                    case EntityState.Added:
                        entry.Entity.CreatedAt = _dateTimeProvider.UtcNow;
                        break;
                    // Si la entidad ha sido modificada (Modified),
                    // asignamos la fecha de actualización (UpdatedAt) con la fecha y hora actual en UTC
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = _dateTimeProvider.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // En lugar de andar creando 1 por 1 cada configuración, buscamos todas
            // las clases que implementen IEntityTypeConfiguration<T> y las aplique automáticamente
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
