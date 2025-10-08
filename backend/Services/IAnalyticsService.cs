using Backend.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Interface for analytics and reporting services
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        Task<DashboardStatsDto> GetDashboardStatsAsync();

        /// <summary>
        /// Get container throughput data
        /// </summary>
        Task<IEnumerable<ThroughputDataDto>> GetContainerThroughputAsync(string period, int days);

        /// <summary>
        /// Get berth utilization data
        /// </summary>
        Task<IEnumerable<BerthUtilizationDto>> GetBerthUtilizationAsync(int? portId, int days);

        /// <summary>
        /// Get ship turnaround data
        /// </summary>
        Task<IEnumerable<TurnaroundDataDto>> GetShipTurnaroundAsync(int? portId, int days);

        /// <summary>
        /// Get port performance data
        /// </summary>
        Task<IEnumerable<PortPerformanceDto>> GetPortPerformanceAsync(string metric, int days);

        /// <summary>
        /// Get real-time metrics
        /// </summary>
        Task<RealtimeMetricsDto> GetRealtimeMetricsAsync();

        /// <summary>
        /// Generate custom report
        /// </summary>
        Task<CustomReportDto> GenerateCustomReportAsync(CustomReportRequestDto request);

        /// <summary>
        /// Export analytics data
        /// </summary>
        Task<byte[]> ExportAnalyticsAsync(string reportType, DateTime fromDate, DateTime toDate);
    }
}
