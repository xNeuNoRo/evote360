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

        public DashboardService(
            IElectionRepository electionRepository,
            ICitizenRepository citizenRepository,
            IPoliticalPartiesRepository politicalPartiesRepository,
            IVoterParticipationRepository voterParticipationRepository,
            IResultService resultService
        )
        {
            _electionRepository = electionRepository;
            _citizenRepository = citizenRepository;
            _politicalPartiesRepository = politicalPartiesRepository;
            _voterParticipationRepository = voterParticipationRepository;
            _resultService = resultService;
        }

        public async Task<DashboardStatisticsResponse> GetGeneralStatisticsAsync()
        {
            var totalCitizensTask = _citizenRepository.CountAsync();
            var totalPartiesTask = _politicalPartiesRepository.CountAsync();
            var totalElectionsTask = _electionRepository.CountAsync();

            await Task.WhenAll(totalCitizensTask, totalPartiesTask, totalElectionsTask);

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
                TotalElections: totalElectionsTask.Result,
                ActiveElectionName: activeElectionName,
                VoterParticipationCount: voterParticipationCount,
                TotalCitizens: totalCitizensTask.Result,
                TotalParties: totalPartiesTask.Result
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
    }
}
