using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities of type T
        /// </summary>
        /// <returns>IEnumerable of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve</param>
        /// <returns>The entity or null if not found</returns>
        Task<T> GetByIdAsync(object id);
        
        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>IEnumerable of filtered entities</returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter);
        
        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>The added entity</returns>
        Task<T> CreateAsync(T entity);
        
        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity with updated values</param>
        /// <returns>The updated entity</returns>
        Task<T> UpdateAsync(T entity);
        
        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteAsync(object id);
    }
}