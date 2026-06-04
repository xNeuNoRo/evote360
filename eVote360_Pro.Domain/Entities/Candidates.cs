using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Candidates : ActivatableBaseEntity
    {
        public required string FirstName { get; set; } = null!;
        public required string LastName { get; set; } = null!;
        public required string PhotoPath { get; set; } = null!;
        public required int OriginalPartyId { get; set; }

        // Navigation property
        public PoliticalParties OriginalParty { get; set; } = null!;
    }
}
