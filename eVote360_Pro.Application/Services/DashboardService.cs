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

            return new DashboardStatisticsResponse(
                TotalElections: totalElections,
                ActiveElectionName: activeElectionName,
                VoterParticipationCount: voterParticipationCount,
                TotalCitizens: totalCitizens,
                TotalParties: totalParties
            );
        }

        public async Task<ResultReportResponse?> GetAdminDashboardAsync(int electoralYear)
        {
            var options = new QueryOptions<Election>
            {
                Filter = e =>
                    e.RealizationDate.Year == electoralYear && e.Status == ElectionStatus.Finished,
                IsTracking = false,
            };

            var election = await _electionRepository.GetFirstOrDefaultAsync(options);

            if (election == null)
                return null;

            return await _resultService.GetReportAsync(election.Id);
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
                return new LeaderDashboardStatisticsResponse(0, 0, 0, 0, 0);
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

            var approvedAlliances = await _alliancesRepository.CountAsync(a =>
                (a.RequesterPartyId == partyId || a.ReceiverPartyId == partyId)
                && a.Status == AllianceStatus.Accepted
            );

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
                assignedCandidates
            );
        }
    }
}
