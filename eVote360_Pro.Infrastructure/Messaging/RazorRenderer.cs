using System.IO;
using System.Threading.Tasks;
using RazorLight;

namespace eVote360_Pro.Infrastructure.Messaging
{
    /// <summary>
    /// Implementación del renderizador utilizando RazorLight.
    /// Lee las plantillas físicas (.cshtml) y las combina con el modelo fuertemente tipado.
    /// </summary>
    internal class RazorRenderer : IRazorRenderer
    {
        private readonly IRazorLightEngine _engine;

        public RazorRenderer()
        {
            // Construimos el motor configurado para cachear en memoria
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

            // Leemos el contenido físico del archivo .cshtml
            string templateContent = await File.ReadAllTextAsync(templatePath);

            // Renderizamos la plantilla a HTML usando el path como key para la caché interna
            return await _engine.CompileRenderStringAsync(templatePath, templateContent, model);
        }
    }
}
