using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository implementation for BerthAssignment operations
    /// </summary>
    public class BerthAssignmentRepository : Repository<BerthAssignment>, IBerthAssignmentRepository
    {
        public BerthAssignmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets assignments by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Assignments for the specified container</returns>
        public async Task<IEnumerable<BerthAssignment>> GetByContainerIdAsync(string containerId)
        {
            return await _dbSet
                .Include(ba => ba.Berth)
                .Include(ba => ba.Container)
                .Where(ba => ba.ContainerId == containerId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets assignments by berth ID
        /// </summary>
        /// <param name="berthId">The berth ID</param>
        /// <returns>Assignments for the specified berth</returns>
        public async Task<IEnumerable<BerthAssignment>> GetByBerthIdAsync(int berthId)
        {
            return await _dbSet
                .Include(ba => ba.Berth)
                .Include(ba => ba.Container)
                .Where(ba => ba.BerthId == berthId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets active assignments (where ReleasedAt is null)
        /// </summary>
        /// <returns>All active assignments</returns>
        public async Task<IEnumerable<BerthAssignment>> GetActiveAssignmentsAsync()
        {
            return await _dbSet
                .Include(ba => ba.Berth)
                .Include(ba => ba.Container)
                .Where(ba => ba.ReleasedAt == null)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets assignments within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Assignments within the specified date range</returns>
        public async Task<IEnumerable<BerthAssignment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(ba => ba.Berth)
                .Include(ba => ba.Container)
                .Where(ba => ba.AssignedAt >= startDate && 
                            (ba.ReleasedAt <= endDate || ba.ReleasedAt == null))
                .ToListAsync();
        }
        
        /// <summary>
        /// Overrides the base GetAllAsync to include navigation properties
        /// </summary>
        /// <returns>BerthAssignments with navigation properties</returns>
        public new async Task<IEnumerable<BerthAssignment>> GetAllAsync()
        {
            return await _dbSet
                .Include(ba => ba.Berth)
                .Include(ba => ba.Container)
                .ToListAsync();
        }
    }
}