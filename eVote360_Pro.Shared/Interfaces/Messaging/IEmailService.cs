namespace eVote360_Pro.Shared.Interfaces.Messaging
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync<T>(
            string to,
            string subject,
            string templateName,
            T model,
            CancellationToken cancellationToken = default
        ) where T : IEmailModel;
    }
}
