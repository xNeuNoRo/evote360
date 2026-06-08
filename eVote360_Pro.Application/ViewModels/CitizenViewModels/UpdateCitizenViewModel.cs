using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.CitizenViewModels
{
    public class UpdateCitizenViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El documento de identidad es requerido.")]
        [Display(Name = "Cédula")]
        public string IdentityDocument { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es requerido.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es requerido.")]
        [MaxLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
        [MaxLength(150, ErrorMessage = "El correo electrónico no puede exceder los 150 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = null!;

        [Display(Name = "Estado Activo")]
        public bool IsActive { get; set; }
    }
}
