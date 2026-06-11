namespace eVote360_Pro.Application.ViewModels.ElectivePositionViewModels
{
    public class ElectivePositionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
