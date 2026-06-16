using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.PoliticalLeaderAssignment
{
    public class SaveLeaderAssignmentRequestValidator
        : AbstractValidator<SaveLeaderAssignmentRequest>
    {
        public SaveLeaderAssignmentRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("El identificador del usuario es requerido.");

            RuleFor(x => x.PartyId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un partido político válido.");
        }
    }
}
