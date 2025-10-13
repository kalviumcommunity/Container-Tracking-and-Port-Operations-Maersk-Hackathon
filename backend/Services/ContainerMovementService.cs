using Backend.DTOs;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public interface IContainerMovementService
    {
        Task<IEnumerable<ContainerMovementDto>> GetByContainerIdAsync(string containerId);
        Task<ContainerMovementDto> GetByIdAsync(int id);
        Task<ContainerMovementDto> CreateAsync(ContainerMovementCreateDto createDto, int userId);
        Task<ContainerMovementDto> UpdateAsync(int id, ContainerMovementUpdateDto updateDto);
        Task<ContainerTrackingDto> GetTrackingAsync(string containerId);
        Task<bool> DeleteAsync(int id);
    }

    public class ContainerMovementService : IContainerMovementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContainerMovementService> _logger;

        public ContainerMovementService(ApplicationDbContext context, ILogger<ContainerMovementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ContainerMovementDto>> GetByContainerIdAsync(string containerId)
        {
            var movements = await _context.ContainerMovements
                .Include(cm => cm.Port)
                .Include(cm => cm.Berth)
                .Include(cm => cm.Ship)
                .Include(cm => cm.RecordedByUser)
                .Where(cm => cm.ContainerId == containerId)
                .OrderBy(cm => cm.MovementTimestamp)
                .ToListAsync();

            return movements.Select(MapToDto);
        }

        public async Task<ContainerMovementDto> GetByIdAsync(int id)
        {
            var movement = await _context.ContainerMovements
                .Include(cm => cm.Port)
                .Include(cm => cm.Berth)
                .Include(cm => cm.Ship)
                .Include(cm => cm.RecordedByUser)
                .FirstOrDefaultAsync(cm => cm.MovementId == id); // Fixed: Use MovementId instead of Id

            return movement == null ? null : MapToDto(movement);
        }

        public async Task<ContainerMovementDto> CreateAsync(ContainerMovementCreateDto createDto, int userId)
        {
            var movement = new ContainerMovement
            {
                ContainerId = createDto.ContainerId,
                MovementType = createDto.MovementType,
                FromLocation = createDto.FromLocation,
                ToLocation = createDto.ToLocation,
                MovementTimestamp = createDto.MovementTimestamp ?? DateTime.UtcNow,
                Status = createDto.Status ?? "In Progress",
                Coordinates = createDto.Coordinates ?? "0.0,0.0",
                Notes = createDto.Notes ?? string.Empty,
                PortId = createDto.PortId,
                BerthId = createDto.BerthId,
                ShipId = createDto.ShipId,
                Temperature = createDto.Temperature,
                Humidity = createDto.Humidity,
                EstimatedCompletion = createDto.EstimatedCompletion,
                RecordedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ContainerMovements.Add(movement);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(movement.MovementId); // Fixed: Use MovementId instead of Id
        }

        public async Task<ContainerMovementDto> UpdateAsync(int id, ContainerMovementUpdateDto updateDto)
        {
            var movement = await _context.ContainerMovements.FindAsync(id);
            if (movement == null)
            {
                throw new KeyNotFoundException($"Movement with ID {id} not found");
            }

            if (!string.IsNullOrEmpty(updateDto.Status))
                movement.Status = updateDto.Status;

            if (updateDto.ActualCompletion.HasValue)
                movement.ActualCompletion = updateDto.ActualCompletion.Value;

            if (!string.IsNullOrEmpty(updateDto.Coordinates))
                movement.Coordinates = updateDto.Coordinates;

            if (updateDto.Temperature.HasValue)
                movement.Temperature = updateDto.Temperature.Value;

            if (updateDto.Humidity.HasValue)
                movement.Humidity = updateDto.Humidity.Value;

            if (!string.IsNullOrEmpty(updateDto.Notes))
                movement.Notes = updateDto.Notes;

            movement.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return await GetByIdAsync(movement.MovementId); // Fixed: Use MovementId instead of Id
        }

        public async Task<ContainerTrackingDto> GetTrackingAsync(string containerId)
        {
            var container = await _context.Containers
                .Include(c => c.Ship)
                .FirstOrDefaultAsync(c => c.ContainerId == containerId);

            if (container == null)
                return null;

            var movements = await GetByContainerIdAsync(containerId);

            return new ContainerTrackingDto
            {
                ContainerId = containerId,
                CurrentLocation = container.CurrentLocation,
                Destination = container.Destination,
                Status = container.Status,
                EstimatedArrival = container.EstimatedArrival,
                ShipName = container.Ship?.Name,
                Movements = movements.ToList()
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movement = await _context.ContainerMovements.FindAsync(id);
            if (movement == null)
                return false;

            _context.ContainerMovements.Remove(movement);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ContainerMovementDto MapToDto(ContainerMovement movement)
        {
            return new ContainerMovementDto
            {
                Id = movement.MovementId, // Fixed: Use MovementId instead of Id
                ContainerId = movement.ContainerId,
                MovementType = movement.MovementType,
                FromLocation = movement.FromLocation,
                ToLocation = movement.ToLocation,
                MovementTimestamp = movement.MovementTimestamp,
                Status = movement.Status,
                Coordinates = movement.Coordinates,
                Notes = movement.Notes,
                PortId = movement.PortId,
                PortName = movement.Port?.Name,
                BerthId = movement.BerthId,
                BerthName = movement.Berth?.Name,
                ShipId = movement.ShipId,
                ShipName = movement.Ship?.Name,
                Temperature = movement.Temperature,
                Humidity = movement.Humidity,
                EstimatedCompletion = movement.EstimatedCompletion,
                ActualCompletion = movement.ActualCompletion,
                RecordedByUserId = movement.RecordedByUserId ?? 0, // Handle nullable int
                RecordedByUserName = movement.RecordedByUser?.FullName,
                CreatedAt = movement.CreatedAt
            };
        }
    }
}
