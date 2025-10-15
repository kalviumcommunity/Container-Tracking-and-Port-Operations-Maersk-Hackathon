using System;

namespace Backend.DTOs
{
    public class BerthUsageChargeDto
    {
        public int Id { get; set; }
        public int BerthAssignmentId { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalHours { get; set; }
        public decimal BaseCharges { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal TotalCharges { get; set; }
        public DateTime ChargedAt { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class BerthUsageChargeCreateUpdateDto
    {
        public int BerthAssignmentId { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalHours { get; set; }
        public decimal ServiceCharges { get; set; }
        public string PaymentStatus { get; set; }
    }
}
