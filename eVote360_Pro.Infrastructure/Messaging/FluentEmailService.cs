using eVote360_Pro.Shared.Interfaces.Messaging;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eVote360_Pro.Infrastructure.Messaging
{
    /// <summary>
    /// Implementación del servicio de correos utilizando FluentEmail y Razor Templates.
    /// Gestiona la concurrencia SMTP y asegura la entrega de correos.
    /// </summary>
    public class FluentEmailService : IEmailService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FluentEmailService> _logger;

        // Patron semaforo para controlar la concurrencia hacia el servidor SMTP
        private static readonly SemaphoreSlim _smtpSemaphore = new SemaphoreSlim(1, 1);

        public FluentEmailService(
            IServiceScopeFactory scopeFactory,
            ILogger<FluentEmailService> logger
        )
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
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
                // Creamos un scope para resolver la factory de correos
                using var scope = _scopeFactory.CreateScope();
                var emailFactory = scope.ServiceProvider.GetRequiredService<IFluentEmailFactory>();

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

                // Preparamos el correo
                var email = emailFactory
                    .Create()
                    .To(to)
                    .Subject(subject)
                    .UsingTemplateFromFile(templatePath, model);

                // Enviamos el correo y esperamos la respuesta
                var response = await email.SendAsync(cancellationToken);

                if (!response.Successful)
                {
                    _logger.LogError(
                        "Error al enviar correo a {Email}. Detalles: {Errors}",
                        to,
                        string.Join(", ", response.ErrorMessages)
                    );
                    return false;
                }

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
