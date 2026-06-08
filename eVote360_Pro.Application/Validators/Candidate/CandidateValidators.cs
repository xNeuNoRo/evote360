using eVote360_Pro.Application.DTOs.Candidate.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.Candidate
{
    public class CreateCandidateRequestValidator : AbstractValidator<CreateCandidateRequest>
    {
        public CreateCandidateRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre del candidato es requerido.")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido del candidato es requerido.")
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder los 100 caracteres.");

            RuleFor(x => x.PhotoFile).NotNull().WithMessage("La foto del candidato es requerida.");
        }
    }

    public class UpdateCandidateRequestValidator : AbstractValidator<UpdateCandidateRequest>
    {
        public UpdateCandidateRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El identificador del candidato es requerido.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre del candidato es requerido.")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido del candidato es requerido.")
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder los 100 caracteres.");
        }
    }
}
