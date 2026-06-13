using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del motor de resultados.
    /// </summary>
    public class ResultService : IResultService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IElectivePositionsRepository _positionRepository;
        private readonly ICandidatesRepository _candidateRepository;
        private readonly IPoliticalPartiesRepository _partyRepository;

        public ResultService(
            IVoteRepository voteRepository,
            IElectionRepository electionRepository,
            IElectivePositionsRepository positionRepository,
            ICandidatesRepository candidateRepository,
            IPoliticalPartiesRepository partyRepository
        )
        {
            _voteRepository = voteRepository;
            _electionRepository = electionRepository;
            _positionRepository = positionRepository;
            _candidateRepository = candidateRepository;
            _partyRepository = partyRepository;
        }

        public async Task<ResultReportResponse> GetReportAsync(Guid electionId)
        {
            var election =
                await _electionRepository.GetByIdAsync(electionId)
                ?? throw new BusinessException(
                    "El proceso electoral solicitado no existe.",
                    "Election.NotFound"
                );

            // Obtenemos las metricas clave para el reporte de resultados
            var totalVoters = await _voteRepository.GetTotalVoterParticipationAsync(electionId);

            var candidatesDict = (
                await _candidateRepository.GetAllAsync(
                    new QueryOptions<Candidate> { IsTracking = false }
                )
            ).ToDictionary(c => c.Id, c => c);
            var partiesDict = (
                await _partyRepository.GetAllAsync(
                    new QueryOptions<PoliticalParty> { IsTracking = false }
                )
            ).ToDictionary(p => p.Id, p => p);

            var positions = await _positionRepository.GetAllAsync(
                new QueryOptions<ElectivePosition> { IsTracking = false }
            );
            var positionResults = new List<PositionResultResponse>();

            foreach (var position in positions)
            {
                var distribution = await _voteRepository.GetVotesDistributionAsync(
                    electionId,
                    position.Id
                );
                var totalVotesForPosition = await _voteRepository.GetTotalVotesByPositionAsync(
                    electionId,
                    position.Id
                );

                var candidateResults = new List<CandidateResultResponse>();

                foreach (var item in distribution)
                {
                    double percentage =
                        totalVotesForPosition > 0
                            ? ((double)item.VoteCount * 100.0) / totalVotesForPosition
                            : 0;

                    string candidateName = "Ninguno (Blanco)";
                    string photoUrl = string.Empty;
                    if (
                        item.CandidateId.HasValue
                        && candidatesDict.TryGetValue(item.CandidateId.Value, out var candidate)
                    )
                    {
                        candidateName = $"{candidate.FirstName} {candidate.LastName}";
                        photoUrl = candidate.PhotoPath;
                    }

                    string partyName = string.Empty;
                    string partyAcronym = string.Empty;
                    if (
                        item.PartyId.HasValue
                        && partiesDict.TryGetValue(item.PartyId.Value, out var party)
                    )
                    {
                        partyName = party.Name;
                        partyAcronym = party.Acronym;
                    }

                    candidateResults.Add(
                        new CandidateResultResponse(
                            item.CandidateId,
                            candidateName,
                            item.PartyId,
                            partyName,
                            partyAcronym,
                            photoUrl,
                            item.VoteCount,
                            Math.Round(percentage, 2)
                        )
                    );
                }

                bool isTie = await _voteRepository.IsTieInFirstPlaceAsync(electionId, position.Id);

                positionResults.Add(position.ToPositionResult(isTie, candidateResults));
            }

            return election.ToResultReport(totalVoters, positionResults);
        }

        public async Task<ResultReportResponse?> GetDashboardSummaryAsync()
        {
            var active = await _electionRepository.GetActiveElectionAsync();
            if (active == null)
                return null;

            return await GetReportAsync(active.Id);
        }
    }
}
