using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.ViewModels.PoliticalPartyViewModels
{
    public class UpdatePoliticalPartyViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del partido es requerido.")]
        [MaxLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        [Display(Name = "Nombre del Partido")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Las siglas son requeridas.")]
        [MaxLength(20, ErrorMessage = "Las siglas no pueden exceder los 20 caracteres.")]
        [Display(Name = "Siglas")]
        public string Acronym { get; set; } = null!;

        [Display(Name = "Descripción")]
        [MaxLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Description { get; set; }

        [Display(Name = "Nuevo Logo (opcional)")]
        public IFormFile? LogoFile { get; set; }

        public string? CurrentLogoUrl { get; set; }

        [Display(Name = "Estado Activo")]
        public bool IsActive { get; set; }
    }
}
