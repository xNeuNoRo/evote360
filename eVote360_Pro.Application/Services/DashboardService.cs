using eVote360_Pro.Application.ViewModels.DashboardViewModels;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Application.Services
{
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

        public async Task<DashboardViewModel> GetDashboardAsync()
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

            return new DashboardViewModel
            {
                TotalElections = totalElections,
                ActiveElectionName = activeElectionName,
                VoterParticipationCount = voterParticipationCount,
                TotalCitizens = totalCitizens,
                TotalParties = totalParties,
            };
        }

        public async Task<ResultReportResponse?> GetAdminDashboardAsync(int electoralYear)
        {
            var allElections = await _electionRepository.GetAllAsync(
                new QueryOptions<Election>
                {
                    IsTracking = false,
                }
            );

            var election = allElections.FirstOrDefault(e =>
                e.RealizationDate.Year == electoralYear && !e.IsActive
            );

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

