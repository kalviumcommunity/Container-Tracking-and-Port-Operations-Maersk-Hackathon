using System;
using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Dashboard statistics DTO
    /// </summary>
    public class DashboardStatsDto
    {
        public int TotalContainers { get; set; }
        public int ActiveShips { get; set; }
        public int AvailableBerths { get; set; }
        public int TotalPorts { get; set; }
        public int TodayArrivals { get; set; }
        public int TodayDepartures { get; set; }
        public int ContainersInTransit { get; set; }
        public int ContainersAtPort { get; set; }
        public decimal AverageTurnaroundTime { get; set; }
        public decimal BerthUtilizationRate { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public List<AlertDto> Alerts { get; set; } = new();
    }

    /// <summary>
    /// Recent activity DTO for dashboard
    /// </summary>
    public class RecentActivityDto
    {
        public string Activity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = string.Empty; // arrival, departure, loading, unloading
        public string? EntityId { get; set; }
        public string? EntityName { get; set; }
    }

    /// <summary>
    /// Alert DTO for dashboard
    /// </summary>
    public class AlertDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // Info, Warning, Error, Critical
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    /// <summary>
    /// Throughput data DTO
    /// </summary>
    public class ThroughputDataDto
    {
        public DateTime Date { get; set; }
        public int ContainersProcessed { get; set; }
        public int ContainersLoaded { get; set; }
        public int ContainersUnloaded { get; set; }
        public decimal AvgProcessingTime { get; set; }
        public string Period { get; set; } = string.Empty;
    }

    /// <summary>
    /// Berth utilization DTO
    /// </summary>
    public class BerthUtilizationDto
    {
        public int BerthId { get; set; }
        public string BerthName { get; set; } = string.Empty;
        public int PortId { get; set; }
        public string PortName { get; set; } = string.Empty;
        public decimal UtilizationRate { get; set; }
        public int TotalCapacity { get; set; }
        public int AverageOccupancy { get; set; }
        public List<UtilizationDataPoint> UtilizationHistory { get; set; } = new();
    }

    /// <summary>
    /// Utilization data point for time series
    /// </summary>
    public class UtilizationDataPoint
    {
        public DateTime Timestamp { get; set; }
        public decimal Utilization { get; set; }
        public int Occupancy { get; set; }
    }

    /// <summary>
    /// Ship turnaround data DTO
    /// </summary>
    public class TurnaroundDataDto
    {
        public int ShipId { get; set; }
        public string ShipName { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public decimal TurnaroundHours { get; set; }
        public int ContainersLoaded { get; set; }
        public int ContainersUnloaded { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PortId { get; set; }
        public string PortName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Port performance DTO
    /// </summary>
    public class PortPerformanceDto
    {
        public int PortId { get; set; }
        public string PortName { get; set; } = string.Empty;
        public decimal ThroughputContainers { get; set; }
        public decimal AvgTurnaroundTime { get; set; }
        public decimal EfficiencyScore { get; set; }
        public decimal BerthUtilization { get; set; }
        public int ShipsProcessed { get; set; }
        public List<PerformanceDataPoint> PerformanceHistory { get; set; } = new();
    }

    /// <summary>
    /// Performance data point for charts
    /// </summary>
    public class PerformanceDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Metric { get; set; } = string.Empty;
    }

    /// <summary>
    /// Real-time metrics DTO
    /// </summary>
    public class RealtimeMetricsDto
    {
        public DateTime LastUpdated { get; set; }
        public int ActiveOperations { get; set; }
        public int ShipsInPort { get; set; }
        public int ShipsApproaching { get; set; }
        public int ContainersBeingProcessed { get; set; }
        public List<PortStatusDto> PortStatuses { get; set; } = new();
        public List<AlertDto> UrgentAlerts { get; set; } = new();
    }

    /// <summary>
    /// Port status for real-time monitoring
    /// </summary>
    public class PortStatusDto
    {
        public int PortId { get; set; }
        public string PortName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Operational, Congested, Maintenance
        public int QueuedShips { get; set; }
        public int AvailableBerths { get; set; }
        public int TotalBerths { get; set; }
        public decimal UtilizationRate { get; set; }
    }

    /// <summary>
    /// Custom report request DTO
    /// </summary>
    public class CustomReportRequestDto
    {
        public string ReportType { get; set; } = string.Empty; // throughput, utilization, performance
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<int>? PortIds { get; set; }
        public List<int>? BerthIds { get; set; }
        public List<string>? ContainerTypes { get; set; }
        public string Granularity { get; set; } = "daily"; // hourly, daily, weekly, monthly
        public List<string> Metrics { get; set; } = new();
        public Dictionary<string, object> Filters { get; set; } = new();
    }

    /// <summary>
    /// Custom report result DTO
    /// </summary>
    public class CustomReportDto
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Dictionary<string, object>> Data { get; set; } = new();
        public Dictionary<string, object> Summary { get; set; } = new();
        public List<string> Headers { get; set; } = new();
    }
}