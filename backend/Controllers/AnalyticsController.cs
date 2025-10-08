using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Backend.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IAnalyticsService analyticsService,
            ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        [HttpGet("dashboard-stats")]
        [RequirePermission(Backend.Constants.Permissions.ViewPortReports)]
        [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), 200)]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _analyticsService.GetDashboardStatsAsync();
                return Ok(ApiResponse<DashboardStatsDto>.Ok(stats));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<DashboardStatsDto>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Get container throughput data
        /// </summary>
        [HttpGet("throughput")]
        [RequirePermission(Backend.Constants.Permissions.ViewPortReports)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ThroughputDataDto>>), 200)]
        public async Task<IActionResult> GetThroughputData([FromQuery] string period = "daily", [FromQuery] int days = 30)
        {
            try
            {
                var data = await _analyticsService.GetContainerThroughputAsync(period, days);
                return Ok(ApiResponse<IEnumerable<ThroughputDataDto>>.Ok(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<ThroughputDataDto>>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Get berth utilization data
        /// </summary>
        [HttpGet("berth-utilization")]
        [RequirePermission(Backend.Constants.Permissions.ViewReports)]
        public async Task<ActionResult<ApiResponse<IEnumerable<BerthUtilizationDto>>>> GetBerthUtilization(
            [FromQuery] int? portId = null,
            [FromQuery] int days = 30)
        {
            try
            {
                var utilization = await _analyticsService.GetBerthUtilizationAsync(portId, days);
                return Ok(ApiResponse<IEnumerable<BerthUtilizationDto>>.OkWithData(utilization, "Berth utilization data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving berth utilization data");
                return StatusCode(500, ApiResponse<IEnumerable<BerthUtilizationDto>>.Fail("Failed to retrieve berth utilization data"));
            }
        }

        /// <summary>
        /// Get real-time metrics
        /// </summary>
        [HttpGet("realtime-metrics")]
        [RequirePermission(Backend.Constants.Permissions.ViewDashboard)]
        public async Task<ActionResult<ApiResponse<RealtimeMetricsDto>>> GetRealtimeMetrics()
        {
            try
            {
                var metrics = await _analyticsService.GetRealtimeMetricsAsync();
                return Ok(ApiResponse<RealtimeMetricsDto>.OkWithData(metrics, "Real-time metrics retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving real-time metrics");
                return StatusCode(500, ApiResponse<RealtimeMetricsDto>.Fail("Failed to retrieve real-time metrics"));
            }
        }

        /// <summary>
        /// Generate custom report
        /// </summary>
        [HttpPost("custom-report")]
        [RequirePermission(Backend.Constants.Permissions.GenerateReports)]
        public async Task<ActionResult<ApiResponse<CustomReportDto>>> GenerateCustomReport([FromBody] CustomReportRequestDto request)
        {
            try
            {
                var report = await _analyticsService.GenerateCustomReportAsync(request);
                return Ok(ApiResponse<CustomReportDto>.OkWithData(report, "Custom report generated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating custom report");
                return StatusCode(500, ApiResponse<CustomReportDto>.Fail("Failed to generate custom report"));
            }
        }

        /// <summary>
        /// Export analytics data as CSV
        /// </summary>
        [HttpGet("export")]
        [RequirePermission(Backend.Constants.Permissions.ExportData)]
        public async Task<IActionResult> ExportAnalytics(
            [FromQuery] string reportType,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var data = await _analyticsService.ExportAnalyticsAsync(reportType, fromDate, toDate);
                return File(data, "text/csv", $"{reportType}_analytics_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting analytics data");
                return StatusCode(500, new { error = "Failed to export analytics data" });
            }
        }
    }
}