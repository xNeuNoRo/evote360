namespace eVote360_Pro.Application.ViewModels.PoliticalAllianceViewModels
{
    public class AllianceViewModel
    {
        public int Id { get; set; }
        public string RequesterPartyName { get; set; } = null!;
        public string ReceiverPartyName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}
