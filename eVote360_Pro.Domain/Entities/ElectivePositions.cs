using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class ElectivePositions : ActivatableBaseEntity
    {
        public required string Name { get; set; } = null!;
        public required string Description { get; set; } = null!;
    }
}
