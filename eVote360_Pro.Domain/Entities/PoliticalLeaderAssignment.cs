using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class PoliticalLeaderAssignment : ActivatableBaseEntity
    {
        public int UserId { get; set; }
        public int PartyId { get; set; }

        // Navigation property
        public virtual PoliticalParty Party { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
