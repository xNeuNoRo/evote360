using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Citizen : ActivatableBaseEntity
    {
        public required string IdentityDocument { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }

        // Navigation properties
        public virtual ICollection<VerificationCode> VerificationCodes { get; set; } =
            new List<VerificationCode>();
        public virtual ICollection<VoterParticipation> Participations { get; set; } =
            new List<VoterParticipation>();
    }
}
