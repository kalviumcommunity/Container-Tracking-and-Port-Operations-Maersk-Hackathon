namespace Backend.DTOs
{
    public class ContainerMovementDto
    {
        public int Id { get; set; }
        public string ContainerId { get; set; }
        public string MovementType { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime MovementTimestamp { get; set; }
        public string Status { get; set; }
        public string Coordinates { get; set; }
        public string Notes { get; set; }
        public int? PortId { get; set; }
        public string PortName { get; set; }
        public int? BerthId { get; set; }
        public string BerthName { get; set; }
        public int? ShipId { get; set; }
        public string ShipName { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
        public DateTime? ActualCompletion { get; set; }
        public int RecordedByUserId { get; set; }
        public string RecordedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ContainerMovementCreateDto
    {
        public string ContainerId { get; set; }
        public string MovementType { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime? MovementTimestamp { get; set; }
        public string Status { get; set; }
        public string Coordinates { get; set; }
        public string Notes { get; set; }
        public int? PortId { get; set; }
        public int? BerthId { get; set; }
        public int? ShipId { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
    }

    public class ContainerMovementUpdateDto
    {
        public string Status { get; set; }
        public string Coordinates { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public string Notes { get; set; }
        public DateTime? ActualCompletion { get; set; }
    }

    public class ContainerTrackingDto
    {
        public string ContainerId { get; set; }
        public string CurrentLocation { get; set; }
        public string Destination { get; set; }
        public string Status { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public string ShipName { get; set; }
        public List<ContainerMovementDto> Movements { get; set; } = new();
    }
}
