using eVote360_Pro.Application.DTOs.Auth.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("El nombre de usuario es requerido.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña es requerida.");
        }
    }
}
