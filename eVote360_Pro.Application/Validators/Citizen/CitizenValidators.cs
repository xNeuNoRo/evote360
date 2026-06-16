using eVote360_Pro.Application.DTOs.Citizen.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.Citizen
{
    public class CreateCitizenRequestValidator : AbstractValidator<CreateCitizenRequest>
    {
        public CreateCitizenRequestValidator()
        {
            RuleFor(x => x.IdentityDocument)
                .NotEmpty()
                .WithMessage("El documento de identidad es requerido.");

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
        }
    }

    public class UpdateCitizenRequestValidator : AbstractValidator<UpdateCitizenRequest>
    {
        public UpdateCitizenRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El identificador del ciudadano es requerido.");

            RuleFor(x => x.IdentityDocument)
                .NotEmpty()
                .WithMessage("El documento de identidad es requerido.");

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
        }
    }
}
