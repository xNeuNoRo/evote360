using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class PoliticalParties : ActivatableBaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        private string _acronym = null!;
        public string Acronym
        {
            get => _acronym;
            set => _acronym = value?.ToUpperInvariant()!;
        }
        public string LogoPath { get; set; } = null!;
    }
}
