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
    public class ShipRouteService : IShipRouteService
    {
        private readonly IShipRouteRepository _shipRouteRepository;

        public ShipRouteService(IShipRouteRepository shipRouteRepository)
        {
            _shipRouteRepository = shipRouteRepository;
        }

        public async Task<ShipRouteDto> CreateAsync(ShipRouteCreateUpdateDto createDto)
        {
            var shipRoute = new ShipRoute
            {
                ShipId = createDto.ShipId,
                RouteNumber = createDto.RouteNumber,
                OriginPortId = createDto.OriginPortId,
                DestinationPortId = createDto.DestinationPortId,
                ScheduledDeparture = createDto.ScheduledDeparture,
                ScheduledArrival = createDto.ScheduledArrival,
                ActualDeparture = createDto.ActualDeparture,
                ActualArrival = createDto.ActualArrival,
                RouteStatus = createDto.RouteStatus,
                WeatherDelay = createDto.WeatherDelay,
                PortDelay = createDto.PortDelay,
                FuelConsumption = createDto.FuelConsumption
            };
            var newShipRoute = await _shipRouteRepository.CreateAsync(shipRoute);
            return MapToDto(newShipRoute);
        }

        public async Task<bool> DeleteAsync(object id)
        {
            return await _shipRouteRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShipRouteDto>> GetAsync(Expression<Func<ShipRoute, bool>> filter)
        {
            var shipRoutes = await _shipRouteRepository.GetAsync(filter);
            return shipRoutes.Select(MapToDto);
        }

        public async Task<IEnumerable<ShipRouteDto>> GetAllAsync()
        {
            var shipRoutes = await _shipRouteRepository.GetAllAsync();
            return shipRoutes.Select(MapToDto);
        }

        public async Task<ShipRouteDto> GetByIdAsync(object id)
        {
            var shipRoute = await _shipRouteRepository.GetByIdAsync(id);
            return shipRoute == null ? null : MapToDto(shipRoute);
        }

        public async Task<IEnumerable<ShipRouteDto>> GetRoutesForShipAsync(int shipId)
        {
            var shipRoutes = await _shipRouteRepository.GetByShipIdAsync(shipId);
            return shipRoutes.Select(MapToDto);
        }

        public async Task<ShipRouteDto> UpdateAsync(object id, ShipRouteCreateUpdateDto updateDto)
        {
            var existingShipRoute = await _shipRouteRepository.GetByIdAsync(id);
            if (existingShipRoute == null)
            {
                throw new KeyNotFoundException("ShipRoute not found");
            }

            existingShipRoute.ShipId = updateDto.ShipId;
            existingShipRoute.RouteNumber = updateDto.RouteNumber;
            existingShipRoute.OriginPortId = updateDto.OriginPortId;
            existingShipRoute.DestinationPortId = updateDto.DestinationPortId;
            existingShipRoute.ScheduledDeparture = updateDto.ScheduledDeparture;
            existingShipRoute.ScheduledArrival = updateDto.ScheduledArrival;
            existingShipRoute.ActualDeparture = updateDto.ActualDeparture;
            existingShipRoute.ActualArrival = updateDto.ActualArrival;
            existingShipRoute.RouteStatus = updateDto.RouteStatus;
            existingShipRoute.WeatherDelay = updateDto.WeatherDelay;
            existingShipRoute.PortDelay = updateDto.PortDelay;
            existingShipRoute.FuelConsumption = updateDto.FuelConsumption;

            await _shipRouteRepository.UpdateAsync(existingShipRoute);
            return MapToDto(existingShipRoute);
        }

        private static ShipRouteDto MapToDto(ShipRoute shipRoute)
        {
            return new ShipRouteDto
            {
                Id = shipRoute.Id,
                ShipId = shipRoute.ShipId,
                ShipName = shipRoute.Ship?.Name,
                RouteNumber = shipRoute.RouteNumber,
                OriginPortId = shipRoute.OriginPortId,
                OriginPortName = shipRoute.OriginPort?.Name,
                DestinationPortId = shipRoute.DestinationPortId,
                DestinationPortName = shipRoute.DestinationPort?.Name,
                ScheduledDeparture = shipRoute.ScheduledDeparture,
                ScheduledArrival = shipRoute.ScheduledArrival,
                ActualDeparture = shipRoute.ActualDeparture,
                ActualArrival = shipRoute.ActualArrival,
                RouteStatus = shipRoute.RouteStatus,
                WeatherDelay = shipRoute.WeatherDelay,
                PortDelay = shipRoute.PortDelay,
                FuelConsumption = shipRoute.FuelConsumption
            };
        }
    }
}
