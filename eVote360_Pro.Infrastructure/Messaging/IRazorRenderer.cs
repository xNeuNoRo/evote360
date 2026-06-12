using System.Threading.Tasks;

namespace eVote360_Pro.Infrastructure.Messaging
{
    /// <summary>
    /// Interfaz interna para renderizar plantillas Razor a cadenas HTML puro.
    /// Esto sustituye la funcionalidad de renderizado que proveía FluentEmail.Razor.
    /// </summary>
    internal interface IRazorRenderer
    {
        Task<string> RenderTemplateAsync<T>(string templatePath, T model);
    }
}
