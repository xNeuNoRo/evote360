using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.VotingViewModels
{
    public class VerifyCodeViewModel
    {
        [Required]
        public int CitizenId { get; set; }

        [Required]
        public Guid ElectionId { get; set; }

        [Required(ErrorMessage = "El código de verificación es requerido.")]
        [MaxLength(10, ErrorMessage = "El código no es válido.")]
        [Display(Name = "Código OTP")]
        public string Code { get; set; } = null!;
    }
}
