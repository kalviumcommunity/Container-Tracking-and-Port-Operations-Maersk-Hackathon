using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    /// <summary>
    /// Database context for the Container Tracking and Port Operations application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor for ApplicationDbContext
        /// </summary>
        /// <param name="options">DbContext options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Containers in the database
        /// </summary>
        public DbSet<Container> Containers { get; set; }
        
        /// <summary>
        /// Ports in the database
        /// </summary>
        public DbSet<Port> Ports { get; set; }
        
        /// <summary>
        /// Berths in the database
        /// </summary>
        public DbSet<Berth> Berths { get; set; }
        
        /// <summary>
        /// Ships in the database
        /// </summary>
        public DbSet<Ship> Ships { get; set; }
        
        /// <summary>
        /// Berth assignments in the database
        /// </summary>
        public DbSet<BerthAssignment> BerthAssignments { get; set; }
        
        /// <summary>
        /// Ship container operations in the database
        /// </summary>
        public DbSet<ShipContainer> ShipContainers { get; set; }

        /// <summary>
        /// Configure entity relationships and constraints
        /// </summary>
        /// <param name="modelBuilder">Model builder for configuring the database</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Container relationships
            modelBuilder.Entity<Container>()
                .HasOne(c => c.Ship)
                .WithMany(s => s.Containers)
                .HasForeignKey(c => c.ShipId)
                .IsRequired(false);  // Container may not be on a ship

            // Configure Port-Berth relationship
            modelBuilder.Entity<Berth>()
                .HasOne(b => b.Port)
                .WithMany(p => p.Berths)
                .HasForeignKey(b => b.PortId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a port is deleted, delete all its berths
            
            // Configure Container-BerthAssignment relationship
            modelBuilder.Entity<BerthAssignment>()
                .HasOne(ba => ba.Container)
                .WithMany(c => c.BerthAssignments)
                .HasForeignKey(ba => ba.ContainerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete berth assignments when a container is deleted
                
            // Configure Berth-BerthAssignment relationship
            modelBuilder.Entity<BerthAssignment>()
                .HasOne(ba => ba.Berth)
                .WithMany(b => b.BerthAssignments)
                .HasForeignKey(ba => ba.BerthId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete berth assignments when a berth is deleted
                
            // Configure Container-ShipContainer relationship
            modelBuilder.Entity<ShipContainer>()
                .HasOne(sc => sc.Container)
                .WithMany(c => c.ShipContainers)
                .HasForeignKey(sc => sc.ContainerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete operation records when a container is deleted
                
            // Configure Ship-ShipContainer relationship
            modelBuilder.Entity<ShipContainer>()
                .HasOne(sc => sc.Ship)
                .WithMany(s => s.ShipContainers)
                .HasForeignKey(sc => sc.ShipId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete operation records when a ship is deleted

            // Add indexes for better query performance
            modelBuilder.Entity<Container>().HasIndex(c => c.ContainerId);
            modelBuilder.Entity<Ship>().HasIndex(s => s.Name);
            modelBuilder.Entity<Berth>().HasIndex(b => new { b.PortId, b.Name });
            modelBuilder.Entity<BerthAssignment>().HasIndex(ba => new { ba.ContainerId, ba.BerthId });
        }
    }
}