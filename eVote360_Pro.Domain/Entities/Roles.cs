using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Roles : BaseEntity
    {
        public required string Name { get; set; }

        // Navigation Property
        public ICollection<Users> Users { get; set; } = new List<Users>();
    }
    
}

