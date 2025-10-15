using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service for Port operations
    /// </summary>
    public class PortService : IPortService
    {
        private readonly IPortRepository _portRepository;

        public PortService(IPortRepository portRepository)
        {
            _portRepository = portRepository;
        }

        /// <summary>
        /// Gets all ports
        /// </summary>
        /// <returns>All ports</returns>
        public async Task<IEnumerable<PortDto>> GetAllAsync()
        {
            var ports = await _portRepository.GetAllAsync();
            return ports.Select(MapToDto);
        }

        /// <summary>
        /// Gets a port by its ID
        /// </summary>
        /// <param name="id">The ID of the port</param>
        /// <returns>The port or null if not found</returns>
        public async Task<PortDto> GetByIdAsync(object id)
        {
            var port = await _portRepository.GetByIdAsync(id);
            return port == null ? null : MapToDto(port);
        }

        /// <summary>
        /// Gets a port by ID with detailed information
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Port with detailed information</returns>
        public async Task<PortDetailDto> GetPortDetailAsync(int portId)
        {
            var port = await _portRepository.GetWithBerthsAsync(portId);
            
            if (port == null)
            {
                return null;
            }
            
            return new PortDetailDto
            {
                PortId = port.PortId,
                Name = port.Name,
                Code = port.Code,
                Country = port.Country,
                Location = port.Location,
                Coordinates = port.Coordinates,
                TotalContainerCapacity = port.TotalContainerCapacity,
                CurrentContainerCount = port.CurrentContainerCount,
                MaxShipCapacity = port.MaxShipCapacity,
                CurrentShipCount = port.CurrentShipCount,
                OperatingHours = port.OperatingHours,
                TimeZone = port.TimeZone,
                ContactInfo = port.ContactInfo,
                Services = port.Services,
                Status = port.Status,
                BerthCount = port.Berths?.Count ?? 0,
                Berths = port.Berths?.Select(b => new BerthDto
                {
                    BerthId = b.BerthId,
                    Name = b.Name,
                    Capacity = b.Capacity,
                    Status = b.Status,
                    PortId = b.PortId,
                    PortName = port.Name,
                    ActiveAssignmentCount = b.BerthAssignments?.Count(ba => ba.ReleasedAt == null) ?? 0
                }).ToList()
            };
        }

        /// <summary>
        /// Gets ports by location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Ports at the specified location</returns>
        public async Task<IEnumerable<PortDto>> GetByLocationAsync(string location)
        {
            var ports = await _portRepository.GetByLocationAsync(location);
            return ports.Select(MapToDto);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        public async Task<IEnumerable<PortDto>> GetAsync(Expression<Func<Port, bool>> filter)
        {
            var ports = await _portRepository.GetAsync(filter);
            return ports.Select(MapToDto);
        }

        /// <summary>
        /// Creates a new port
        /// </summary>
        /// <param name="createDto">Port data</param>
        /// <returns>The created port</returns>
        public async Task<PortDto> CreateAsync(PortCreateUpdateDto createDto)
        {
            var port = new Port
            {
                Name = createDto.Name,
                Code = createDto.Code,
                Country = createDto.Country,
                Location = createDto.Location,
                Coordinates = createDto.Coordinates,
                TotalContainerCapacity = createDto.TotalContainerCapacity,
                CurrentContainerCount = 0, // Initialize to 0 for new port
                MaxShipCapacity = createDto.MaxShipCapacity,
                CurrentShipCount = 0, // Initialize to 0 for new port
                OperatingHours = createDto.OperatingHours,
                TimeZone = createDto.TimeZone,
                ContactInfo = createDto.ContactInfo,
                Services = createDto.Services,
                Status = createDto.Status ?? "Active" // Default to Active if not specified
            };
            
            var createdPort = await _portRepository.CreateAsync(port);
            return MapToDto(createdPort);
        }

        /// <summary>
        /// Updates an existing port
        /// </summary>
        /// <param name="id">The ID of the port to update</param>
        /// <param name="updateDto">Updated port data</param>
        /// <returns>The updated port</returns>
        public async Task<PortDto> UpdateAsync(object id, PortCreateUpdateDto updateDto)
        {
            var existingPort = await _portRepository.GetByIdAsync(id);
            if (existingPort == null)
            {
                throw new KeyNotFoundException($"Port with ID {id} not found");
            }
            
            // Update fields
            existingPort.Name = updateDto.Name;
            existingPort.Code = updateDto.Code;
            existingPort.Country = updateDto.Country;
            existingPort.Location = updateDto.Location;
            existingPort.Coordinates = updateDto.Coordinates;
            existingPort.TotalContainerCapacity = updateDto.TotalContainerCapacity;
            existingPort.MaxShipCapacity = updateDto.MaxShipCapacity;
            existingPort.OperatingHours = updateDto.OperatingHours;
            existingPort.TimeZone = updateDto.TimeZone;
            existingPort.ContactInfo = updateDto.ContactInfo;
            existingPort.Services = updateDto.Services;
            existingPort.Status = updateDto.Status;
            
            var updatedPort = await _portRepository.UpdateAsync(existingPort);
            return MapToDto(updatedPort);
        }

        /// <summary>
        /// Deletes a port
        /// </summary>
        /// <param name="id">The ID of the port to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            return await _portRepository.DeleteAsync(id);
        }
        
        /// <summary>
        /// Maps a Port entity to a PortDto
        /// </summary>
        /// <param name="port">The port entity</param>
        /// <returns>A port DTO</returns>
        private static PortDto MapToDto(Port port)
        {
            return new PortDto
            {
                PortId = port.PortId,
                Name = port.Name,
                Code = port.Code,
                Country = port.Country,
                Location = port.Location,
                Coordinates = port.Coordinates,
                TotalContainerCapacity = port.TotalContainerCapacity,
                CurrentContainerCount = port.CurrentContainerCount,
                MaxShipCapacity = port.MaxShipCapacity,
                CurrentShipCount = port.CurrentShipCount,
                OperatingHours = port.OperatingHours,
                TimeZone = port.TimeZone,
                ContactInfo = port.ContactInfo,
                Services = port.Services,
                Status = port.Status,
                BerthCount = port.Berths?.Count ?? 0
            };
        }
    }
}