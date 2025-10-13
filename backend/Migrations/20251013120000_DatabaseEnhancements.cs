using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Performance Tracking Tables
            
            // 1. Container Movement History Table
            migrationBuilder.CreateTable(
                name: "ContainerMovementHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContainerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ToLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MovementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MovedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Duration = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "ntext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerMovementHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerMovementHistory_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 2. Ship Routes Table
            migrationBuilder.CreateTable(
                name: "ShipRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipId = table.Column<int>(type: "int", nullable: false),
                    RouteNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OriginPortId = table.Column<int>(type: "int", nullable: false),
                    DestinationPortId = table.Column<int>(type: "int", nullable: false),
                    ScheduledDeparture = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledArrival = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDeparture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RouteStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Scheduled"),
                    WeatherDelay = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    PortDelay = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    FuelConsumption = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipRoutes_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipRoutes_Ports_OriginPortId",
                        column: x => x.OriginPortId,
                        principalTable: "Ports",
                        principalColumn: "PortId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipRoutes_Ports_DestinationPortId",
                        column: x => x.DestinationPortId,
                        principalTable: "Ports",
                        principalColumn: "PortId",
                        onDelete: ReferentialAction.Restrict);
                });

            // 3. Berth Usage Charges Table
            migrationBuilder.CreateTable(
                name: "BerthUsageCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BerthAssignmentId = table.Column<int>(type: "int", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalHours = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    BaseCharges = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ServiceCharges = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    TotalCharges = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ChargedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BerthUsageCharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BerthUsageCharges_BerthAssignments_BerthAssignmentId",
                        column: x => x.BerthAssignmentId,
                        principalTable: "BerthAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 4. Container Storage Fees Table
            migrationBuilder.CreateTable(
                name: "ContainerStorageFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContainerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PortId = table.Column<int>(type: "int", nullable: false),
                    StorageStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StorageEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DailyStorageRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalFees = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    FeeStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Calculating"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerStorageFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerStorageFees_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContainerStorageFees_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "PortId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Add new fields to existing tables
            
            // Enhance Ports table with operational metrics
            migrationBuilder.AddColumn<decimal>(
                name: "AverageProcessingTime",
                table: "Ports",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PeakHoursStart",
                table: "Ports",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PeakHoursEnd",
                table: "Ports",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeatherConditions",
                table: "Ports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OperationalEfficiencyRating",
                table: "Ports",
                type: "decimal(3,2)",
                nullable: true);

            // Enhance Berths table with performance analytics
            migrationBuilder.AddColumn<decimal>(
                name: "AverageOccupancyTime",
                table: "Berths",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalShipsServed",
                table: "Berths",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalContainersProcessed",
                table: "Berths",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Berths",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextMaintenanceDate",
                table: "Berths",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EfficiencyRating",
                table: "Berths",
                type: "decimal(3,2)",
                nullable: true);

            // Create indexes for performance
            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovementHistory_ContainerId",
                table: "ContainerMovementHistory",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovementHistory_MovedAt",
                table: "ContainerMovementHistory",
                column: "MovedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ShipRoutes_ShipId",
                table: "ShipRoutes",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipRoutes_OriginPortId",
                table: "ShipRoutes",
                column: "OriginPortId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipRoutes_DestinationPortId",
                table: "ShipRoutes",
                column: "DestinationPortId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipRoutes_RouteStatus",
                table: "ShipRoutes",
                column: "RouteStatus");

            migrationBuilder.CreateIndex(
                name: "IX_BerthUsageCharges_BerthAssignmentId",
                table: "BerthUsageCharges",
                column: "BerthAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BerthUsageCharges_PaymentStatus",
                table: "BerthUsageCharges",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStorageFees_ContainerId",
                table: "ContainerStorageFees",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStorageFees_PortId",
                table: "ContainerStorageFees",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStorageFees_FeeStatus",
                table: "ContainerStorageFees",
                column: "FeeStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_ContainerMovementHistory_ContainerId",
                table: "ContainerMovementHistory");

            migrationBuilder.DropIndex(
                name: "IX_ContainerMovementHistory_MovedAt",
                table: "ContainerMovementHistory");

            migrationBuilder.DropIndex(
                name: "IX_ShipRoutes_ShipId",
                table: "ShipRoutes");

            migrationBuilder.DropIndex(
                name: "IX_ShipRoutes_OriginPortId",
                table: "ShipRoutes");

            migrationBuilder.DropIndex(
                name: "IX_ShipRoutes_DestinationPortId",
                table: "ShipRoutes");

            migrationBuilder.DropIndex(
                name: "IX_ShipRoutes_RouteStatus",
                table: "ShipRoutes");

            migrationBuilder.DropIndex(
                name: "IX_BerthUsageCharges_BerthAssignmentId",
                table: "BerthUsageCharges");

            migrationBuilder.DropIndex(
                name: "IX_BerthUsageCharges_PaymentStatus",
                table: "BerthUsageCharges");

            migrationBuilder.DropIndex(
                name: "IX_ContainerStorageFees_ContainerId",
                table: "ContainerStorageFees");

            migrationBuilder.DropIndex(
                name: "IX_ContainerStorageFees_PortId",
                table: "ContainerStorageFees");

            migrationBuilder.DropIndex(
                name: "IX_ContainerStorageFees_FeeStatus",
                table: "ContainerStorageFees");

            // Drop tables
            migrationBuilder.DropTable(
                name: "ContainerStorageFees");

            migrationBuilder.DropTable(
                name: "BerthUsageCharges");

            migrationBuilder.DropTable(
                name: "ShipRoutes");

            migrationBuilder.DropTable(
                name: "ContainerMovementHistory");

            // Remove columns from existing tables
            migrationBuilder.DropColumn(
                name: "AverageProcessingTime",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "PeakHoursStart",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "PeakHoursEnd",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "WeatherConditions",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "OperationalEfficiencyRating",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "AverageOccupancyTime",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "TotalShipsServed",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "TotalContainersProcessed",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "LastMaintenanceDate",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "NextMaintenanceDate",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "EfficiencyRating",
                table: "Berths");
        }
    }
}