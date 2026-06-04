using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Candidate : ActivatableBaseEntity
    {
        public required string FirstName { get; set; } = null!;
        public required string LastName { get; set; } = null!;
        public required string PhotoPath { get; set; } = null!;
        public required int OriginalPartyId { get; set; }

        // Navigation property
        public virtual PoliticalParty OriginalParty { get; set; } = null!;
        public virtual ICollection<CandidatePostAssignment> PostAssignments { get; set; } =
            new List<CandidatePostAssignment>();
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
