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
        /// Obtiene todos los registros de la entidad T. (SIN TRACKING OJO IMPLEMENTARLO ASI EN TODOS LOS REPOSITORIOS)
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro por su ID
        /// </summary>
        /// <param name="id">El ID del registro a obtener</param>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Agrega un nuevo registro de la entidad T a la base de datos
        /// </summary>
        /// <param name="entity">La entidad a agregar</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Actualiza un registro existente de la entidad T en la base de datos
        /// </summary>
        /// <param name="entity">La entidad a actualizar</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Elimina un registro de la entidad T por su ID
        /// </summary>
        /// <param name="id">El ID del registro a eliminar</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Verifica si existe un registro de la entidad T con el ID especificado
        /// </summary>
        /// <param name="id">El ID del registro a verificar</param>
        Task<bool> ExistsByIdAsync(int id);

        /// <summary>
        /// Obtiene la cantidad de registros que cumplen con una condición opcional.
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
