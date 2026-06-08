using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.VotingViewModels
{
    public class VoteSubmissionViewModel
    {
        [Required]
        public Guid ElectionId { get; set; }

        [Required]
        public int CitizenId { get; set; }

        [Required]
        public string VerificationCode { get; set; } = null!;

        public List<SelectedVoteViewModel> Selections { get; set; } = new();
    }

    public class SelectedVoteViewModel
    {
        [Required]
        public int PositionId { get; set; }

        public int? CandidateId { get; set; }
        public int? PartyId { get; set; }
    }
}
