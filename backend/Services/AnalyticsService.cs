using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Backend.Services
{
    /// <summary>
    /// Implementation of Analytics service for generating reports and statistics
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int? portId = null)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;

            // Base queries - filter by port if specified
            var containerQuery = _context.Containers.AsQueryable();
            var shipQuery = _context.Ships.AsQueryable();
            var berthQuery = _context.Berths.AsQueryable();
            var berthAssignmentQuery = _context.BerthAssignments.AsQueryable();

            if (portId.HasValue)
            {
                // Filter by specific port
                var portName = await _context.Ports.Where(p => p.PortId == portId.Value).Select(p => p.Name).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(portName))
                {
                    containerQuery = containerQuery.Where(c => c.CurrentLocation == portName);
                    shipQuery = shipQuery.Where(s => s.CurrentPortId == portId.Value || s.NextPort == portName);
                    berthQuery = berthQuery.Where(b => b.PortId == portId.Value);
                    berthAssignmentQuery = berthAssignmentQuery.Where(ba => ba.Berth.PortId == portId.Value);
                }
            }

            // Get basic counts
            var totalContainers = await containerQuery.CountAsync();
            var activeShips = await shipQuery.CountAsync(s => s.Status == "In Port" || s.Status == "Approaching");
            var availableBerths = await berthQuery.CountAsync(b => b.Status == "Available");
            var totalPorts = portId.HasValue ? 1 : await _context.Ports.CountAsync();

            // Get today's arrivals and departures
            var todayArrivals = await berthAssignmentQuery
                .CountAsync(ba => ba.ActualArrival.HasValue && ba.ActualArrival.Value.Date == today);
            var todayDepartures = await berthAssignmentQuery
                .CountAsync(ba => ba.ActualDeparture.HasValue && ba.ActualDeparture.Value.Date == today);

            // Container statistics
            var containersInTransit = await containerQuery.CountAsync(c => c.Status == "In Transit");
            var containersAtPort = await containerQuery.CountAsync(c => c.Status == "At Port");

            // Calculate average turnaround time (in hours)
            var avgTurnaround = await berthAssignmentQuery
                .Where(ba => ba.ActualArrival.HasValue && ba.ActualDeparture.HasValue)
                .AverageAsync(ba => ba.ActualDeparture.HasValue && ba.ActualArrival.HasValue 
                    ? (decimal)((ba.ActualDeparture.Value - ba.ActualArrival.Value).TotalHours) 
                    : 0);

            // Calculate berth utilization rate
            var totalBerthCapacity = await berthQuery.SumAsync(b => b.Capacity);
            var currentBerthLoad = await berthQuery.SumAsync(b => b.CurrentLoad);
            var berthUtilization = totalBerthCapacity > 0 ? (decimal)currentBerthLoad / totalBerthCapacity * 100 : 0;

            // Get recent activities
            var recentActivities = await GetRecentActivitiesAsync(10);

            // Get alerts (from events with high priority)
            var alertsQuery = _context.Events
                .Where(e => (e.Priority == "Critical" || e.Priority == "High") && !e.IsResolved);
            
            if (portId.HasValue)
            {
                // Filter events by port if available in the event data
                alertsQuery = alertsQuery.Where(e => e.PortId == portId.Value || e.PortId == null);
            }

            var alerts = await alertsQuery
                .OrderByDescending(e => e.EventTimestamp)
                .Take(5)
                .Select(e => new AlertDto
                {
                    Id = e.EventId,
                    Title = e.Title,
                    Message = e.Description,
                    Severity = e.Priority,
                    CreatedAt = e.CreatedAt,
                    IsRead = e.AcknowledgedAt.HasValue
                })
                .ToListAsync();

            return new DashboardStatsDto
            {
                TotalContainers = totalContainers,
                ActiveShips = activeShips,
                AvailableBerths = availableBerths,
                TotalPorts = totalPorts,
                TodayArrivals = todayArrivals,
                TodayDepartures = todayDepartures,
                ContainersInTransit = containersInTransit,
                ContainersAtPort = containersAtPort,
                AverageTurnaroundTime = avgTurnaround,
                BerthUtilizationRate = berthUtilization,
                RecentActivities = recentActivities.ToList(),
                Alerts = alerts
            };
        }

        /// <summary>
        /// Get container throughput data
        /// </summary>
        public async Task<IEnumerable<ThroughputDataDto>> GetContainerThroughputAsync(string period, int days)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-days);

            var movements = await _context.ContainerMovements
                .Where(cm => cm.MovementTimestamp >= startDate && cm.MovementTimestamp <= endDate)
                .ToListAsync();

            var periodLower = period.ToLower();
            var groupedData = movements.GroupBy(cm =>
            {
                var ts = cm.MovementTimestamp;
                return periodLower switch
                {
                    "hourly" => new DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, 0, 0),
                    "weekly" => GetDateFromWeek(ts.Year, GetWeekOfYear(ts)),
                    "monthly" => new DateTime(ts.Year, ts.Month, 1),
                    _ => ts.Date
                };
            });

            var result = new List<ThroughputDataDto>();

            foreach (var group in groupedData)
            {
                var date = group.Key;

                var loaded = group.Count(cm => cm.MovementType == "Loading");
                var unloaded = group.Count(cm => cm.MovementType == "Unloading");
                var avgProcessingTime = group.Any() ? 
                    (decimal)group.Where(cm => cm.ActualCompletion.HasValue)
                        .Average(cm => (cm.ActualCompletion!.Value - cm.MovementTimestamp).TotalMinutes) : 0;

                result.Add(new ThroughputDataDto
                {
                    Date = date,
                    ContainersProcessed = group.Count(),
                    ContainersLoaded = loaded,
                    ContainersUnloaded = unloaded,
                    AvgProcessingTime = avgProcessingTime,
                    Period = period
                });
            }

            return result.OrderBy(r => r.Date);
        }

        /// <summary>
        /// Get berth utilization data
        /// </summary>
        public async Task<IEnumerable<BerthUtilizationDto>> GetBerthUtilizationAsync(int? portId, int days)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-days);

            var query = _context.Berths.Include(b => b.Port).AsQueryable();
            
            if (portId.HasValue)
                query = query.Where(b => b.PortId == portId.Value);

            var berths = await query.ToListAsync();
            var result = new List<BerthUtilizationDto>();

            foreach (var berth in berths)
            {
                var assignments = await _context.BerthAssignments
                    .Where(ba => ba.BerthId == berth.BerthId && 
                                ba.AssignedAt >= startDate && ba.AssignedAt <= endDate)
                    .ToListAsync();

                var totalHours = (decimal)(endDate - startDate).TotalHours;
                var occupiedHours = assignments.Sum(ba => 
                {
                    DateTime? start = ba.ActualArrival ?? ba.ScheduledArrival;
                    DateTime? end = ba.ActualDeparture ?? ba.ScheduledDeparture;
                    var effectiveEnd = end ?? endDate;
                    return start.HasValue ? (decimal)(effectiveEnd - start.Value).TotalHours : 0;
                });

                var utilizationRate = totalHours > 0 ? (occupiedHours / totalHours) * 100 : 0;
                var avgOccupancy = assignments.Any() ? 
                    (int)assignments.Average(ba => ba.ContainerCount ?? 0) : 0;

                // Generate utilization history (daily data points)
                var utilizationHistory = new List<UtilizationDataPoint>();
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var dailyAssignments = assignments.Where(ba => 
                        ba.ActualArrival?.Date <= date && 
                        (ba.ActualDeparture?.Date ?? endDate.Date) >= date).ToList();
                    
                    var dailyOccupancy = dailyAssignments.Sum(ba => ba.ContainerCount ?? 0);
                    var dailyUtilization = berth.Capacity > 0 ? 
                        (decimal)dailyOccupancy / berth.Capacity * 100 : 0;

                    utilizationHistory.Add(new UtilizationDataPoint
                    {
                        Timestamp = date,
                        Utilization = dailyUtilization,
                        Occupancy = dailyOccupancy
                    });
                }

                result.Add(new BerthUtilizationDto
                {
                    BerthId = berth.BerthId,
                    BerthName = berth.Name,
                    PortId = berth.PortId,
                    PortName = berth.Port.Name,
                    UtilizationRate = utilizationRate,
                    TotalCapacity = berth.Capacity,
                    AverageOccupancy = avgOccupancy,
                    UtilizationHistory = utilizationHistory
                });
            }

            return result;
        }

        /// <summary>
        /// Get ship turnaround data
        /// </summary>
        public async Task<IEnumerable<TurnaroundDataDto>> GetShipTurnaroundAsync(int? portId, int days)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-days);

            var query = _context.BerthAssignments
                .Include(ba => ba.Berth)
                    .ThenInclude(b => b.Port)
                .Where(ba => ba.ActualArrival >= startDate && ba.ActualArrival <= endDate);

            if (portId.HasValue)
                query = query.Where(ba => ba.Berth.PortId == portId.Value);

            var assignments = await query.ToListAsync();

            var result = new List<TurnaroundDataDto>();

            foreach (var assignment in assignments)
            {
                var ship = await _context.Ships.FindAsync(assignment.ShipId);
                if (ship == null) continue;

                DateTime? arrivalTime = assignment.ActualArrival ?? assignment.ScheduledArrival;
                DateTime? departureTime = assignment.ActualDeparture ?? assignment.ScheduledDeparture;
                
                var turnaroundHours = departureTime.HasValue && arrivalTime.HasValue ? 
                    (decimal)(departureTime.Value - arrivalTime.Value).TotalHours : 0;

                // Get container operations for this assignment
                var containerOps = await _context.ContainerMovements
                    .Where(cm => cm.ShipId == ship.ShipId && 
                                cm.MovementTimestamp >= arrivalTime && 
                                cm.MovementTimestamp <= (departureTime ?? endDate))
                    .ToListAsync();

                result.Add(new TurnaroundDataDto
                {
                    ShipId = ship.ShipId,
                    ShipName = ship.Name,
                    ArrivalTime = arrivalTime ?? DateTime.MinValue,
                    DepartureTime = departureTime ?? DateTime.MinValue,
                    TurnaroundHours = turnaroundHours,
                    ContainersLoaded = containerOps.Count(co => co.MovementType == "Loading"),
                    ContainersUnloaded = containerOps.Count(co => co.MovementType == "Unloading"),
                    Status = assignment.Status,
                    PortId = assignment.Berth.PortId,
                    PortName = assignment.Berth.Port.Name
                });
            }

            return result.OrderBy(r => r.ArrivalTime);
        }

        /// <summary>
        /// Get port performance data
        /// </summary>
        public async Task<IEnumerable<PortPerformanceDto>> GetPortPerformanceAsync(string metric, int days)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-days);

            var ports = await _context.Ports
                .Include(p => p.Berths)
                .ToListAsync();

            var result = new List<PortPerformanceDto>();

            foreach (var port in ports)
            {
                var berthIds = port.Berths.Select(b => b.BerthId).ToList();
                
                var assignments = await _context.BerthAssignments
                    .Where(ba => berthIds.Contains(ba.BerthId) && 
                                ba.ActualArrival >= startDate && ba.ActualArrival <= endDate)
                    .ToListAsync();

                var containers = await _context.Containers
                    .Where(c => c.CurrentLocation == port.Name && 
                               c.CreatedAt >= startDate && c.CreatedAt <= endDate)
                    .CountAsync();

                var avgTurnaround = assignments.Any(a => a.ActualDeparture.HasValue && a.ActualArrival.HasValue)
                    ? assignments.Where(a => a.ActualDeparture.HasValue && a.ActualArrival.HasValue)
                        .Average(a => (a.ActualDeparture!.Value - a.ActualArrival!.Value).TotalHours)
                    : 0;

                var totalCapacity = port.Berths.Sum(b => b.Capacity);
                var currentLoad = port.Berths.Sum(b => b.CurrentLoad);
                var berthUtilization = totalCapacity > 0 ? (decimal)currentLoad / totalCapacity * 100 : 0;

                // Calculate efficiency score (based on turnaround time and utilization)
                var efficiencyScore = avgTurnaround > 0 ? 
                    Math.Max(0, 100 - (decimal)avgTurnaround / 24 * 100) * (berthUtilization / 100) : 50;

                // Generate performance history
                var performanceHistory = new List<PerformanceDataPoint>();
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var dailyValue = metric.ToLower() switch
                    {
                        "throughput" => assignments.Count(a => a.ActualArrival?.Date == date),
                        "utilization" => (double)berthUtilization,
                        "turnaround" => avgTurnaround,
                        _ => (double)efficiencyScore
                    };

                    performanceHistory.Add(new PerformanceDataPoint
                    {
                        Date = date,
                        Value = (decimal)dailyValue,
                        Metric = metric
                    });
                }

                result.Add(new PortPerformanceDto
                {
                    PortId = port.PortId,
                    PortName = port.Name,
                    ThroughputContainers = containers,
                    AvgTurnaroundTime = (decimal)avgTurnaround,
                    EfficiencyScore = efficiencyScore,
                    BerthUtilization = berthUtilization,
                    ShipsProcessed = assignments.Count,
                    PerformanceHistory = performanceHistory
                });
            }

            return result;
        }

        /// <summary>
        /// Get real-time metrics
        /// </summary>
        public async Task<RealtimeMetricsDto> GetRealtimeMetricsAsync()
        {
            var now = DateTime.UtcNow;

            var activeOperations = await _context.ContainerMovements
                .CountAsync(cm => cm.ActualCompletion == null);

            var shipsInPort = await _context.Ships
                .CountAsync(s => s.Status == "In Port");

            var shipsApproaching = await _context.Ships
                .CountAsync(s => s.Status == "Approaching");

            var containersBeingProcessed = await _context.Containers
                .CountAsync(c => c.Status == "Processing");

            // Get port statuses
            var portStatuses = await _context.Ports
                .Include(p => p.Berths)
                .Select(p => new PortStatusDto
                {
                    PortId = p.PortId,
                    PortName = p.Name,
                    Status = p.Status,
                    QueuedShips = _context.Ships.Count(s => s.NextPort == p.Name && s.Status == "Approaching"),
                    AvailableBerths = p.Berths.Count(b => b.Status == "Available"),
                    TotalBerths = p.Berths.Count(),
                    UtilizationRate = p.Berths.Any() ? 
                        (decimal)p.Berths.Sum(b => b.CurrentLoad) / p.Berths.Sum(b => b.Capacity) * 100 : 0
                })
                .ToListAsync();

            // Get urgent alerts
            var urgentAlerts = await _context.Events
                .Where(e => e.Severity == "Critical" && !e.AcknowledgedAt.HasValue)
                .OrderByDescending(e => e.EventTimestamp)
                .Take(5)
                .Select(e => new AlertDto
                {
                    Id = e.EventId,
                    Title = e.Title,
                    Message = e.Description,
                    Severity = e.Severity,
                    CreatedAt = e.CreatedAt,
                    IsRead = e.AcknowledgedAt.HasValue
                })
                .ToListAsync();

            return new RealtimeMetricsDto
            {
                LastUpdated = now,
                ActiveOperations = activeOperations,
                ShipsInPort = shipsInPort,
                ShipsApproaching = shipsApproaching,
                ContainersBeingProcessed = containersBeingProcessed,
                PortStatuses = portStatuses,
                UrgentAlerts = urgentAlerts
            };
        }

        /// <summary>
        /// Generate custom report
        /// </summary>
        public async Task<CustomReportDto> GenerateCustomReportAsync(CustomReportRequestDto request)
        {
            var data = new List<Dictionary<string, object>>();
            var summary = new Dictionary<string, object>();
            var headers = new List<string>();

            switch (request.ReportType.ToLower())
            {
                case "throughput":
                    var throughputData = await GetContainerThroughputAsync(request.Granularity, 
                        (int)(request.ToDate - request.FromDate).TotalDays);
                    
                    headers = new List<string> { "Date", "Containers Processed", "Loaded", "Unloaded", "Avg Processing Time" };
                    
                    foreach (var item in throughputData)
                    {
                        data.Add(new Dictionary<string, object>
                        {
                            ["Date"] = item.Date,
                            ["Containers Processed"] = item.ContainersProcessed,
                            ["Loaded"] = item.ContainersLoaded,
                            ["Unloaded"] = item.ContainersUnloaded,
                            ["Avg Processing Time"] = item.AvgProcessingTime
                        });
                    }

                    summary["Total Containers"] = throughputData.Sum(d => d.ContainersProcessed);
                    summary["Average Daily Throughput"] = throughputData.Any() ? 
                        throughputData.Average(d => d.ContainersProcessed) : 0;
                    break;

                case "utilization":
                    var utilizationData = await GetBerthUtilizationAsync(null, 
                        (int)(request.ToDate - request.FromDate).TotalDays);
                    
                    headers = new List<string> { "Berth", "Port", "Utilization Rate", "Capacity", "Avg Occupancy" };
                    
                    foreach (var item in utilizationData)
                    {
                        data.Add(new Dictionary<string, object>
                        {
                            ["Berth"] = item.BerthName,
                            ["Port"] = item.PortName,
                            ["Utilization Rate"] = item.UtilizationRate,
                            ["Capacity"] = item.TotalCapacity,
                            ["Avg Occupancy"] = item.AverageOccupancy
                        });
                    }

                    summary["Average Utilization"] = utilizationData.Any() ? 
                        utilizationData.Average(d => d.UtilizationRate) : 0;
                    break;

                case "performance":
                    var performanceData = await GetPortPerformanceAsync("efficiency", 
                        (int)(request.ToDate - request.FromDate).TotalDays);
                    
                    headers = new List<string> { "Port", "Throughput", "Avg Turnaround", "Efficiency", "Ships Processed" };
                    
                    foreach (var item in performanceData)
                    {
                        data.Add(new Dictionary<string, object>
                        {
                            ["Port"] = item.PortName,
                            ["Throughput"] = item.ThroughputContainers,
                            ["Avg Turnaround"] = item.AvgTurnaroundTime,
                            ["Efficiency"] = item.EfficiencyScore,
                            ["Ships Processed"] = item.ShipsProcessed
                        });
                    }

                    summary["Average Efficiency"] = performanceData.Any() ? 
                        performanceData.Average(d => d.EfficiencyScore) : 0;
                    break;
            }

            return new CustomReportDto
            {
                ReportType = request.ReportType,
                GeneratedAt = DateTime.UtcNow,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Data = data,
                Summary = summary,
                Headers = headers
            };
        }

        /// <summary>
        /// Export analytics data
        /// </summary>
        public async Task<byte[]> ExportAnalyticsAsync(string reportType, DateTime fromDate, DateTime toDate)
        {
            var request = new CustomReportRequestDto
            {
                ReportType = reportType,
                FromDate = fromDate,
                ToDate = toDate,
                Granularity = "daily"
            };

            var report = await GenerateCustomReportAsync(request);
            
            // Convert to CSV format
            var csv = new List<string>();
            csv.Add(string.Join(",", report.Headers));

            foreach (var row in report.Data)
            {
                var values = report.Headers.Select(h => row.ContainsKey(h) ? row[h]?.ToString() ?? "" : "");
                csv.Add(string.Join(",", values));
            }

            return System.Text.Encoding.UTF8.GetBytes(string.Join("\n", csv));
        }

        /// <summary>
        /// Get containers by port
        /// </summary>
        public async Task<IEnumerable<ContainerDto>> GetContainersByPortAsync(int? portId)
        {
            var query = _context.Containers.AsQueryable();

            if (portId.HasValue)
            {
                var portName = await _context.Ports.Where(p => p.PortId == portId.Value).Select(p => p.Name).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(portName))
                {
                    query = query.Where(c => c.CurrentLocation == portName);
                }
            }

            var containers = await query.ToListAsync();

            return containers.Select(c => new ContainerDto
            {
                ContainerId = c.ContainerId,
                Type = c.Type,
                Status = c.Status,
                CargoType = c.CargoType,
                Weight = c.Weight,
                CurrentLocation = c.CurrentLocation,
                Destination = c.Destination,
                Size = c.Size,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ShipId = c.ShipId
            });
        }

        /// <summary>
        /// Get berths by port
        /// </summary>
        public async Task<IEnumerable<BerthDto>> GetBerthsByPortAsync(int? portId)
        {
            var query = _context.Berths.Include(b => b.Port).AsQueryable();

            if (portId.HasValue)
            {
                query = query.Where(b => b.PortId == portId.Value);
            }

            var berths = await query.ToListAsync();

            return berths.Select(b => new BerthDto
            {
                BerthId = b.BerthId,
                Name = b.Name,
                PortId = b.PortId,
                PortName = b.Port.Name,
                Status = b.Status,
                Type = b.Type,
                Capacity = b.Capacity,
                CurrentLoad = b.CurrentLoad,
                ActiveAssignmentCount = _context.BerthAssignments.Count(ba => ba.BerthId == b.BerthId && ba.Status == "Active")
            });
        }

        // Helper methods
        private async Task<IEnumerable<RecentActivityDto>> GetRecentActivitiesAsync(int count)
        {
            var activities = new List<RecentActivityDto>();

            // Get recent berth assignments
            var recentAssignments = await _context.BerthAssignments
                .Include(ba => ba.Berth)
                .Include(ba => ba.Container)
                .OrderByDescending(ba => ba.AssignedAt)
                .Take(count / 2)
                .ToListAsync();

            foreach (var assignment in recentAssignments)
            {
                activities.Add(new RecentActivityDto
                {
                    Activity = $"Berth Assignment",
                    Description = $"Assigned to {assignment.Berth.Name}",
                    Timestamp = assignment.AssignedAt,
                    Type = "assignment",
                    EntityId = assignment.Id.ToString(),
                    EntityName = assignment.Berth.Name
                });
            }

            // Get recent container movements
            var recentMovements = await _context.ContainerMovements
                .OrderByDescending(cm => cm.MovementTimestamp)
                .Take(count / 2)
                .ToListAsync();

            foreach (var movement in recentMovements)
            {
                activities.Add(new RecentActivityDto
                {
                    Activity = movement.MovementType,
                    Description = $"Container {movement.ContainerId} {movement.MovementType.ToLower()}",
                    Timestamp = movement.MovementTimestamp,
                    Type = movement.MovementType.ToLower(),
                    EntityId = movement.ContainerId,
                    EntityName = movement.ContainerId
                });
            }

            return activities.OrderByDescending(a => a.Timestamp).Take(count);
        }

        private int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, 
                culture.DateTimeFormat.CalendarWeekRule, 
                culture.DateTimeFormat.FirstDayOfWeek);
        }

        private DateTime GetDateFromWeek(int year, int week)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            var firstWeek = jan1.AddDays(daysOffset);
            return firstWeek.AddDays((week - 1) * 7);
        }
    }
}