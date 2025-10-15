using System;

namespace Backend.DTOs
{
    /// <summary>
    /// DTO for ship route information
    /// </summary>
    public class ShipRouteDto
    {
        public int Id { get; set; }
        public int ShipId { get; set; }
        public string ShipName { get; set; }
        public string RouteNumber { get; set; }
        public int OriginPortId { get; set; }
        public string OriginPortName { get; set; }
        public int DestinationPortId { get; set; }
        public string DestinationPortName { get; set; }
        public DateTime ScheduledDeparture { get; set; }
        public DateTime ScheduledArrival { get; set; }
        public DateTime? ActualDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }
        public string RouteStatus { get; set; }
        public decimal WeatherDelay { get; set; }
        public decimal PortDelay { get; set; }
        public decimal? FuelConsumption { get; set; }
    }

    /// <summary>
    /// DTO for creating or updating a ship route
    /// </summary>
    public class ShipRouteCreateUpdateDto
    {
        public int ShipId { get; set; }
        public string RouteNumber { get; set; }
        public int OriginPortId { get; set; }
        public int DestinationPortId { get; set; }
        public DateTime ScheduledDeparture { get; set; }
        public DateTime ScheduledArrival { get; set; }
        public DateTime? ActualDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }
        public string RouteStatus { get; set; }
        public decimal WeatherDelay { get; set; }
        public decimal PortDelay { get; set; }
        public decimal? FuelConsumption { get; set; }
    }
}
