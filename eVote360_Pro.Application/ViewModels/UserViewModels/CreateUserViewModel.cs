using System.ComponentModel.DataAnnotations;

namespace eVote360_Pro.Application.ViewModels.UserViewModels
{
    public class CreateUserViewModel
    {
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

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "La contraseña debe contener al menos una letra y un número.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "El rol es requerido.")]
        [Display(Name = "Rol de Usuario")]
        public int RoleId { get; set; }
    }
}
