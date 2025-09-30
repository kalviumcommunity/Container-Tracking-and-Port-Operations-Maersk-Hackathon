using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Generic repository implementation for CRUD operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Gets all entities of type T
        /// </summary>
        /// <returns>IEnumerable of all entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve</param>
        /// <returns>The entity or null if not found</returns>
        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>IEnumerable of filtered entities</returns>
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>The added entity</returns>
        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity with updated values</param>
        /// <returns>The updated entity</returns>
        public async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            T entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}