using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.ElectivePositionViewModels
{
    public class CreateElectivePositionViewModel
    {
        [Required(ErrorMessage = "El nombre del puesto es requerido.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre del Puesto")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La descripción del puesto es requerida.")]
        [MaxLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;
    }
}
