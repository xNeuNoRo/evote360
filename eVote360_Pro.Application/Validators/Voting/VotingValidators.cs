using eVote360_Pro.Application.DTOs.Voting.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.Voting
{
    public class ValidateElectorRequestValidator : AbstractValidator<ValidateElectorRequest>
    {
        public ValidateElectorRequestValidator()
        {
            RuleFor(x => x.IdentityDocument)
                .NotEmpty()
                .WithMessage("El número de documento de identidad es requerido.");

            RuleFor(x => x.IdCardImage)
                .NotNull()
                .WithMessage("La foto de la cédula es requerida para la validación OCR.");
        }
    }

    public class VerifyCodeRequestValidator : AbstractValidator<VerifyCodeRequest>
    {
        public VerifyCodeRequestValidator()
        {
            RuleFor(x => x.CitizenId)
                .GreaterThan(0)
                .WithMessage("El identificador del ciudadano es requerido.");

            RuleFor(x => x.ElectionId)
                .NotEmpty()
                .WithMessage("El identificador de la elección es requerido.");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("El código de verificación es requerido.")
                .MaximumLength(10)
                .WithMessage("El código no puede exceder los 10 caracteres.");
        }
    }

    public class SubmitVoteRequestValidator : AbstractValidator<SubmitVoteRequest>
    {
        public SubmitVoteRequestValidator()
        {
            RuleFor(x => x.ElectionId)
                .NotEmpty()
                .WithMessage("El identificador de la elección es requerido.");

            RuleFor(x => x.CitizenId)
                .GreaterThan(0)
                .WithMessage("El identificador del ciudadano es requerido.");

            RuleFor(x => x.VerificationCode)
                .NotEmpty()
                .WithMessage("El código de verificación es requerido.");

            RuleFor(x => x.Selections)
                .NotEmpty()
                .WithMessage("Debe seleccionar al menos una opción para votar.");

            RuleForEach(x => x.Selections).SetValidator(new SelectedVoteValidator());
        }
    }

    public class SelectedVoteValidator : AbstractValidator<SelectedVote>
    {
        public SelectedVoteValidator()
        {
            RuleFor(x => x.PositionId)
                .GreaterThan(0)
                .WithMessage("El puesto electivo seleccionado no es válido.");
        }
    }
}
