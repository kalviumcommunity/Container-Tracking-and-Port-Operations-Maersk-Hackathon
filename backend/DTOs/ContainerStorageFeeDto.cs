using System;

namespace Backend.DTOs
{
    public class ContainerStorageFeeDto
    {
        public int Id { get; set; }
        public string ContainerId { get; set; }
        public int PortId { get; set; }
        public DateTime StorageStartDate { get; set; }
        public DateTime? StorageEndDate { get; set; }
        public decimal DailyStorageRate { get; set; }
        public int TotalDays { get; set; }
        public decimal TotalFees { get; set; }
        public string FeeStatus { get; set; }
    }

    public class ContainerStorageFeeCreateUpdateDto
    {
        public string ContainerId { get; set; }
        public int PortId { get; set; }
        public DateTime StorageStartDate { get; set; }
        public DateTime? StorageEndDate { get; set; }
        public decimal DailyStorageRate { get; set; }
        public string FeeStatus { get; set; }
    }
}
