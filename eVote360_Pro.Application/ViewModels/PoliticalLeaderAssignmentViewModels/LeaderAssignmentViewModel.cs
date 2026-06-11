namespace eVote360_Pro.Application.ViewModels.PoliticalLeaderAssignmentViewModels
{
    public class LeaderAssignmentViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public string PartyName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
