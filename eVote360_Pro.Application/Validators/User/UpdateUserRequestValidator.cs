using eVote360_Pro.Application.DTOs.User.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.User
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("El identificador del usuario es requerido.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre es requerido.")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido es requerido.")
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder los 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("El correo electrónico es requerido.")
                .EmailAddress()
                .WithMessage("El correo electrónico debe tener un formato válido.")
                .MaximumLength(150)
                .WithMessage("El correo electrónico no puede exceder los 150 caracteres.");

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("El nombre de usuario es requerido.")
                .MaximumLength(50)
                .WithMessage("El nombre de usuario no puede exceder los 50 caracteres.");

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage(
                    "Si se proporciona una nueva contraseña, debe tener al menos 8 caracteres."
                )
                .Matches(@"[a-zA-Z]")
                .When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("La nueva contraseña debe contener al menos una letra.")
                .Matches(@"\d")
                .When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("La nueva contraseña debe contener al menos un número.");

            RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("El rol es requerido.");
        }
    }
}
