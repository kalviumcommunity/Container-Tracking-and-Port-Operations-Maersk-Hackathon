using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FinalEnhancedSchemaWithConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Berths_PortId_Name",
                table: "Berths");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_BerthId",
                table: "BerthAssignments");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_ContainerId_BerthId",
                table: "BerthAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ships",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "Beam",
                table: "Ships",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Ships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Coordinates",
                table: "Ships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Ships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CurrentPortId",
                table: "Ships",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Draft",
                table: "Ships",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedArrival",
                table: "Ships",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flag",
                table: "Ships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "GrossTonnage",
                table: "Ships",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Heading",
                table: "Ships",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImoNumber",
                table: "Ships",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "Ships",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextPort",
                table: "Ships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Speed",
                table: "Ships",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Ships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Ships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "YearBuilt",
                table: "Ships",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Ports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Ports",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Coordinates",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Ports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Ports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CurrentContainerCount",
                table: "Ports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentShipCount",
                table: "Ports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxShipCapacity",
                table: "Ports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OperatingHours",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Ports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CargoDescription",
                table: "Containers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Containers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContainerNumber",
                table: "Containers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Coordinates",
                table: "Containers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Containers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedArrival",
                table: "Containers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxWeight",
                table: "Containers",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Containers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "Containers",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "Containers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Berths",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AvailableServices",
                table: "Berths",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CraneCount",
                table: "Berths",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Berths",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CurrentLoad",
                table: "Berths",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Berths",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Berths",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDraft",
                table: "Berths",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxShipLength",
                table: "Berths",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Berths",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Berths",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Berths",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Berths",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "BerthAssignments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualArrival",
                table: "BerthAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualDeparture",
                table: "BerthAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualProcessingTime",
                table: "BerthAssignments",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignmentType",
                table: "BerthAssignments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ContainerCount",
                table: "BerthAssignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "BerthAssignments",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BerthAssignments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "BerthAssignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedProcessingTime",
                table: "BerthAssignments",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "BerthAssignments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "BerthAssignments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledArrival",
                table: "BerthAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDeparture",
                table: "BerthAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "BerthAssignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BerthAssignments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BerthAssignments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Analytics",
                columns: table => new
                {
                    AnalyticsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetricType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Period = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TargetValue = table.Column<decimal>(type: "numeric", nullable: true),
                    PreviousPeriodValue = table.Column<decimal>(type: "numeric", nullable: true),
                    PercentageChange = table.Column<decimal>(type: "numeric", nullable: true),
                    PortId = table.Column<int>(type: "integer", nullable: true),
                    BerthId = table.Column<int>(type: "integer", nullable: true),
                    ShipId = table.Column<int>(type: "integer", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    Trend = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MetricTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analytics", x => x.AnalyticsId);
                    table.ForeignKey(
                        name: "FK_Analytics_Berths_BerthId",
                        column: x => x.BerthId,
                        principalTable: "Berths",
                        principalColumn: "BerthId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Analytics_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "PortId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Analytics_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ContainerMovements",
                columns: table => new
                {
                    MovementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContainerId = table.Column<string>(type: "text", nullable: false),
                    MovementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FromLocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ToLocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Coordinates = table.Column<string>(type: "text", nullable: false),
                    EstimatedCompletion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualCompletion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    DistanceKm = table.Column<decimal>(type: "numeric", nullable: true),
                    ShipId = table.Column<int>(type: "integer", nullable: true),
                    BerthId = table.Column<int>(type: "integer", nullable: true),
                    PortId = table.Column<int>(type: "integer", nullable: true),
                    Temperature = table.Column<decimal>(type: "numeric", nullable: true),
                    Humidity = table.Column<decimal>(type: "numeric", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    RecordedByUserId = table.Column<int>(type: "integer", nullable: true),
                    MovementTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerMovements", x => x.MovementId);
                    table.ForeignKey(
                        name: "FK_ContainerMovements_Berths_BerthId",
                        column: x => x.BerthId,
                        principalTable: "Berths",
                        principalColumn: "BerthId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContainerMovements_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContainerMovements_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "PortId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContainerMovements_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContainerMovements_Users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContainerId = table.Column<string>(type: "text", nullable: true),
                    ShipId = table.Column<int>(type: "integer", nullable: true),
                    BerthId = table.Column<int>(type: "integer", nullable: true),
                    PortId = table.Column<int>(type: "integer", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: true),
                    AcknowledgedByUserId = table.Column<int>(type: "integer", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EventData = table.Column<string>(type: "jsonb", nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    Coordinates = table.Column<string>(type: "text", nullable: false),
                    RequiresAction = table.Column<bool>(type: "boolean", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Berths_BerthId",
                        column: x => x.BerthId,
                        principalTable: "Berths",
                        principalColumn: "BerthId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Events_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Events_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "PortId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Events_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Events_Users_AcknowledgedByUserId",
                        column: x => x.AcknowledgedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Events_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ships_CurrentPortId",
                table: "Ships",
                column: "CurrentPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_ImoNumber_Unique",
                table: "Ships",
                column: "ImoNumber",
                filter: "\"ImoNumber\" IS NOT NULL AND \"ImoNumber\" != ''");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerNumber_Unique",
                table: "Containers",
                column: "ContainerNumber",
                filter: "\"ContainerNumber\" IS NOT NULL AND \"ContainerNumber\" != ''");

            migrationBuilder.CreateIndex(
                name: "IX_Berths_PortId_Identifier_Unique",
                table: "Berths",
                columns: new[] { "PortId", "Identifier" },
                unique: true,
                filter: "\"Identifier\" IS NOT NULL AND \"Identifier\" != ''");

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_BerthId_AssignedAt",
                table: "BerthAssignments",
                columns: new[] { "BerthId", "AssignedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_ContainerId",
                table: "BerthAssignments",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_CreatedByUserId",
                table: "BerthAssignments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_ShipId",
                table: "BerthAssignments",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_BerthId",
                table: "Analytics",
                column: "BerthId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_MetricType_MetricTimestamp",
                table: "Analytics",
                columns: new[] { "MetricType", "MetricTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_PortId_Period_MetricTimestamp",
                table: "Analytics",
                columns: new[] { "PortId", "Period", "MetricTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ShipId",
                table: "Analytics",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovements_BerthId",
                table: "ContainerMovements",
                column: "BerthId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovements_ContainerId_MovementTimestamp",
                table: "ContainerMovements",
                columns: new[] { "ContainerId", "MovementTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovements_PortId",
                table: "ContainerMovements",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovements_RecordedByUserId",
                table: "ContainerMovements",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerMovements_ShipId",
                table: "ContainerMovements",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AcknowledgedByUserId",
                table: "Events",
                column: "AcknowledgedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AssignedToUserId",
                table: "Events",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_BerthId",
                table: "Events",
                column: "BerthId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ContainerId",
                table: "Events",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTimestamp_Priority",
                table: "Events",
                columns: new[] { "EventTimestamp", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_PortId",
                table: "Events",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ShipId",
                table: "Events",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Status",
                table: "Events",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_BerthAssignments_Ships_ShipId",
                table: "BerthAssignments",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "ShipId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BerthAssignments_Users_CreatedByUserId",
                table: "BerthAssignments",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_Ports_CurrentPortId",
                table: "Ships",
                column: "CurrentPortId",
                principalTable: "Ports",
                principalColumn: "PortId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BerthAssignments_Ships_ShipId",
                table: "BerthAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_BerthAssignments_Users_CreatedByUserId",
                table: "BerthAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_Ports_CurrentPortId",
                table: "Ships");

            migrationBuilder.DropTable(
                name: "Analytics");

            migrationBuilder.DropTable(
                name: "ContainerMovements");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Ships_CurrentPortId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_ImoNumber_Unique",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Containers_ContainerNumber_Unique",
                table: "Containers");

            migrationBuilder.DropIndex(
                name: "IX_Berths_PortId_Identifier_Unique",
                table: "Berths");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_BerthId_AssignedAt",
                table: "BerthAssignments");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_ContainerId",
                table: "BerthAssignments");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_CreatedByUserId",
                table: "BerthAssignments");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_ShipId",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "Beam",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "CurrentPortId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Draft",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "EstimatedArrival",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "GrossTonnage",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Heading",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "ImoNumber",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "NextPort",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "YearBuilt",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "ContactInfo",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "CurrentContainerCount",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "CurrentShipCount",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "MaxShipCapacity",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "OperatingHours",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "CargoDescription",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "ContainerNumber",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "EstimatedArrival",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "MaxWeight",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "AvailableServices",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "CraneCount",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "CurrentLoad",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "MaxDraft",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "MaxShipLength",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Berths");

            migrationBuilder.DropColumn(
                name: "ActualArrival",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "ActualDeparture",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "ActualProcessingTime",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "AssignmentType",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "ContainerCount",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "EstimatedProcessingTime",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "ScheduledArrival",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "ScheduledDeparture",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BerthAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ships",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ports",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Ports",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Berths",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "BerthAssignments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Berths_PortId_Name",
                table: "Berths",
                columns: new[] { "PortId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_BerthId",
                table: "BerthAssignments",
                column: "BerthId");

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_ContainerId_BerthId",
                table: "BerthAssignments",
                columns: new[] { "ContainerId", "BerthId" });
        }
    }
}
