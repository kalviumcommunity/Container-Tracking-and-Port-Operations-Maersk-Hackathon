using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Tracks ship routes and schedules for optimization and delay management
    /// </summary>
    public class ShipRoute
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShipId { get; set; }

        [StringLength(20)]
        public string? RouteNumber { get; set; }

        [Required]
        public int OriginPortId { get; set; }

        [Required]
        public int DestinationPortId { get; set; }

        [Required]
        public DateTime ScheduledDeparture { get; set; }

        [Required]
        public DateTime ScheduledArrival { get; set; }

        public DateTime? ActualDeparture { get; set; }

        public DateTime? ActualArrival { get; set; }

        [Required]
        [StringLength(20)]
        public string RouteStatus { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Delayed

        [Column(TypeName = "decimal(5,2)")]
        public decimal WeatherDelay { get; set; } = 0; // Delay in hours due to weather

        [Column(TypeName = "decimal(5,2)")]
        public decimal PortDelay { get; set; } = 0; // Delay in hours due to port operations

        [Column(TypeName = "decimal(10,2)")]
        public decimal? FuelConsumption { get; set; } // Fuel consumption in liters

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ShipId")]
        public virtual Ship Ship { get; set; }

        [ForeignKey("OriginPortId")]
        public virtual Port OriginPort { get; set; }

        [ForeignKey("DestinationPortId")]
        public virtual Port DestinationPort { get; set; }
    }
}