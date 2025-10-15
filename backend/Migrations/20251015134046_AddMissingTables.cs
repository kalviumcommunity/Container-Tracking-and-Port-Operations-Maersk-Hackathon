using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add missing ContainerMovementHistory table
            migrationBuilder.CreateTable(
                name: "ContainerMovementHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContainerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FromLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ToLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MovementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MovedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Duration = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Equipment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            // 2. Add missing ShipRoutes table
            migrationBuilder.CreateTable(
                name: "ShipRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipId = table.Column<int>(type: "integer", nullable: false),
                    RouteNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OriginPortId = table.Column<int>(type: "integer", nullable: false),
                    DestinationPortId = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDeparture = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledArrival = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualDeparture = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualArrival = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RouteStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WeatherDelay = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    PortDelay = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    FuelConsumption = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipRoutes_Ports_DestinationPortId",
                        column: x => x.DestinationPortId,
                        principalTable: "Ports",
                        principalColumn: "PortId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 3. Add missing BerthUsageCharges table
            migrationBuilder.CreateTable(
                name: "BerthUsageCharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BerthAssignmentId = table.Column<int>(type: "integer", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalHours = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    BaseCharges = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ServiceCharges = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    TotalCharges = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ChargedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            // 4. Add missing ContainerStorageFees table
            migrationBuilder.CreateTable(
                name: "ContainerStorageFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContainerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PortId = table.Column<int>(type: "integer", nullable: false),
                    StorageStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StorageEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DailyStorageRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalFees = table.Column<decimal>(type: "decimal(15,2)", nullable: false, defaultValue: 0m),
                    FeeStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Calculating"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            // Add indexes for performance
            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovementHistory_ContainerId",
                table: "ContainerMovementHistory",
                column: "ContainerId");

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
                name: "IX_BerthUsageCharges_BerthAssignmentId",
                table: "BerthUsageCharges",
                column: "BerthAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStorageFees_ContainerId",
                table: "ContainerStorageFees",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerStorageFees_PortId",
                table: "ContainerStorageFees",
                column: "PortId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ContainerStorageFees");
            migrationBuilder.DropTable(name: "BerthUsageCharges");
            migrationBuilder.DropTable(name: "ShipRoutes");
            migrationBuilder.DropTable(name: "ContainerMovementHistory");
        }
    }
}
