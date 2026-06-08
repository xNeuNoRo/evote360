using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.CandidateViewModels
{
    public class CreateCandidateViewModel
    {
        [Required(ErrorMessage = "El nombre del candidato es requerido.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "El apellido del candidato es requerido.")]
        [MaxLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "La foto del candidato es requerida.")]
        [Display(Name = "Foto del Candidato")]
        public IFormFile PhotoFile { get; set; } = null!;

        [Required(ErrorMessage = "El partido de origen es requerido.")]
        [Display(Name = "Partido de Origen")]
        public int OriginalPartyId { get; set; }
    }
}
