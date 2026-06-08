using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.VotingViewModels
{
    public class IdentityValidationViewModel
    {
        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        [Display(Name = "Número de Cédula")]
        public string IdentityDocument { get; set; } = null!;

        [Required(ErrorMessage = "La foto de la cédula es requerida para validar su identidad.")]
        [Display(Name = "Foto de la Cédula")]
        public IFormFile IdCardImage { get; set; } = null!;
    }
}
