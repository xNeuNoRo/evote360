using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.ElectionViewModels
{
    public class UpdateElectionViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre de la elección es requerido.")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres.")]
        [Display(Name = "Nombre de la Elección")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de realización es requerida.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Realización")]
        public DateTime RealizationDate { get; set; }

        [Display(Name = "Estado Activo")]
        public bool IsActive { get; set; }
    }
}
