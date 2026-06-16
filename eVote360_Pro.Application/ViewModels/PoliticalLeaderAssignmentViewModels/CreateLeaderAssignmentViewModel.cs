using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.PoliticalLeaderAssignmentViewModels
{
    public class CreateLeaderAssignmentViewModel
    {
        [Required(ErrorMessage = "El usuario es requerido.")]
        [Display(Name = "Usuario Dirigente")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "El partido político es requerido.")]
        [Display(Name = "Partido Político")]
        public int PartyId { get; set; }
    }
}
