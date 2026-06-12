using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using eVote360_Pro.Shared.Interfaces.Messaging;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace eVote360_Pro.Infrastructure.Messaging
{
    /// <summary>
    /// Implementación del servicio de correos utilizando MailKit y MimeKit.
    /// Mantiene la lógica del semáforo para concurrencia y la lectura física de plantillas Razor.
    /// </summary>
    public class MailKitEmailService : IEmailService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MailKitEmailService> _logger;
        private readonly IConfiguration _configuration;

        // Patron semaforo para controlar la concurrencia hacia el servidor SMTP
        private static readonly SemaphoreSlim _smtpSemaphore = new SemaphoreSlim(1, 1);

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
            // Esperamos el turno en la cola SMTP con un timeout de 30 segundos
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
                // Creamos un scope para resolver el IRazorRenderer
                using var scope = _scopeFactory.CreateScope();
                var renderer = scope.ServiceProvider.GetRequiredService<IRazorRenderer>();

                // Sanitizamos el nombre de la plantilla para evitar problemas de path traversal
                string sanitizedTemplateName = Path.GetFileNameWithoutExtension(templateName);

                // Construimos la ruta de la plantilla
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

                // Generamos el HTML usando nuestro motor de renderizado
                string htmlBody = await renderer.RenderTemplateAsync(templatePath, model);

                // Leemos las credenciales desde el IConfiguration
                string host = _configuration.GetValue<string>("SmtpSettings:Host") ?? "";
                int port = _configuration.GetValue<int>("SmtpSettings:Port");
                string senderEmail =
                    _configuration.GetValue<string>("SmtpSettings:SenderEmail") ?? "";
                string senderName =
                    _configuration.GetValue<string>("SmtpSettings:SenderName") ?? "";
                string username = _configuration.GetValue<string>("SmtpSettings:Username") ?? "";
                string password = _configuration.GetValue<string>("SmtpSettings:Password") ?? "";
                bool enableSsl = _configuration.GetValue<bool>("SmtpSettings:EnableSsl");

                // Preparamos el correo con MimeKit
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                // Enviamos el correo con MailKit
                using var client = new SmtpClient();

                var secureSocketOptions = enableSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.Auto;

                // Intentamos conectar y enviar el correo
                await client.ConnectAsync(host, port, secureSocketOptions, cancellationToken);

                // Solo autenticamos si se proporcionaron credenciales
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
