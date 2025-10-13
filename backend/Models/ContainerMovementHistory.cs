using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Tracks the movement history of containers through the port system
    /// </summary>
    public class ContainerMovementHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ContainerId { get; set; }

        [StringLength(100)]
        public string? FromLocation { get; set; }

        [StringLength(100)]
        public string? ToLocation { get; set; }

        [Required]
        [StringLength(50)]
        public string MovementType { get; set; } // Load, Unload, Transfer, Storage

        [Required]
        public DateTime MovedAt { get; set; }

        [StringLength(100)]
        public string? MovedBy { get; set; } // Operator/System name

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Duration { get; set; } // Duration in hours

        [StringLength(50)]
        public string? Equipment { get; set; } // Crane, Truck, etc.

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("ContainerId")]
        public virtual Container Container { get; set; }
    }
}