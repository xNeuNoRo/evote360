namespace eVote360_Pro.Application.DTOs.Dashboard.Responses
{
    public record ElectionSummaryResponse(
        Guid Id,
        string Name,
        DateTime RealizationDate,
        int ParticipatingPartiesCount,
        int RealCandidatesCount,
        int VoterParticipationCount
    );
}
