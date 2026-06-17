namespace eVote360_Pro.Shared.Messaging
{
    /// <summary>
    /// Renderiza plantillas Razor a HTML puro para los correos del sistema.
    /// </summary>
    public interface IRazorRenderer
    {
        Task<string> RenderTemplateAsync<T>(string templatePath, T model);
    }
}
