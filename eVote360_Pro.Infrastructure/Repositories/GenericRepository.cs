using System.Linq.Expressions;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    public class GenericRepository<T, TId> : IGenericRepository<T, TId>
        where T : BaseEntity<TId>
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? options = null)
        {
            return await ApplyOptions(options).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(QueryOptions<T> options)
        {
            return await ApplyOptions(options).FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync(TId id, params Expression<Func<T, object>>[] includes)
        {
            // guardamos el query base del DbSet para luego aplicar las inclusiones
            IQueryable<T> query = _dbSet;

            // Iteramos sobre las expresiones de inclusión y las aplicamos al query
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Finalmente, ejecutamos el query con el filtro por Id
            return await query.FirstOrDefaultAsync(x => x.Id!.Equals(id));
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Aplica la lógica de filtrado, inclusiones, ordenamiento y paginación de forma centralizada.
        /// </summary>
        private IQueryable<T> ApplyOptions(QueryOptions<T>? options)
        {
            IQueryable<T> query = _dbSet;

            if (options == null)
                return query.AsNoTracking();

            // Control de Tracking
            if (!options.IsTracking)
            {
                query = query.AsNoTracking();
            }

            // Inclusiones (Include)
            foreach (var include in options.Includes)
            {
                query = query.Include(include);
            }

            // Filtrado (Where)
            if (options.Filter != null)
            {
                query = query.Where(options.Filter);
            }

            // Ordenamiento (OrderBy)
            if (options.OrderBy != null)
            {
                query = options.OrderBy(query);
            }

            // Paginación (Skip y Take)
            if (options.Skip.HasValue)
            {
                query = query.Skip(options.Skip.Value);
            }

            if (options.Take.HasValue)
            {
                query = query.Take(options.Take.Value);
            }

            return query;
        }
    }
}
