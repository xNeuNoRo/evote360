using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.PoliticalAllianceViewModels
{
    public class CreateAllianceViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un partido político para la alianza.")]
        [Display(Name = "Partido Receptor")]
        public int ReceiverPartyId { get; set; }
    }
}
