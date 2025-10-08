namespace Backend.DTOs
{
    /// <summary>
    /// Result of data seeding operations
    /// </summary>
    public class SeedingResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsProduction { get; set; }
        public DatabaseStatsDto? Statistics { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// Database statistics for seeding and monitoring
    /// </summary>
    public class DatabaseStatsDto
    {
        public int Permissions { get; set; }
        public int Roles { get; set; }
        public int Users { get; set; }
        public int Ports { get; set; }
        public int Berths { get; set; }
        public int Ships { get; set; }
        public int Containers { get; set; }
        public int BerthAssignments { get; set; }
        public int ShipContainers { get; set; }
        public int ContainerMovements { get; set; }
        public int Events { get; set; }
        public int Analytics { get; set; }
        public int RoleApplications { get; set; }
        
        public int TotalRecords => Permissions + Roles + Users + Ports + Berths + Ships + 
                                  Containers + BerthAssignments + ShipContainers + 
                                  ContainerMovements + Events + Analytics + RoleApplications;
    }
}