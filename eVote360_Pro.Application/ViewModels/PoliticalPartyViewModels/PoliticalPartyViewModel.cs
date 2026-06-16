namespace eVote360_Pro.Application.ViewModels.PoliticalPartyViewModels
{
    public class PoliticalPartyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Acronym { get; set; } = null!;
        public string? Description { get; set; }
        public string LogoUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool HasLeaderAssigned { get; set; }
    }
}
