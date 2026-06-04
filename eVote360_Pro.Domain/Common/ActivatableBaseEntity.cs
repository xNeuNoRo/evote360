namespace eVote360_Pro.Domain.Common
{
    public abstract class ActivatableBaseEntity : BaseEntity
    {
        public bool IsActive { get; set; } = true;
    }
}
