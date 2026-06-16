namespace eVote360_Pro.Application.DTOs.Voting.Requests
{
    /// <summary>
    /// Contrato de entrada para el envío final de los votos del ciudadano.
    /// </summary>
    public record SubmitVoteRequest(
        Guid ElectionId,
        int CitizenId,
        string VerificationCode,
        List<SelectedVote> Selections
    );

    /// <summary>
    /// Representa la selección de un candidato para un puesto específico.
    /// </summary>
    public record SelectedVote(
        int PositionId,
        int? CandidateId, // Nulo para voto en blanco
        int? PartyId // Nulo para voto en blanco
    );
}
