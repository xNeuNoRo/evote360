using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Interfaces.Security;
using eVote360_Pro.Infrastructure.Contexts;
using eVote360_Pro.Infrastructure.Messaging;
using eVote360_Pro.Infrastructure.OCR;
using eVote360_Pro.Infrastructure.Persistence;
using eVote360_Pro.Infrastructure.Providers;
using eVote360_Pro.Infrastructure.Repositories;
using eVote360_Pro.Infrastructure.Security;
using eVote360_Pro.Infrastructure.Storage;
using eVote360_Pro.Shared.Interfaces.Messaging;
using eVote360_Pro.Shared.Interfaces.OCR;
using eVote360_Pro.Shared.Interfaces.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360_Pro.Infrastructure
{
    /// <summary>
    /// Registra todos los servicios de infraestructura y persistencia en el contenedor de dependencias.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Configuracion de la Base de Datos (SQL Server)
            var connectionString = configuration.GetConnectionString("eVote360Db");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                )
            );

            // Persistencia
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositorios
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<
                ICandidatePostAssignmentsRepository,
                CandidatePostAssignmentRepository
            >();
            services.AddScoped<ICandidatesRepository, CandidateRepository>();
            services.AddScoped<ICitizenRepository, CitizenRepository>();
            services.AddScoped<IElectionRepository, ElectionRepository>();
            services.AddScoped<IElectivePositionsRepository, ElectivePositionRepository>();
            services.AddScoped<IPoliticalAlliancesRepository, PoliticalAllianceRepository>();
            services.AddScoped<
                IPoliticalLeaderAssignmentsRepository,
                PoliticalLeaderAssignmentRepository
            >();
            services.AddScoped<IPoliticalPartiesRepository, PoliticalPartyRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<IVoterParticipationRepository, VoterParticipationRepository>();

            // Seguridad y Auth
            services.AddHttpContextAccessor();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IVerificationCodeGenerator, VerificationCodeGenerator>();

            // Proveedores
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Servicios de Seguridad
            services.Configure<FileSettings>(configuration.GetSection(FileSettings.SectionName));
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IOcrService, OcrService>();

            // Configuracion de Email (MailKit y RazorLight)
            services.AddSingleton<IRazorRenderer, RazorRenderer>();
            services.AddScoped<IEmailService, MailKitEmailService>();

            return services;
        }
    }
}
