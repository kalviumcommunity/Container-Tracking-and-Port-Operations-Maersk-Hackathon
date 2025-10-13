using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Tracks financial charges for berth usage including hourly rates and services
    /// </summary>
    public class BerthUsageCharge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BerthAssignmentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal HourlyRate { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalHours { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal BaseCharges { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal ServiceCharges { get; set; } = 0; // Crane, electricity, etc.

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalCharges { get; set; }

        public DateTime ChargedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Overdue

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("BerthAssignmentId")]
        public virtual BerthAssignment BerthAssignment { get; set; }
    }
}