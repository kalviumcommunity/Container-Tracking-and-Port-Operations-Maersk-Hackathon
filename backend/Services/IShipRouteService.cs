using Backend.DTOs;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IShipRouteService : IService<ShipRoute, ShipRouteDto, ShipRouteCreateUpdateDto>
    {
        Task<IEnumerable<ShipRouteDto>> GetRoutesForShipAsync(int shipId);
    }
}
