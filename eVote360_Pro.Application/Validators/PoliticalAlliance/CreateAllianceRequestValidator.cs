using eVote360_Pro.Application.DTOs.PoliticalAlliance.Requests;
using FluentValidation;

namespace eVote360_Pro.Application.Validators.PoliticalAlliance
{
    public class CreateAllianceRequestValidator : AbstractValidator<CreateAllianceRequest>
    {
        public CreateAllianceRequestValidator()
        {
            RuleFor(x => x.ReceiverPartyId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un partido político receptor válido.");
        }
    }
}
