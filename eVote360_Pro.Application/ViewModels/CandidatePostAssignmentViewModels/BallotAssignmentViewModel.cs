namespace eVote360_Pro.Application.ViewModels.CandidatePostAssignmentViewModels
{
    public class BallotAssignmentViewModel
    {
        public int Id { get; set; }
        public string PositionName { get; set; } = null!;
        public string CandidateName { get; set; } = null!;
        public string CandidatePhotoUrl { get; set; } = null!;
        public bool IsAlly { get; set; }
    }
}
