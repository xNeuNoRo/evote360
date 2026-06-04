using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Vote : BaseEntity
    {
        public required int ElectionId { get; set; }
        public required int PositionId { get; set; }
        public int? CandidateId { get; set; }
        public int? PartyId { get; set; }

        // Navigation Properties
        public virtual Election Election { get; set; } = null!;
        public virtual ElectivePosition Position { get; set; } = null!;
        public virtual Candidate? Candidate { get; set; }
        public virtual PoliticalParty? Party { get; set; }
    }
}
