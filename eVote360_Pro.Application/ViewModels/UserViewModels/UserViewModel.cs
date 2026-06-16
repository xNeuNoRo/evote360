namespace eVote360_Pro.Application.ViewModels.UserViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? AssignedPartyName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
