namespace eVote360_Pro.Application.ViewModels.DashboardViewModels
{
    public class DashboardViewModel
    {
        public int TotalElections { get; set; }
        public string? ActiveElectionName { get; set; }
        public int VoterParticipationCount { get; set; }
        public int TotalCitizens { get; set; }
        public int TotalParties { get; set; }
    }
}
