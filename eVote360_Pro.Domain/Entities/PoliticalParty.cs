using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class PoliticalParty : ActivatableBaseEntity
    {
        public required string Name { get; set; } = null!;
        public string? Description { get; set; }
        private string _acronym = null!;
        public required string Acronym
        {
            get => _acronym;
            set => _acronym = value?.ToUpperInvariant()!;
        }
        public string LogoPath { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
        public virtual ICollection<CandidatePostAssignment> CandidatePostAssignments { get; set; } =
            new List<CandidatePostAssignment>();
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<PoliticalAlliance> RequestedAlliances { get; set; } =
            new List<PoliticalAlliance>();
        public virtual ICollection<PoliticalAlliance> ReceivedAlliances { get; set; } =
            new List<PoliticalAlliance>();
        public virtual PoliticalLeaderAssignment LeaderAssignment { get; set; } = null!;
    }
}
