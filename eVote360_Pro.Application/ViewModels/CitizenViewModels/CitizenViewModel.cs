namespace eVote360_Pro.Application.ViewModels.CitizenViewModels
{
    public class CitizenViewModel
    {
        public int Id { get; set; }
        public string IdentityDocument { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool HasVotedInActiveElection { get; set; }
    }
}
