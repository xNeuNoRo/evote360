using System.Linq.Expressions;

namespace eVote360_Pro.Domain.Common
{
    /// <summary>
    /// Encapsula las opciones de consulta para los repositorios, 
    /// permitiendo un filtrado, ordenamiento e inclusión de datos relacionados más limpio.
    /// </summary>
    /// <typeparam name="T">La entidad sobre la que se realiza la consulta.</typeparam>
    public class QueryOptions<T>
    {
        /// <summary>
        /// Predicado de filtrado (el WHERE).
        /// </summary>
        public Expression<Func<T, bool>>? Filter { get; set; }

        /// <summary>
        /// Lista de propiedades de navegación a incluir.
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; set; } = new();

        /// <summary>
        /// Función de ordenamiento de los resultados.
        /// </summary>
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }

        /// <summary>
        /// Cantidad de registros a saltar.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Cantidad de registros a tomar.
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// Indica si se debe realizar el seguimiento de cambios (Tracking) en las entidades.
        /// Por defecto es false para optimizar el rendimiento en lecturas.
        /// </summary>
        public bool IsTracking { get; set; } = false;
    }
}
