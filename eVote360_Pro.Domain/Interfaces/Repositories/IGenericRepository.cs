using System.Linq.Expressions;
using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repositorio generico base para operaciones CRUD estandar a lo largo del project
    /// </summary>
    /// <typeparam name="T">La entidad que hereda de BaseEntity</typeparam>
    public interface IGenericRepository<T>
        where T : BaseEntity
    {
        /// <summary>
        /// Obtiene todos los registros según las opciones de consulta proporcionadas.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null);

        /// <summary>
        /// Obtiene el primer registro que cumpla con la condición especificada en las opciones.
        /// </summary>
        Task<T?> GetFirstOrDefaultAsync(QueryOptions<T> options);

        /// <summary>
        /// Obtiene un registro por su ID con soporte opcional para inclusión de propiedades de navegación.
        /// </summary>
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Agrega un nuevo registro.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Agrega una colección de registros.
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Marca un registro para actualización.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Marca un registro para eliminación.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Verifica si existe algún registro que cumpla con la condición.
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Obtiene la cantidad de registros que cumplen con una condición opcional.
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
