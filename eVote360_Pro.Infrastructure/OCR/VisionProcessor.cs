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
        public static bool ContainsDocument(Mat src, double minAreaPercentage = 0.20)
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

            // Buscamos el contorno más grande que sea un cuadrilátero
            foreach (var contour in contours.OrderByDescending(c => Cv2.ContourArea(c)))
            {
                // Aproximamos el contorno a una forma poligonal para detectar si es un cuadrilátero
                double peri = Cv2.ArcLength(contour, true);
                var approx = Cv2.ApproxPolyDP(contour, 0.02 * peri, true);

                // Si el contorno aproximado tiene 4 vértices, es un candidato a documento
                if (approx.Length == 4)
                {
                    double area = Cv2.ContourArea(approx);
                    double totalArea = src.Width * src.Height;

                    return area > (totalArea * minAreaPercentage);
                }
            }

            return false;
        }

        /// <summary>
        /// Aplica una cadena de filtros para optimizar la imagen antes de la extracción de texto.
        /// </summary>
        /// <param name="src">Imagen original.</param>
        /// <returns>Una nueva instancia de Mat optimizada para OCR.</returns>
        public static Mat PrepareForTextExtraction(Mat src)
        {
            // Creamos una nueva instancia para no modificar la imagen original
            var gray = new Mat();
            // Convertimos a escala de grises para simplificar la información y mejorar el contraste
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Creamos una nueva instancia para el resultado del filtro
            var thresholded = new Mat();

            // Aplicamos un filtro adaptativo para resaltar las áreas de texto,
            // especialmente en condiciones de iluminación no ideales
            Cv2.AdaptiveThreshold(
                gray,
                thresholded,
                255,
                AdaptiveThresholdTypes.GaussianC,
                ThresholdTypes.Binary,
                11,
                2
            );

            // Retornamos la imagen procesada, lista para ser pasada al motor OCR
            return thresholded;
        }
    }
}
