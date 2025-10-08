using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for event trend data
    /// </summary>
    public class EventTrendDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string EventType { get; set; }
    }
}
