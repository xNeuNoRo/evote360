using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.CandidateViewModels
{
    public class UpdateCandidateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del candidato es requerido.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "El apellido del candidato es requerido.")]
        [MaxLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Nueva Foto (opcional)")]
        public IFormFile? PhotoFile { get; set; }

        [Display(Name = "Estado Activo")]
        public bool IsActive { get; set; }

        public string? CurrentPhotoPath { get; set; }
        
        public bool HasParticipated { get; set; }
    }
}
