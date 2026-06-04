using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class CandidatePostAssignments : BaseEntity
    {
        public int CandidateId { get; set; }
        public int PositionId { get; set; }
        public int PartyId { get; set; }
        public bool IsAlly { get; set; } = false;

        // Navigation properties
        public Candidates Candidate { get; set; } = null!;
        public ElectivePositions Position { get; set; } = null!;
        public PoliticalParties Party { get; set; } = null!;
    }
}
