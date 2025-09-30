using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Generic service interface for business logic operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <typeparam name="TDto">The DTO type for the entity</typeparam>
    /// <typeparam name="TCreateUpdateDto">The DTO type for creating or updating the entity</typeparam>
    public interface IService<T, TDto, TCreateUpdateDto> where T : class
    {
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>All entities</returns>
        Task<IEnumerable<TDto>> GetAllAsync();
        
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity</param>
        /// <returns>The entity or null if not found</returns>
        Task<TDto> GetByIdAsync(object id);
        
        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        Task<IEnumerable<TDto>> GetAsync(Expression<Func<T, bool>> filter);
        
        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="createDto">DTO with entity data</param>
        /// <returns>The created entity</returns>
        Task<TDto> CreateAsync(TCreateUpdateDto createDto);
        
        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="id">The ID of the entity to update</param>
        /// <param name="updateDto">DTO with updated entity data</param>
        /// <returns>The updated entity</returns>
        Task<TDto> UpdateAsync(object id, TCreateUpdateDto updateDto);
        
        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        /// <returns>True if the entity was deleted successfully</returns>
        Task<bool> DeleteAsync(object id);
    }
}