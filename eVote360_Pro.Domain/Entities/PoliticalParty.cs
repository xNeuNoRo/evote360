using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class PoliticalParty : ActivatableBaseEntity
    {
        public required string Name { get; set; } = null!;
        public string? Description { get; set; }
        private  string _acronym = null!;
        public required string Acronym
        {
            get => _acronym;
            set => _acronym = value?.ToUpperInvariant()!;
        }
        public string LogoPath { get; set; } = null!;
    }
}
