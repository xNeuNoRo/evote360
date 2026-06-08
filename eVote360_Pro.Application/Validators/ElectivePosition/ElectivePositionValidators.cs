using eVote360_Pro.Application.DTOs.ElectivePosition.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.ElectivePosition
{
    public class CreateElectivePositionRequestValidator
        : AbstractValidator<CreateElectivePositionRequest>
    {
        public CreateElectivePositionRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del puesto es requerido.")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("La descripción del puesto es requerida.")
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder los 500 caracteres.");
        }
    }

    public class UpdateElectivePositionRequestValidator
        : AbstractValidator<UpdateElectivePositionRequest>
    {
        public UpdateElectivePositionRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El identificador del puesto es requerido.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del puesto es requerido.")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("La descripción del puesto es requerida.")
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder los 500 caracteres.");
        }
    }
}
