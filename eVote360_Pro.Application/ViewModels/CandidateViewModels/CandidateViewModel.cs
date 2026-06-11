namespace eVote360_Pro.Application.ViewModels.CandidateViewModels
{
    public class CandidateViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhotoUrl { get; set; } = null!;
        public string OriginalPartyName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
