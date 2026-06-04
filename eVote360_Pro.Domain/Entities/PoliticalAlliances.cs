using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Enums;

namespace eVote360_Pro.Domain.Entities
{
    public class PoliticalAlliances : BaseEntity
    {
        public int RequesterPartyId { get; set; }
        public int ReceiverPartyId { get; set; }
        public AllianceStatus Status { get; set; } = AllianceStatus.Pending;

        // Navigation properties
        public PoliticalParties RequesterParty { get; set; } = null!;
        public PoliticalParties ReceiverParty { get; set; } = null!;
    }
}
