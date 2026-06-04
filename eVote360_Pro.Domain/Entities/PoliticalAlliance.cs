using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Enums;

namespace eVote360_Pro.Domain.Entities
{
    public class PoliticalAlliance : BaseEntity
    {
        public int RequesterPartyId { get; set; }
        public int ReceiverPartyId { get; set; }
        public AllianceStatus Status { get; set; } = AllianceStatus.Pending;

        // Navigation properties
        public PoliticalParty RequesterParty { get; set; } = null!;
        public PoliticalParty ReceiverParty { get; set; } = null!;
    }
}
