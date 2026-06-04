using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Enums;

namespace eVote360_Pro.Domain.Entities
{
    public class Election : ActivatableBaseEntity
    {
        public required string Name { get; set; }
        public required DateTime RealizationDate { get; set; }
        public ElectionStatus Status { get; set; } = ElectionStatus.Pending;

        // Navigation Properties
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<VoterParticipation> Participations { get; set; } =
            new List<VoterParticipation>();
        public virtual ICollection<VerificationCode> VerificationCodes { get; set; } =
            new List<VerificationCode>();
    }
}
