using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Votes : BaseEntity
    {
        public required int ElectionId { get; set; }    
        public required int PositionId { get; set; }
        public int? CandidateId { get; set; } // Nullable to allow for blank votes
        public int? PartyId { get; set; } // Nullable to allow for blank votes
        
        


        
    }
}
