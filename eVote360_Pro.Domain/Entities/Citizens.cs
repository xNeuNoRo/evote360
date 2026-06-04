using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Entities
{
    public class Citizens : ActivatableBaseEntity
    {
        public required string IdentityDocument { get; set; } 
        public required string FirstName { get; set; } 
        public required string LastName { get; set; } 

        public required string Email { get; set; }

        public ICollection<VerificationCodes> VerificationCodes { get; set; } = new List<VerificationCodes>();





    }
}

