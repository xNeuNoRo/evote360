using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class ElectivePositions : ActivatableBaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
