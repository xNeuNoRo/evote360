using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace eVote360_Pro.Application.Common.Internal
{
    /// <summary>
    /// Utilidad para traducir códigos de error técnicos a mensajes amigables en español.
    /// Lee las traducciones desde el archivo ErrorMessages.json.
    /// </summary>
    public class ExceptionTranslator
    {
        private readonly IDictionary<string, string> _messages;
        private readonly ILogger<ExceptionTranslator> _logger;

        public ExceptionTranslator(ILogger<ExceptionTranslator> logger)
        {
            _logger = logger;
            _messages = LoadMessages();
        }

        public string Translate(string code)
        {
            if (_messages.TryGetValue(code, out var message))
            {
                return message;
            }

            _logger.LogWarning("Código de error no encontrado en el diccionario: {Code}", code);
            return _messages["General.Unexpected"];
        }

        private static IDictionary<string, string> LoadMessages()
        {
            try
            {
                string path = Path.Combine(
                    AppContext.BaseDirectory,
                    "Resources",
                    "ErrorMessages.json"
                );

                // Si no existe el archivo, usamos un diccionario vacío
                if (!File.Exists(path))
                    return new Dictionary<string, string>
                    {
                        { "General.Unexpected", "Error inesperado." },
                    };

                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                    ?? new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>
                {
                    { "General.Unexpected", "Error inesperado." },
                };
            }
        }
    }
}
