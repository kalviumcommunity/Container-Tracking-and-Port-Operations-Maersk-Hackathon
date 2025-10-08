using System;
using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for event statistics
    /// </summary>
    public class EventStatsDto
    {
        public int TotalEvents { get; set; }
        public int UnreadEvents { get; set; }
        public int TodayEvents { get; set; }
        public int WeekEvents { get; set; }
        public Dictionary<string, int> EventsBySeverity { get; set; } = new();
        public Dictionary<string, int> EventsByType { get; set; } = new();
        public Dictionary<string, int> EventsBySource { get; set; } = new();
        public List<EventTrendDto> RecentTrends { get; set; } = new();
    }
}
