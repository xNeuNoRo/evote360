namespace eVote360_Pro.Application.ViewModels.ResultViewModels
{
    public class ElectionResultViewModel
    {
        public Guid ElectionId { get; set; }
        public string ElectionName { get; set; } = null!;
        public int TotalVoters { get; set; }
        public List<PositionResultViewModel> ResultsByPosition { get; set; } = new();
    }

    public class PositionResultViewModel
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; } = null!;
        public bool IsTie { get; set; }
        public List<CandidateResultViewModel> Candidates { get; set; } = new();
    }

    public class CandidateResultViewModel
    {
        public int? CandidateId { get; set; }
        public string CandidateName { get; set; } = null!;
        public string PartyName { get; set; } = null!;
        public string PhotoUrl { get; set; } = null!;
        public int VotesCount { get; set; }
        public double Percentage { get; set; }
    }
}
