using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service for Container operations
    /// </summary>
    public class ContainerService : IContainerService
    {
        private readonly IContainerRepository _containerRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContainerService> _logger;

        public ContainerService(
            IContainerRepository containerRepository, 
            ApplicationDbContext context,
            ILogger<ContainerService> logger)
        {
            _containerRepository = containerRepository;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all containers
        /// </summary>
        /// <returns>All containers</returns>
        public async Task<IEnumerable<ContainerDto>> GetAllAsync()
        {
            var containers = await _containerRepository.GetAllAsync();
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets a container by its ID
        /// </summary>
        /// <param name="id">The ID of the container</param>
        /// <returns>The container or null if not found</returns>
        public async Task<ContainerDto> GetByIdAsync(object id)
        {
            var container = await _containerRepository.GetByIdAsync(id);
            return container == null ? null : MapToDto(container);
        }

        /// <summary>
        /// Gets a container by ID with detailed information
        /// </summary>
        /// <param name="containerId">The ID of the container</param>
        /// <returns>Container with detailed information</returns>
        public async Task<ContainerDetailDto> GetContainerDetailAsync(string containerId)
        {
            // Using the repository method to get container with details
            // This requires adding a method to the ContainerRepository
            var container = await ((ContainerRepository)_containerRepository).GetByIdWithDetailsAsync(containerId);
            
            if (container == null)
            {
                return null;
            }
            
            return new ContainerDetailDto
            {
                ContainerId = container.ContainerId,
                CargoType = container.CargoType,
                Type = container.Type,
                Status = container.Status,
                CurrentLocation = container.CurrentLocation,
                CreatedAt = container.CreatedAt,
                UpdatedAt = container.UpdatedAt,
                ShipId = container.ShipId,
                ShipName = container.Ship?.Name,
                BerthAssignments = container.BerthAssignments?.Select(ba => new BerthAssignmentDto
                {
                    Id = ba.Id,
                    ContainerId = ba.ContainerId,
                    ContainerName = container.CargoType,
                    BerthId = ba.BerthId,
                    BerthName = ba.Berth?.Name,
                    AssignedAt = ba.AssignedAt,
                    ReleasedAt = ba.ReleasedAt
                }).ToList(),
                ShipContainers = container.ShipContainers?.Select(sc => new ShipContainerDto
                {
                    Id = sc.Id,
                    ShipId = sc.ShipId,
                    ShipName = sc.Ship?.Name,
                    ContainerId = sc.ContainerId,
                    ContainerName = container.CargoType,
                    LoadedAt = sc.LoadedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Gets containers by their current location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Containers at the specified location</returns>
        public async Task<IEnumerable<ContainerDto>> GetByLocationAsync(string location)
        {
            var containers = await _containerRepository.GetByLocationAsync(location);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets containers by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Containers with the specified status</returns>
        public async Task<IEnumerable<ContainerDto>> GetByStatusAsync(string status)
        {
            var containers = await _containerRepository.GetByStatusAsync(status);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets containers by the ship they're on
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Containers on the specified ship</returns>
        public async Task<IEnumerable<ContainerDto>> GetByShipIdAsync(int shipId)
        {
            var containers = await _containerRepository.GetByShipIdAsync(shipId);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        public async Task<IEnumerable<ContainerDto>> GetAsync(Expression<Func<Container, bool>> filter)
        {
            var containers = await _containerRepository.GetAsync(filter);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="createDto">Container data</param>
        /// <returns>The created container</returns>
        public async Task<ContainerDto> CreateAsync(ContainerCreateUpdateDto createDto)
        {
            var container = new Container
            {
                ContainerId = createDto.ContainerId,
                CargoType = createDto.CargoType,
                CargoDescription = createDto.CargoDescription ?? string.Empty,
                Type = createDto.Type,
                Status = createDto.Status,
                Condition = createDto.Condition ?? "Good",
                CurrentLocation = createDto.CurrentLocation,
                Destination = createDto.Destination ?? string.Empty,
                Weight = createDto.Weight,
                MaxWeight = createDto.MaxWeight,
                Size = createDto.Size ?? string.Empty,
                Temperature = createDto.Temperature,
                Coordinates = createDto.Coordinates ?? string.Empty,
                EstimatedArrival = createDto.EstimatedArrival,
                ShipId = createDto.ShipId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            var createdContainer = await _containerRepository.CreateAsync(container);
            return MapToDto(createdContainer);
        }

        /// <summary>
        /// Updates an existing container
        /// </summary>
        /// <param name="id">The ID of the container to update</param>
        /// <param name="updateDto">Updated container data</param>
        /// <returns>The updated container</returns>
        public async Task<ContainerDto> UpdateAsync(object id, ContainerCreateUpdateDto updateDto)
        {
            var existingContainer = await _containerRepository.GetByIdAsync(id);
            if (existingContainer == null)
            {
                throw new KeyNotFoundException($"Container with ID {id} not found");
            }
            
            // Update ALL available fields - this was the missing piece
            if (!string.IsNullOrEmpty(updateDto.CargoType))
                existingContainer.CargoType = updateDto.CargoType;
            
            if (!string.IsNullOrEmpty(updateDto.CargoDescription))
                existingContainer.CargoDescription = updateDto.CargoDescription;
            
            if (!string.IsNullOrEmpty(updateDto.Type))
                existingContainer.Type = updateDto.Type;
            
            if (!string.IsNullOrEmpty(updateDto.Status))
                existingContainer.Status = updateDto.Status;
            
            if (!string.IsNullOrEmpty(updateDto.Condition))
                existingContainer.Condition = updateDto.Condition;
            
            if (!string.IsNullOrEmpty(updateDto.CurrentLocation))
                existingContainer.CurrentLocation = updateDto.CurrentLocation;
            
            if (!string.IsNullOrEmpty(updateDto.Destination))
                existingContainer.Destination = updateDto.Destination;
            
            if (updateDto.Weight > 0)
                existingContainer.Weight = updateDto.Weight;
            
            if (updateDto.MaxWeight.HasValue)
                existingContainer.MaxWeight = updateDto.MaxWeight;
            
            if (!string.IsNullOrEmpty(updateDto.Size))
                existingContainer.Size = updateDto.Size;
            
            if (updateDto.Temperature.HasValue)
                existingContainer.Temperature = updateDto.Temperature;
            
            if (!string.IsNullOrEmpty(updateDto.Coordinates))
                existingContainer.Coordinates = updateDto.Coordinates;
            
            if (updateDto.EstimatedArrival.HasValue)
                existingContainer.EstimatedArrival = updateDto.EstimatedArrival;
            
            if (updateDto.ShipId.HasValue)
                existingContainer.ShipId = updateDto.ShipId;
            
            existingContainer.UpdatedAt = DateTime.UtcNow;
            
            var updatedContainer = await _containerRepository.UpdateAsync(existingContainer);
            return MapToDto(updatedContainer);
        }

        /// <summary>
        /// Deletes a container
        /// </summary>
        /// <param name="id">The ID of the container to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            return await _containerRepository.DeleteAsync(id);
        }
        
        /// <summary>
        /// Maps a Container entity to a ContainerDto
        /// </summary>
        /// <param name="container">The container entity</param>
        /// <returns>A container DTO</returns>
        private static ContainerDto MapToDto(Container container)
        {
            return new ContainerDto
            {
                ContainerId = container.ContainerId,
                CargoType = container.CargoType,
                Type = container.Type,
                Status = container.Status,
                CurrentLocation = container.CurrentLocation,
                Destination = container.Destination,
                Weight = container.Weight,
                MaxWeight = container.MaxWeight,
                CargoDescription = container.CargoDescription,
                Size = container.Size,
                Temperature = container.Temperature,
                Condition = container.Condition,
                Coordinates = container.Coordinates,
                EstimatedArrival = container.EstimatedArrival,
                CreatedAt = container.CreatedAt,
                UpdatedAt = container.UpdatedAt,
                ShipId = container.ShipId,
                ShipName = container.Ship?.Name ?? string.Empty
            };
        }

        /// <summary>
        /// Gets containers with filtering and pagination
        /// </summary>
        public async Task<PaginatedResponse<ContainerDto>> GetFilteredContainersAsync(ContainerFilterDto filter)
        {
            var query = _context.Containers.AsQueryable();

            // Apply filters - enhanced to support all fields
            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(c => c.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.Type))
                query = query.Where(c => c.Type == filter.Type);

            if (!string.IsNullOrEmpty(filter.CargoType))
                query = query.Where(c => c.CargoType == filter.CargoType);

            if (!string.IsNullOrEmpty(filter.CurrentLocation))
                query = query.Where(c => c.CurrentLocation.Contains(filter.CurrentLocation));

            if (!string.IsNullOrEmpty(filter.Destination))
                query = query.Where(c => c.Destination.Contains(filter.Destination));

            if (filter.ShipId.HasValue)
                query = query.Where(c => c.ShipId == filter.ShipId);

            if (filter.CreatedAfter.HasValue)
                query = query.Where(c => c.CreatedAt >= filter.CreatedAfter);

            if (filter.CreatedBefore.HasValue)
                query = query.Where(c => c.CreatedAt <= filter.CreatedBefore);

            if (filter.MinWeight.HasValue)
                query = query.Where(c => c.Weight >= filter.MinWeight);

            if (filter.MaxWeight.HasValue)
                query = query.Where(c => c.Weight <= filter.MaxWeight);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(c => 
                    c.ContainerId.Contains(filter.SearchTerm) ||
                    c.CargoType.Contains(filter.SearchTerm) ||
                    c.CargoDescription.Contains(filter.SearchTerm) ||
                    c.CurrentLocation.Contains(filter.SearchTerm) ||
                    c.Destination.Contains(filter.SearchTerm));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "containerid":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.ContainerId)
                            : query.OrderBy(c => c.ContainerId);
                        break;
                    case "status":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.Status)
                            : query.OrderBy(c => c.Status);
                        break;
                    case "cargotype":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.CargoType)
                            : query.OrderBy(c => c.CargoType);
                        break;
                    case "currentlocation":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.CurrentLocation)
                            : query.OrderBy(c => c.CurrentLocation);
                        break;
                    case "destination":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.Destination)
                            : query.OrderBy(c => c.Destination);
                        break;
                    case "weight":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.Weight)
                            : query.OrderBy(c => c.Weight);
                        break;
                    case "createdat":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.CreatedAt)
                            : query.OrderBy(c => c.CreatedAt);
                        break;
                    case "updatedat":
                        query = filter.SortDirection?.ToLower() == "desc" 
                            ? query.OrderByDescending(c => c.UpdatedAt)
                            : query.OrderBy(c => c.UpdatedAt);
                        break;
                    default:
                        query = query.OrderByDescending(c => c.UpdatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(c => c.UpdatedAt);
            }

            // Apply pagination
            var containers = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Include(c => c.Ship)
                .Select(c => new ContainerDto
                {
                    ContainerId = c.ContainerId,
                    CargoType = c.CargoType,
                    CargoDescription = c.CargoDescription,
                    Type = c.Type,
                    Status = c.Status,
                    Condition = c.Condition,
                    CurrentLocation = c.CurrentLocation,
                    Destination = c.Destination,
                    Weight = c.Weight,
                    MaxWeight = c.MaxWeight,
                    Size = c.Size,
                    Temperature = c.Temperature,
                    Coordinates = c.Coordinates,
                    EstimatedArrival = c.EstimatedArrival,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ShipId = c.ShipId,
                    ShipName = c.Ship != null ? c.Ship.Name : string.Empty
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            return new PaginatedResponse<ContainerDto>
            {
                Data = containers,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasNextPage = filter.Page < totalPages,
                HasPreviousPage = filter.Page > 1
            };
        }

        /// <summary>
        /// Gets container statistics
        /// </summary>
        public async Task<ContainerStatsDto> GetContainerStatisticsAsync()
        {
            var totalContainers = await _context.Containers.CountAsync();
            var availableContainers = await _context.Containers.CountAsync(c => c.Status == "Available");
            var inTransitContainers = await _context.Containers.CountAsync(c => c.Status == "In Transit");
            var atPortContainers = await _context.Containers.CountAsync(c => c.Status == "At Port");
            var loadingContainers = await _context.Containers.CountAsync(c => c.Status == "Loading");
            var unloadingContainers = await _context.Containers.CountAsync(c => c.Status == "Unloading");

            var containersByType = await _context.Containers
                .GroupBy(c => c.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);

            var containersByStatus = await _context.Containers
                .GroupBy(c => c.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            var containersByLocation = await _context.Containers
                .GroupBy(c => c.CurrentLocation)
                .Select(g => new { Location = g.Key, Count = g.Count() })
                .Take(10) // Top 10 locations
                .ToDictionaryAsync(x => x.Location, x => x.Count);

            return new ContainerStatsDto
            {
                TotalContainers = totalContainers,
                AvailableContainers = availableContainers,
                InTransitContainers = inTransitContainers,
                AtPortContainers = atPortContainers,
                LoadingContainers = loadingContainers,
                UnloadingContainers = unloadingContainers,
                ContainersByType = containersByType,
                ContainersByStatus = containersByStatus,
                ContainersByLocation = containersByLocation
            };
        }

        /// <summary>
        /// Bulk update container statuses
        /// </summary>
        public async Task<BulkUpdateResultDto> BulkUpdateStatusAsync(BulkStatusUpdateDto bulkUpdate)
        {
            var result = new BulkUpdateResultDto();
            
            foreach (var containerId in bulkUpdate.ContainerIds)
            {
                try
                {
                    var container = await _context.Containers.FindAsync(containerId);
                    if (container != null)
                    {
                        container.Status = bulkUpdate.NewStatus;
                        container.UpdatedAt = DateTime.UtcNow;
                        result.SuccessCount++;
                    }
                    else
                    {
                        result.FailedCount++;
                        result.FailedContainerIds.Add(containerId);
                        result.ErrorMessages.Add($"Container {containerId} not found");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.FailedContainerIds.Add(containerId);
                    result.ErrorMessages.Add($"Error updating {containerId}: {ex.Message}");
                    _logger.LogError(ex, "Error updating container {ContainerId}", containerId);
                }
            }

            await _context.SaveChangesAsync();
            return result;
        }

        /// <summary>
        /// Export containers to CSV
        /// </summary>
        public async Task<byte[]> ExportContainersAsync(ContainerFilterDto filter)
        {
            // Set a high page size for export
            filter.PageSize = 10000;
            filter.Page = 1;
            
            var containersResponse = await GetFilteredContainersAsync(filter);
            var containers = containersResponse.Data;

            var csv = new StringBuilder();
            
            // Headers
            csv.AppendLine("Container ID,Cargo Type,Type,Status,Current Location,Destination,Weight,Size,Created At,Updated At");
            
            // Data
            foreach (var container in containers)
            {
                csv.AppendLine($"{container.ContainerId},{container.CargoType},{container.Type}," +
                             $"{container.Status},{container.CurrentLocation},{container.Destination}," +
                             $"{container.Weight},{container.Size},{container.CreatedAt:yyyy-MM-dd}," +
                             $"{container.UpdatedAt:yyyy-MM-dd}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}