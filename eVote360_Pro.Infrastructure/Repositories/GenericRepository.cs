using System.Linq.Expressions;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Infrastructure.Contexts;
using eVote360_Pro.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Domain.Repositories
{
    /// <summary>
    /// Repositorio genérico base para operaciones CRUD estándar a lo largo del proyecto.
    /// Asi no se nos queman las manos de tanto escribir cruds xd
    /// </summary>
    /// <typeparam name="T">La entidad que hereda de BaseEntity</typeparam>
    public class GenericRepository<T> : IGenericRepository<T>
        where T : BaseEntity
    {
        // Protected para que si un repositorio en especifico hereda de este,
        // pueda acceder al contexto y al DbSet de la entidad T si es q requiere
        // implementar mas metodos aparte de los estandar del CRUD
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Obtiene todos los registros de la entidad T.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro por su ID.
        /// </summary>
        /// <param name="id">EL ID del registro a obtener</param>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Agrega un nuevo registro de la entidad T a la base de datos.
        /// </summary>
        /// <param name="entity">El registro a agregar</param>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un registro de la entidad T en la base de datos.
        /// </summary>
        /// <param name="entity">El registro a actualizar</param>
        public async Task UpdateAsync(T entity)
        {
            // La marcamos nosotros como modificada para que el contexto sepa que debe actualizarla
            // De esa forma actualizamos sin importar q no este trackeada
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un registro de la entidad T de la base de datos.
        /// </summary>
        /// <param name="id">El ID del registro a eliminar</param>
        public async Task DeleteAsync(int id)
        {
            // Creamos una instancia ligera de la entidad con solo el ID para evitar cargarla completamente desde la base de datos
            var entity = _context.Set<T>().Local.FirstOrDefault(e => e.Id == id);
            if (entity == null)
            {
                // Si no está en memoria local, creamos la instancia ligera
                entity = Activator.CreateInstance<T>();
                entity.Id = id;
                // Lo adjuntamos directamente marcándolo para borrado
                _context.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                _dbSet.Remove(entity);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica si existe un registro de la entidad T con el ID especificado.
        /// </summary>
        /// <param name="id">El ID del registro a verificar</param>
        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Obtiene la cantidad de registros que cumplen con una condición opcional.
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }

            return await _dbSet.CountAsync(predicate);
        }
    }
}
