using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class PoliticalLeaderAssignments : ActivatableBaseEntity
    {
        public int UserId { get => Id; set => Id = value; }
        public int PartyId { get; set; }

        // Navigation property
        public PoliticalParties Party { get; set; } = null!;
    }
}
