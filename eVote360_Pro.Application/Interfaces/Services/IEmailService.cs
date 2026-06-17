using eVote360_Pro.Application.Models.Emails;

namespace eVote360_Pro.Application.Interfaces.Services
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
