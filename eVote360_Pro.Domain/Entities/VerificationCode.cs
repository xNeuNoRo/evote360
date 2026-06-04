using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class VerificationCode : BaseEntity
    {
        public required int CitizenId { get; set; }
        public Citizen? Citizen { get; set; }
        public required int ElectionId { get; set; }
        public required string Code { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public required bool IsUsed { get; set; } = false;

        // Navigation Properties
        public virtual Election Election { get; set; } = null!;
    }
}
