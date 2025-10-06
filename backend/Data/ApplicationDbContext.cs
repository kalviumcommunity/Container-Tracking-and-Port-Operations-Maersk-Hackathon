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
        /// Users in the database
        /// </summary>
        public DbSet<User> Users { get; set; }
        
        /// <summary>
        /// Roles in the database
        /// </summary>
        public DbSet<Role> Roles { get; set; }
        
        /// <summary>
        /// User role assignments in the database
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }
        
        /// <summary>
        /// Permissions in the database
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }
        
        /// <summary>
        /// Role permission assignments in the database
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; }
        
        /// <summary>
        /// Events in the database for real-time streaming
        /// </summary>
        public DbSet<Event> Events { get; set; }
        
        /// <summary>
        /// Container movements and tracking records
        /// </summary>
        public DbSet<ContainerMovement> ContainerMovements { get; set; }
        
        /// <summary>
        /// Analytics and metrics data for dashboard reporting
        /// </summary>
        public DbSet<Analytics> Analytics { get; set; }

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
                .IsRequired(false)  // BerthAssignment may be for ship only
                .OnDelete(DeleteBehavior.Restrict);
                
            // Configure Ship-BerthAssignment relationship
            modelBuilder.Entity<BerthAssignment>()
                .HasOne(ba => ba.Ship)
                .WithMany(s => s.BerthAssignments)
                .HasForeignKey(ba => ba.ShipId)
                .IsRequired(false)  // BerthAssignment may be for container only
                .OnDelete(DeleteBehavior.Restrict);
                
            // Configure Berth-BerthAssignment relationship
            modelBuilder.Entity<BerthAssignment>()
                .HasOne(ba => ba.Berth)
                .WithMany(b => b.BerthAssignments)
                .HasForeignKey(ba => ba.BerthId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
                
            // Configure User-BerthAssignment relationship for CreatedBy
            modelBuilder.Entity<BerthAssignment>()
                .HasOne(ba => ba.CreatedBy)
                .WithMany()
                .HasForeignKey(ba => ba.CreatedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Configure Ship-Port relationship for CurrentPort
            modelBuilder.Entity<Ship>()
                .HasOne(s => s.CurrentPort)
                .WithMany(p => p.DockedShips)
                .HasForeignKey(s => s.CurrentPortId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
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
            // Only create unique index on ContainerNumber if it's not null and not empty
            modelBuilder.Entity<Container>().HasIndex(c => c.ContainerNumber)
                .HasDatabaseName("IX_Containers_ContainerNumber_Unique")
                .HasFilter("\"ContainerNumber\" IS NOT NULL AND \"ContainerNumber\" != ''");
            modelBuilder.Entity<Ship>().HasIndex(s => s.Name);
            // Only create unique index on ImoNumber if it's not null
            modelBuilder.Entity<Ship>().HasIndex(s => s.ImoNumber)
                .HasDatabaseName("IX_Ships_ImoNumber_Unique")
                .HasFilter("\"ImoNumber\" IS NOT NULL AND \"ImoNumber\" != ''");
            modelBuilder.Entity<Berth>().HasIndex(b => new { b.PortId, b.Identifier })
                .IsUnique()
                .HasDatabaseName("IX_Berths_PortId_Identifier_Unique")
                .HasFilter("\"Identifier\" IS NOT NULL AND \"Identifier\" != ''");
            modelBuilder.Entity<BerthAssignment>().HasIndex(ba => new { ba.BerthId, ba.AssignedAt });
            
            // Configure Event relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Container)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.ContainerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Ship)
                .WithMany()
                .HasForeignKey(e => e.ShipId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Berth)
                .WithMany(b => b.Events)
                .HasForeignKey(e => e.BerthId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Port)
                .WithMany()
                .HasForeignKey(e => e.PortId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Event>()
                .HasOne(e => e.AssignedToUser)
                .WithMany()
                .HasForeignKey(e => e.AssignedToUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Event>()
                .HasOne(e => e.AcknowledgedByUser)
                .WithMany()
                .HasForeignKey(e => e.AcknowledgedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Configure ContainerMovement relationships
            modelBuilder.Entity<ContainerMovement>()
                .HasOne(cm => cm.Container)
                .WithMany(c => c.Movements)
                .HasForeignKey(cm => cm.ContainerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<ContainerMovement>()
                .HasOne(cm => cm.Ship)
                .WithMany()
                .HasForeignKey(cm => cm.ShipId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<ContainerMovement>()
                .HasOne(cm => cm.Berth)
                .WithMany(b => b.ContainerMovements)
                .HasForeignKey(cm => cm.BerthId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<ContainerMovement>()
                .HasOne(cm => cm.Port)
                .WithMany()
                .HasForeignKey(cm => cm.PortId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<ContainerMovement>()
                .HasOne(cm => cm.RecordedByUser)
                .WithMany()
                .HasForeignKey(cm => cm.RecordedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Configure Analytics relationships
            modelBuilder.Entity<Analytics>()
                .HasOne(a => a.Port)
                .WithMany()
                .HasForeignKey(a => a.PortId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Analytics>()
                .HasOne(a => a.Berth)
                .WithMany(b => b.Analytics)
                .HasForeignKey(a => a.BerthId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Analytics>()
                .HasOne(a => a.Ship)
                .WithMany()
                .HasForeignKey(a => a.ShipId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Add indexes for new models
            modelBuilder.Entity<Event>().HasIndex(e => new { e.EventTimestamp, e.Priority });
            modelBuilder.Entity<Event>().HasIndex(e => e.Status);
            modelBuilder.Entity<ContainerMovement>().HasIndex(cm => new { cm.ContainerId, cm.MovementTimestamp });
            modelBuilder.Entity<Analytics>().HasIndex(a => new { a.MetricType, a.MetricTimestamp });
            modelBuilder.Entity<Analytics>().HasIndex(a => new { a.PortId, a.Period, a.MetricTimestamp });

            // Authentication and Authorization model configurations
            
            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
                
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
            // Configure User-Port relationship (optional port assignment)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Port)
                .WithMany(p => p.AssignedUsers)
                .HasForeignKey(u => u.PortId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Role entity
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Configure Permission entity
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            // Configure UserRole many-to-many relationship
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
                
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Configure UserRole self-reference for AssignedByUser
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.AssignedByUser)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure RolePermission many-to-many relationship
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });
                
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Configure RolePermission self-reference for GrantedByUser
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.GrantedByUser)
                .WithMany()
                .HasForeignKey(rp => rp.GrantedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}