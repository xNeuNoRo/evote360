using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class CandidatePostAssignment : BaseEntity
    {
        public int CandidateId { get; set; }
        public int PositionId { get; set; }
        public int PartyId { get; set; }
        public bool IsAlly { get; set; } = false;

        // Navigation properties
        public Candidate Candidate { get; set; } = null!;
        public ElectivePosition Position { get; set; } = null!;
        public PoliticalParty Party { get; set; } = null!;
    }
}
