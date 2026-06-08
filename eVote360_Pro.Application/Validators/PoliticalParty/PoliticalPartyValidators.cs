using eVote360_Pro.Application.DTOs.PoliticalParty.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.PoliticalParty
{
    public class CreatePoliticalPartyRequestValidator
        : AbstractValidator<CreatePoliticalPartyRequest>
    {
        public CreatePoliticalPartyRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del partido es requerido.")
                .MaximumLength(150)
                .WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.Acronym)
                .NotEmpty()
                .WithMessage("Las siglas son requeridas.")
                .MaximumLength(20)
                .WithMessage("Las siglas no pueden exceder los 20 caracteres.");

            RuleFor(x => x.LogoFile).NotNull().WithMessage("El logo del partido es requerido.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder los 500 caracteres.");
        }
    }

    public class UpdatePoliticalPartyRequestValidator
        : AbstractValidator<UpdatePoliticalPartyRequest>
    {
        public UpdatePoliticalPartyRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El identificador del partido es requerido.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del partido es requerido.")
                .MaximumLength(150)
                .WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.Acronym)
                .NotEmpty()
                .WithMessage("Las siglas son requeridas.")
                .MaximumLength(20)
                .WithMessage("Las siglas no pueden exceder los 20 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder los 500 caracteres.");
        }
    }
}
