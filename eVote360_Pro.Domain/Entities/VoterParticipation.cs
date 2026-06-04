using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class VoterParticipation : BaseEntity
    {
        public required int CitizenId { get; set; }
        public required int ElectionId { get; set; }

        // Navigation properties
        public virtual Citizen Citizen { get; set; } = null!;
        public virtual Election Election { get; set; } = null!;
    }
}
