using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.Models.Emails;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace eVote360_Pro.Shared.Messaging
{
    /// <summary>
    /// Implementación del servicio de correos utilizando MailKit y MimeKit.
    /// Mantiene la lectura física de plantillas Razor y el envío SMTP centralizado.
    /// </summary>
    public class MailKitEmailService : IEmailService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MailKitEmailService> _logger;
        private readonly IConfiguration _configuration;

        private static readonly SemaphoreSlim _smtpSemaphore = new(1, 1);

        public MailKitEmailService(
            IServiceScopeFactory scopeFactory,
            ILogger<MailKitEmailService> logger,
            IConfiguration configuration
        )
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync<T>(
            string to,
            string subject,
            string templateName,
            T model,
            CancellationToken cancellationToken = default
        )
            where T : IEmailModel
        {
            bool acquired = await _smtpSemaphore.WaitAsync(
                TimeSpan.FromSeconds(30),
                cancellationToken
            );

            if (!acquired)
            {
                _logger.LogError(
                    "No se pudo conseguir un turno en el semáforo SMTP. Tiempo de espera agotado para: {Email}",
                    to
                );
                return false;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var renderer = scope.ServiceProvider.GetRequiredService<IRazorRenderer>();

                string sanitizedTemplateName = Path.GetFileNameWithoutExtension(templateName);
                string templatePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "Templates",
                    "Emails",
                    $"{sanitizedTemplateName}.cshtml"
                );

                if (!File.Exists(templatePath))
                {
                    _logger.LogError("Plantilla de correo no encontrada: {Path}", templatePath);
                    return false;
                }

                string htmlBody = await renderer.RenderTemplateAsync(templatePath, model);

                string host = _configuration.GetValue<string>("SmtpSettings:Host") ?? "";
                int port = _configuration.GetValue<int>("SmtpSettings:Port");
                string senderEmail =
                    _configuration.GetValue<string>("SmtpSettings:SenderEmail") ?? "";
                string senderName =
                    _configuration.GetValue<string>("SmtpSettings:SenderName") ?? "";
                string username = _configuration.GetValue<string>("SmtpSettings:Username") ?? "";
                string password = _configuration.GetValue<string>("SmtpSettings:Password") ?? "";
                bool enableSsl = _configuration.GetValue<bool>("SmtpSettings:EnableSsl");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                var secureSocketOptions = enableSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.Auto;

                await client.ConnectAsync(host, port, secureSocketOptions, cancellationToken);

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    await client.AuthenticateAsync(username, password, cancellationToken);
                }

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation(
                    "Correo enviado exitosamente a {Email}. Asunto: {Subject}",
                    to,
                    subject
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    ex,
                    "Excepción no controlada al intentar enviar correo a {Email}",
                    to
                );
                return false;
            }
            finally
            {
                _smtpSemaphore.Release();
            }
        }
    }
}
