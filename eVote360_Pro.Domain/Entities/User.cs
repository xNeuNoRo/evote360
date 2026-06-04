using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class User : ActivatableBaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required int RoleId { get; set; }

        // Navigation properties
        public virtual Role? Role { get; set; }
        public virtual PoliticalLeaderAssignment? LeaderAssignment { get; set; }
    }
}
