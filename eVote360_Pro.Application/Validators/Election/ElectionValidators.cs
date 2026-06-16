using eVote360_Pro.Application.DTOs.Election.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.Election
{
    public class CreateElectionRequestValidator : AbstractValidator<CreateElectionRequest>
    {
        public CreateElectionRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre de la elección es requerido.")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder los 200 caracteres.");

            RuleFor(x => x.RealizationDate)
                .NotEmpty()
                .WithMessage("La fecha de realización es requerida.");
        }
    }

    public class UpdateElectionRequestValidator : AbstractValidator<UpdateElectionRequest>
    {
        public UpdateElectionRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("El identificador de la elección es requerido.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre de la elección es requerido.")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder los 200 caracteres.");

            RuleFor(x => x.RealizationDate)
                .NotEmpty()
                .WithMessage("La fecha de realización es requerida.");
        }
    }
}
