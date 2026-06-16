using eVote360_Pro.Application.DTOs.Dashboard.Responses;
using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de Dashboards.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly ICitizenRepository _citizenRepository;
        private readonly IPoliticalPartiesRepository _politicalPartiesRepository;
        private readonly IVoterParticipationRepository _voterParticipationRepository;
        private readonly IResultService _resultService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPoliticalLeaderAssignmentsRepository _leaderAssignmentRepository;
        private readonly ICandidatesRepository _candidatesRepository;
        private readonly IPoliticalAlliancesRepository _alliancesRepository;
        private readonly ICandidatePostAssignmentsRepository _postAssignmentsRepository;

        public DashboardService(
            IElectionRepository electionRepository,
            ICitizenRepository citizenRepository,
            IPoliticalPartiesRepository politicalPartiesRepository,
            IVoterParticipationRepository voterParticipationRepository,
            IResultService resultService,
            ICurrentUserService currentUserService,
            IPoliticalLeaderAssignmentsRepository leaderAssignmentRepository,
            ICandidatesRepository candidatesRepository,
            IPoliticalAlliancesRepository alliancesRepository,
            ICandidatePostAssignmentsRepository postAssignmentsRepository
        )
        {
            _electionRepository = electionRepository;
            _citizenRepository = citizenRepository;
            _politicalPartiesRepository = politicalPartiesRepository;
            _voterParticipationRepository = voterParticipationRepository;
            _resultService = resultService;
            _currentUserService = currentUserService;
            _leaderAssignmentRepository = leaderAssignmentRepository;
            _candidatesRepository = candidatesRepository;
            _alliancesRepository = alliancesRepository;
            _postAssignmentsRepository = postAssignmentsRepository;
        }

        public async Task<DashboardStatisticsResponse> GetGeneralStatisticsAsync()
        {
            var totalCitizens = await _citizenRepository.CountAsync();
            var totalParties = await _politicalPartiesRepository.CountAsync();
            var totalElections = await _electionRepository.CountAsync();

            var activeElection = await _electionRepository.GetActiveElectionAsync();

            int voterParticipationCount = 0;
            string? activeElectionName = null;

            if (activeElection != null)
            {
                activeElectionName = activeElection.Name;
                voterParticipationCount =
                    await _voterParticipationRepository.GetTotalVotersByElectionAsync(
                        activeElection.Id
                    );
            }

            var recentCitizens = await _citizenRepository.GetAllAsync(new QueryOptions<Citizen>
            {
                IsTracking = false
            });
            var recentCitizensDto = recentCitizens.OrderByDescending(c => c.Id).Take(5)
                .Select(c => new RecentCitizenDto($"{c.FirstName} {c.LastName}", c.IdentityDocument, "Reciente"))
                .ToList();

            var recentParties = await _politicalPartiesRepository.GetAllAsync(new QueryOptions<PoliticalParty>
            {
                IsTracking = false
            });
            var recentPartiesDto = recentParties.OrderByDescending(p => p.Id).Take(5)
                .Select(p => new RecentPartyDto(p.Name, p.Acronym, p.LogoPath))
                .ToList();

            return new DashboardStatisticsResponse(
                TotalElections: totalElections,
                ActiveElectionName: activeElectionName,
                VoterParticipationCount: voterParticipationCount,
                TotalCitizens: totalCitizens,
                TotalParties: totalParties,
                RecentCitizens: recentCitizensDto,
                RecentParties: recentPartiesDto
            );
        }

        public async Task<List<ElectionSummaryResponse>> GetAdminDashboardAsync(int electoralYear)
        {
            var options = new QueryOptions<Election>
            {
                Filter = e =>
                    e.RealizationDate.Year == electoralYear && e.Status == ElectionStatus.Finished,
                Includes = new List<System.Linq.Expressions.Expression<Func<Election, object>>>
                {
                    e => e.Votes,
                    e => e.Participations
                },
                IsTracking = false,
            };

            var elections = await _electionRepository.GetAllAsync(options);

            var result = elections.Select(e => new ElectionSummaryResponse(
                Id: e.Id,
                Name: e.Name,
                RealizationDate: e.RealizationDate,
                ParticipatingPartiesCount: e.Votes.Select(v => v.PartyId).Distinct().Count(),
                RealCandidatesCount: e.Votes.Select(v => v.CandidateId).Distinct().Count(),
                VoterParticipationCount: e.Participations.Count
            )).ToList();

            return result;
        }

        public async Task<ResultReportResponse?> GetLeaderDashboardAsync()
        {
            return await _resultService.GetDashboardSummaryAsync();
        }

        public async Task<LeaderDashboardStatisticsResponse> GetLeaderStatisticsAsync()
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("Usuario no autenticado");

            var assignmentOpts = new QueryOptions<PoliticalLeaderAssignment>
            {
                Filter = a => a.Id == userId.Value,
                IsTracking = false,
            };
            var assignment = await _leaderAssignmentRepository.GetFirstOrDefaultAsync(
                assignmentOpts
            );

            if (assignment == null)
            {
                return new LeaderDashboardStatisticsResponse(0, 0, 0, 0, 0, new(), new());
            }

            var partyId = assignment.PartyId;

            var candidatesOpts = new QueryOptions<Candidate>
            {
                Filter = c => c.OriginalPartyId == partyId,
                IsTracking = false,
            };
            var allCandidates = await _candidatesRepository.GetAllAsync(candidatesOpts);
            int activeCandidates = allCandidates.Count(c => c.IsActive);
            int inactiveCandidates = allCandidates.Count(c => !c.IsActive);

            var recentCandidates = allCandidates.OrderByDescending(c => c.Id).Take(5)
                .Select(c => new RecentCandidateDto($"{c.FirstName} {c.LastName}", "Candidato", c.PhotoPath))
                .ToList();

            var allAlliancesOpts = new QueryOptions<PoliticalAlliance>
            {
                Filter = a => (a.RequesterPartyId == partyId || a.ReceiverPartyId == partyId) && a.Status == AllianceStatus.Accepted,
                Includes = new() { a => a.RequesterParty, a => a.ReceiverParty },
                IsTracking = false
            };
            var allAlliances = await _alliancesRepository.GetAllAsync(allAlliancesOpts);
            
            var approvedAlliances = allAlliances.Count();
            
            var alliedParties = allAlliances.Take(5).Select(a => {
                var otherParty = a.RequesterPartyId == partyId ? a.ReceiverParty : a.RequesterParty;
                return new AlliedPartyDto(otherParty.Name, otherParty.Acronym, otherParty.LogoPath);
            }).ToList();

            var pendingAlliances = await _alliancesRepository.CountAsync(a =>
                a.ReceiverPartyId == partyId && a.Status == AllianceStatus.Pending
            );

            var assignmentsOpts = new QueryOptions<CandidatePostAssignment>
            {
                Filter = a => a.PartyId == partyId,
                IsTracking = false,
            };
            var assignedCandidates = await _postAssignmentsRepository.CountAsync(
                assignmentsOpts.Filter
            );

            return new LeaderDashboardStatisticsResponse(
                activeCandidates,
                inactiveCandidates,
                approvedAlliances,
                pendingAlliances,
                assignedCandidates,
                recentCandidates,
                alliedParties
            );
        }
    }
}
