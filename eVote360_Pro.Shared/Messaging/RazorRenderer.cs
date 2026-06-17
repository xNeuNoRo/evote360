using RazorLight;

namespace eVote360_Pro.Shared.Messaging
{
    /// <summary>
    /// Implementación del renderizador utilizando RazorLight.
    /// Lee las plantillas físicas (.cshtml) y las combina con el modelo fuertemente tipado.
    /// </summary>
    public class RazorRenderer : IRazorRenderer
    {
        private readonly IRazorLightEngine _engine;

        public RazorRenderer()
        {
            _engine = new RazorLightEngineBuilder().UseMemoryCachingProvider().Build();
        }

        public async Task<string> RenderTemplateAsync<T>(string templatePath, T model)
        {
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    $"La plantilla de correo no se encontró en: {templatePath}"
                );
            }

            string templateContent = await File.ReadAllTextAsync(templatePath);
            return await _engine.CompileRenderStringAsync(templatePath, templateContent, model);
        }
    }
}
