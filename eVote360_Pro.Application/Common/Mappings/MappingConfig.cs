using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Responses;
using eVote360_Pro.Application.DTOs.Citizen.Responses;
using eVote360_Pro.Application.DTOs.Election.Responses;
using eVote360_Pro.Application.DTOs.ElectivePosition.Responses;
using eVote360_Pro.Application.DTOs.PoliticalAlliance.Responses;
using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Responses;
using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;
using eVote360_Pro.Application.DTOs.User.Responses;
using eVote360_Pro.Domain.Entities;
using Mapster;

namespace eVote360_Pro.Application.Common.Mappings
{
    /// <summary>
    /// Configuración centralizada de mapeos automatizados con Mapster.
    /// Define las reglas de transformación entre Entidades de Dominio y DTOs de salida.
    /// </summary>
    public static class MappingConfig
    {
        public static void Configure()
        {
            // Usuarios
            TypeAdapterConfig<User, UserResponse>
                .NewConfig()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .Map(dest => dest.RoleName, src => src.Role != null ? src.Role.Name : "N/A")
                .Map(
                    dest => dest.AssignedPartyId,
                    src => src.LeaderAssignment != null ? (int?)src.LeaderAssignment.PartyId : null
                )
                .Map(
                    dest => dest.AssignedPartyName,
                    src => src.LeaderAssignment != null ? src.LeaderAssignment.Party.Name : null
                );

            // Ciudadanos
            TypeAdapterConfig<Citizen, CitizenResponse>
                .NewConfig()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .Map(dest => dest.IsIdentityDocumentImmutable, src => src.Participations.Any())
                .Map(
                    dest => dest.HasVotedInActiveElection,
                    src =>
                        MapContext.Current != null
                        && MapContext.Current.Parameters.ContainsKey("ActiveElectionId")
                        && src.Participations.Any(p =>
                            p.ElectionId == (Guid)MapContext.Current.Parameters["ActiveElectionId"]
                        )
                );

            // Candidatos
            TypeAdapterConfig<Candidate, CandidateResponse>
                .NewConfig()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .Map(dest => dest.PhotoUrl, src => src.PhotoPath)
                .Map(
                    dest => dest.OriginalPartyName,
                    src => src.OriginalParty != null ? src.OriginalParty.Name : "N/A"
                )
                .Map(dest => dest.IsImmutable, src => src.Votes.Any());

            // Partidos Políticos
            TypeAdapterConfig<PoliticalParty, PoliticalPartyResponse>
                .NewConfig()
                .Map(dest => dest.LogoUrl, src => src.LogoPath)
                .Map(
                    dest => dest.IsImmutable,
                    src => src.Votes.Any() || src.CandidatePostAssignments.Any()
                )
                .Map(dest => dest.HasLeaderAssigned, src => src.LeaderAssignment != null);

            // Procesos Electorales
            TypeAdapterConfig<Election, ElectionResponse>
                .NewConfig()
                .Map(dest => dest.Year, src => src.RealizationDate.Year)
                .Map(
                    dest => dest.CanActivate,
                    src =>
                        MapContext.Current != null
                        && MapContext.Current.Parameters.ContainsKey("CanActivate")
                            ? (bool)MapContext.Current.Parameters["CanActivate"]
                            : false
                )
                .Map(
                    dest => dest.MissingParties,
                    src =>
                        MapContext.Current != null
                        && MapContext.Current.Parameters.ContainsKey("MissingParties")
                            ? (IEnumerable<string>)MapContext.Current.Parameters["MissingParties"]
                            : Enumerable.Empty<string>()
                );

            // Puestos Electivos
            TypeAdapterConfig<ElectivePosition, ElectivePositionResponse>
                .NewConfig()
                .Map(
                    dest => dest.IsImmutable,
                    src => src.Votes.Any() || src.CandidatePostAssignments.Any()
                );

            // Alianzas Políticas
            TypeAdapterConfig<PoliticalAlliance, AllianceResponse>
                .NewConfig()
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.RequesterPartyName, src => src.RequesterParty.Name)
                .Map(dest => dest.ReceiverPartyName, src => src.ReceiverParty.Name);

            // Asignaciones de Candidatos a Puestos
            TypeAdapterConfig<CandidatePostAssignment, BallotAssignmentResponse>
                .NewConfig()
                .Map(
                    dest => dest.CandidateName,
                    src => $"{src.Candidate.FirstName} {src.Candidate.LastName}"
                )
                .Map(dest => dest.PositionName, src => src.Position.Name)
                .Map(
                    dest => dest.CandidateOriginalPartyName,
                    src => src.Candidate.OriginalParty.Name
                )
                .Map(dest => dest.CandidatePhotoUrl, src => src.Candidate.PhotoPath);

            // Asignaciones de Líderes a Partidos
            TypeAdapterConfig<PoliticalLeaderAssignment, LeaderAssignmentResponse>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.User.Username)
                .Map(dest => dest.UserFullName, src => $"{src.User.FirstName} {src.User.LastName}")
                .Map(dest => dest.PartyName, src => src.Party.Name)
                .Map(dest => dest.PartyAcronym, src => src.Party.Acronym);
        }
    }
}
