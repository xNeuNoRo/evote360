using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class ElectivePosition : ActivatableBaseEntity
    {
        public required string Name { get; set; } = null!;
        public required string Description { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<CandidatePostAssignment> CandidatePostAssignments { get; set; } =
            new List<CandidatePostAssignment>();
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
