using eVote360_Pro.Shared.Interfaces.Messaging;

namespace eVote360_Pro.Application.Models.Emails
{
    public record OtpVerificationModel(string FullName, string Code) : IEmailModel;

    public record UserWelcomeModel(
        string FullName,
        string Username,
        string TemporaryPassword,
        string LoginUrl
    ) : IEmailModel;

    public record AllianceRequestModel(
        string RequesterPartyName,
        string ReceiverLeaderName,
        string ActionUrl
    ) : IEmailModel;

    public record AllianceResponseModel(string ReceiverPartyName, string Status, string Message)
        : IEmailModel;

    public record VoteConfirmationModel(
        string CitizenName,
        string ElectionName,
        string ParticipationCode,
        string Date
    ) : IEmailModel;
}
