namespace eVote360_Pro.Application.ViewModels.ElectionViewModels
{
    public class ElectionViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime RealizationDate { get; set; }
        public string Status { get; set; } = null!;
        public int Year { get; set; }
        public bool IsActive { get; set; }
        public bool CanActivate { get; set; }
    }
}
