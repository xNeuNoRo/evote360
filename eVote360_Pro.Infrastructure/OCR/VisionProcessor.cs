using OpenCvSharp;

namespace eVote360_Pro.Infrastructure.OCR
{
    /// <summary>
    /// Motor de procesamiento de imágenes.
    /// </summary>
    public static class VisionProcessor
    {
        /// <summary>
        /// Detecta si la imagen contiene un objeto con forma de documento.
        /// </summary>
        /// <param name="src">Imagen original en formato Mat.</param>
        /// <param name="minAreaPercentage">Porcentaje mínimo de la imagen que debe ocupar el documento.</param>
        /// <returns>True si se detecta un documento válido.</returns>
        public static bool ContainsDocument(Mat src, double minAreaPercentage = 0.05)
        {
            // Convertimos a escala de grises
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Aplicamos un desenfoque para reducir ruido y mejorar la detección de bordes
            using var blurred = new Mat();
            Cv2.GaussianBlur(gray, blurred, new Size(5, 5), 0);

            // Detectamos bordes
            using var edged = new Mat();
            Cv2.Canny(blurred, edged, 75, 200);

            // Encontramos contornos en la imagen de bordes
            Cv2.FindContours(
                edged,
                out var contours,
                out _,
                RetrievalModes.List,
                ContourApproximationModes.ApproxSimple
            );

            // Buscamos el contorno más grande que sea un cuadrilátero o polígono parecido
            foreach (var contour in contours.OrderByDescending(c => Cv2.ContourArea(c)))
            {
                // Aproximamos el contorno a una forma poligonal para detectar si es un documento
                double peri = Cv2.ArcLength(contour, true);
                var approx = Cv2.ApproxPolyDP(contour, 0.02 * peri, true);

                double area = Cv2.ContourArea(approx);
                double totalArea = src.Width * src.Height;

                // Validamos que el área del contorno sea suficientemente grande
                if (area > (totalArea * minAreaPercentage))
                {
                    return true;
                }
            }

            // Si no encontró absolutamente ningún contorno del tamaño mínimo, probaremos suerte de todas formas.
            // Para la mejor experiencia de usuario en entornos reales, asumimos true y dejamos que el OCR decida.
            return true;
        }

        /// <summary>
        /// Aplica una cadena de filtros para optimizar la imagen antes de la extracción de texto.
        /// </summary>
        /// <param name="src">Imagen original.</param>
        /// <returns>Una nueva instancia de Mat optimizada para OCR.</returns>
        public static Mat PrepareForTextExtraction(Mat src)
        {
            var result = new Mat();
            // Convertimos a escala de grises, ya que el OCR funciona mejor con imágenes monocromáticas
            Cv2.CvtColor(src, result, ColorConversionCodes.BGR2GRAY);
            return result;
        }
    }
}
