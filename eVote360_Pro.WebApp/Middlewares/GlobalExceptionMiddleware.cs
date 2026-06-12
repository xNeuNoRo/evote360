using System.Net;
using System.Text.Json;
using eVote360_Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace eVote360_Pro.WebApp.Middlewares
{
    /// <summary>
    /// Middleware global para interceptar excepciones de dominio y negocio,
    /// traduciéndolas a respuestas amigables para el cliente.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITempDataDictionaryFactory tempDataFactory
        )
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error en el flujo de la aplicación.");
                await HandleExceptionAsync(context, ex, tempDataFactory);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ITempDataDictionaryFactory tempDataFactory
        )
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message =
                "Ha ocurrido un error inesperado. Por favor, contacte al soporte técnico.";
            string errorCode = "System.InternalError";

            // Evaluamos si es una de nuestras excepciones de negocio controladas
            if (exception is DomainException domainEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = domainEx.Message;
                errorCode = domainEx.Code;
            }
            else if (exception is BusinessException businessEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = businessEx.Message;
                errorCode = businessEx.Code;
            }
            else if (exception is ValidationBusinessException validationEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = validationEx.Message;
                errorCode = validationEx.Code;
            }

            bool isAjax =
                context.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                || context.Request.Headers["Accept"].ToString().Contains("application/json");

            if (isAjax)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var response = new { Message = message, Code = errorCode };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else
            {
                var tempData = tempDataFactory.GetTempData(context);
                tempData["ErrorMessage"] = message;
                tempData.Save();

                var referer = context.Request.Headers["Referer"].ToString();

                if (!string.IsNullOrEmpty(referer) && !referer.Contains("/Home/Error"))
                {
                    context.Response.Redirect(referer);
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }
            }
        }
    }

    // Método de extensión para facilitar el registro en Program.cs
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
