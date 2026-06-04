using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Users: ActivatableBaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        public required int RoleId { get; set; }
        public Roles? Roles { get; set; }

    


    }
}