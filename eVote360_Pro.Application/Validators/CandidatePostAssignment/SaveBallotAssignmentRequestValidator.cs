using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.CandidatePostAssignment
{
    public class SaveBallotAssignmentRequestValidator
        : AbstractValidator<SaveBallotAssignmentRequest>
    {
        public SaveBallotAssignmentRequestValidator()
        {
            RuleFor(x => x.PositionId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un puesto electivo válido.");

            RuleFor(x => x.CandidateId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un candidato válido.");
        }
    }
}
