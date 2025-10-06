using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents analytics and metrics data for dashboard reporting
    /// </summary>
    public class Analytics
    {
        /// <summary>
        /// Unique identifier for the analytics record
        /// </summary>
        [Key]
        public int AnalyticsId { get; set; }
        
        /// <summary>
        /// Type of metric (Container Throughput, Berth Utilization, Ship Turnaround, etc.)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MetricType { get; set; }
        
        /// <summary>
        /// Category of analytics (Operations, Performance, Efficiency, Revenue)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }
        
        /// <summary>
        /// Time period for this metric (Hourly, Daily, Weekly, Monthly)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Period { get; set; }
        
        /// <summary>
        /// Numeric value of the metric
        /// </summary>
        [Required]
        public decimal Value { get; set; }
        
        /// <summary>
        /// Unit of measurement (TEU, Hours, Percentage, Currency)
        /// </summary>
        [MaxLength(20)]
        public string Unit { get; set; }
        
        /// <summary>
        /// Target or benchmark value for comparison
        /// </summary>
        public decimal? TargetValue { get; set; }
        
        /// <summary>
        /// Previous period value for trend analysis
        /// </summary>
        public decimal? PreviousPeriodValue { get; set; }
        
        /// <summary>
        /// Percentage change from previous period
        /// </summary>
        public decimal? PercentageChange { get; set; }
        
        /// <summary>
        /// Associated port (if metric is port-specific)
        /// </summary>
        public int? PortId { get; set; }
        
        /// <summary>
        /// Associated berth (if metric is berth-specific)
        /// </summary>
        public int? BerthId { get; set; }
        
        /// <summary>
        /// Associated ship (if metric is ship-specific)
        /// </summary>
        public int? ShipId { get; set; }
        
        /// <summary>
        /// Additional metadata as JSON
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; }
        
        /// <summary>
        /// Trend direction (Increasing, Decreasing, Stable)
        /// </summary>
        [MaxLength(20)]
        public string Trend { get; set; }
        
        /// <summary>
        /// Status indicator (Good, Warning, Critical)
        /// </summary>
        [MaxLength(20)]
        public string Status { get; set; }
        
        /// <summary>
        /// Date and time this metric represents
        /// </summary>
        [Required]
        public DateTime MetricTimestamp { get; set; }
        
        /// <summary>
        /// When this record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When this record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// Associated port (if applicable)
        /// </summary>
        [ForeignKey("PortId")]
        public Port Port { get; set; }
        
        /// <summary>
        /// Associated berth (if applicable)
        /// </summary>
        [ForeignKey("BerthId")]
        public Berth Berth { get; set; }
        
        /// <summary>
        /// Associated ship (if applicable)
        /// </summary>
        [ForeignKey("ShipId")]
        public Ship Ship { get; set; }
    }
}