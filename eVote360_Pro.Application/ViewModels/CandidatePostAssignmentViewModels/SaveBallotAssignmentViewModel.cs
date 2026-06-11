using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.CandidatePostAssignmentViewModels
{
    public class SaveBallotAssignmentViewModel
    {
        [Required(ErrorMessage = "El puesto electivo es requerido.")]
        [Display(Name = "Puesto Electivo")]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "El candidato es requerido.")]
        [Display(Name = "Candidato")]
        public int CandidateId { get; set; }

        [Display(Name = "¿Es Candidato Aliado?")]
        public bool IsAlly { get; set; }
    }
}
