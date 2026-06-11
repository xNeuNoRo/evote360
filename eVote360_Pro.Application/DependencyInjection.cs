using System.Reflection;
using eVote360_Pro.Application.Common.Internal;
using eVote360_Pro.Application.Common.Mappings;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360_Pro.Application
{
    /// <summary>
    /// Registra los servicios de la capa de aplicación en el contenedor de inyección de dependencias.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registramos los servicios
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICitizenService, CitizenService>();
            services.AddScoped<IPoliticalPartyService, PoliticalPartyService>();
            services.AddScoped<ICandidateService, CandidateService>();
            services.AddScoped<IElectivePositionService, ElectivePositionService>();
            services.AddScoped<IPoliticalAllianceService, PoliticalAllianceService>();
            services.AddScoped<ICandidatePostAssignmentService, CandidatePostAssignmentService>();
            services.AddScoped<
                IPoliticalLeaderAssignmentService,
                PoliticalLeaderAssignmentService
            >();
            services.AddScoped<IElectionService, ElectionService>();
            services.AddScoped<IVotingService, VotingService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // Registramos el traductor de excepciones para manejo centralizado de errores
            services.AddSingleton<ExceptionTranslator>();

            // Registramos el validador de modelos usando FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Ejecuta la configuración estática de los adaptadores de tipos
            MappingConfig.Configure();

            return services;
        }
    }
}
