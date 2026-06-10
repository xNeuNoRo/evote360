using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    
    public class ResultService : IResultService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IElectivePositionsRepository _positionRepository;

        public ResultService(
            IVoteRepository voteRepository,
            IElectionRepository electionRepository,
            IElectivePositionsRepository positionRepository
        )
        {
            _voteRepository = voteRepository;
            _electionRepository = electionRepository;
            _positionRepository = positionRepository;
        }

        public async Task<ResultReportResponse> GetReportAsync(Guid electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
                throw new Exception("Election not found");

            // Total de votantes
            var totalVoters = await _voteRepository.GetTotalVoterParticipationAsync(electionId);

            var positions = await _positionRepository.GetAllAsync();

            var positionResults = new List<PositionResultResponse>();

            foreach (var position in positions)
            {
                // Distribución de votos por opción
                var distribution = await _voteRepository.GetVotesDistributionAsync(electionId, position.Id);

                // Sumar votos válidos (todas las opciones) y votos en blanco (CandidateId null)
                int totalValid = distribution.Sum(d => d.VoteCount);
                int blanks = distribution.Where(d => d.CandidateId == null).Sum(d => d.VoteCount);

                var candidates = new List<CandidateResultResponse>();

                foreach (var item in distribution)
                {
                    var votes = item.VoteCount;
                    double percentage = 0;
                    if (totalVoters > 0)
                        percentage = (double)votes * 100.0 / (double)totalVoters;

                    // Obtener nombres y foto del candidato/partido si aplica
                    string candidateName = item.CandidateId.HasValue ? $"Candidato {item.CandidateId.Value}" : "Blanco";
                    string partyName = item.PartyId.HasValue ? $"Partido {item.PartyId.Value}" : string.Empty;

                    candidates.Add(
                        new CandidateResultResponse(
                            item.CandidateId,
                            candidateName,
                            item.PartyId,
                            partyName,
                            string.Empty,
                            votes,
                            Math.Round(percentage, 2)
                        )
                    );
                }

                // Detectar empate en primer lugar
                var isTie = await _voteRepository.IsTieInFirstPlaceAsync(electionId, position.Id);

                // Mapear posición
                var posResult = position.ToPositionResult(isTie, candidates);
                positionResults.Add(posResult);
            }

            return election.ToResultReport(totalVoters, positionResults);
        }

        public async Task<ResultReportResponse?> GetDashboardSummaryAsync()
        {
            var active = await _electionRepository.GetActiveElectionAsync();
            if (active == null) return null;

            return await GetReportAsync(active.Id);
        }
    }
}
