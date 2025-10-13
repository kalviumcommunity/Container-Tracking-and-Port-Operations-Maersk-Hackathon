using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Tracks container storage fees and duration at ports
    /// </summary>
    public class ContainerStorageFee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ContainerId { get; set; }

        [Required]
        public int PortId { get; set; }

        [Required]
        public DateTime StorageStartDate { get; set; }

        public DateTime? StorageEndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DailyStorageRate { get; set; }

        public int TotalDays { get; set; } = 0;

        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalFees { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public string FeeStatus { get; set; } = "Calculating"; // Calculating, Finalized, Paid

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ContainerId")]
        public virtual Container Container { get; set; }

        [ForeignKey("PortId")]
        public virtual Port Port { get; set; }
    }
}