using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaToMatchDiagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BerthAssignments_Ships_ShipId",
                table: "BerthAssignments");

            migrationBuilder.DropTable(
                name: "BerthAssignmentContainers");

            migrationBuilder.DropIndex(
                name: "IX_Ships_IMONumber",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_ShipId_BerthId",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "IMONumber",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "OperationType",
                table: "ShipContainers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShipContainers");

            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "BerthAssignments");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "BerthAssignments");

            migrationBuilder.RenameColumn(
                name: "OperationTime",
                table: "ShipContainers",
                newName: "LoadedAt");

            migrationBuilder.RenameColumn(
                name: "ShipContainerId",
                table: "ShipContainers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "MaxCapacity",
                table: "Berths",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "BerthNumber",
                table: "Berths",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Berths_PortId_BerthNumber",
                table: "Berths",
                newName: "IX_Berths_PortId_Name");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "BerthAssignments",
                newName: "ContainerId");

            migrationBuilder.RenameColumn(
                name: "ExpectedDepartureTime",
                table: "BerthAssignments",
                newName: "AssignedAt");

            migrationBuilder.RenameColumn(
                name: "ActualDepartureTime",
                table: "BerthAssignments",
                newName: "ReleasedAt");

            migrationBuilder.RenameColumn(
                name: "BerthAssignmentId",
                table: "BerthAssignments",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_Name",
                table: "Ships",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_ContainerId_BerthId",
                table: "BerthAssignments",
                columns: new[] { "ContainerId", "BerthId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BerthAssignments_Containers_ContainerId",
                table: "BerthAssignments",
                column: "ContainerId",
                principalTable: "Containers",
                principalColumn: "ContainerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BerthAssignments_Containers_ContainerId",
                table: "BerthAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Ships_Name",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_BerthAssignments_ContainerId_BerthId",
                table: "BerthAssignments");

            migrationBuilder.RenameColumn(
                name: "LoadedAt",
                table: "ShipContainers",
                newName: "OperationTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShipContainers",
                newName: "ShipContainerId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Berths",
                newName: "BerthNumber");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Berths",
                newName: "MaxCapacity");

            migrationBuilder.RenameIndex(
                name: "IX_Berths_PortId_Name",
                table: "Berths",
                newName: "IX_Berths_PortId_BerthNumber");

            migrationBuilder.RenameColumn(
                name: "ReleasedAt",
                table: "BerthAssignments",
                newName: "ActualDepartureTime");

            migrationBuilder.RenameColumn(
                name: "ContainerId",
                table: "BerthAssignments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "BerthAssignments",
                newName: "ExpectedDepartureTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BerthAssignments",
                newName: "BerthAssignmentId");

            migrationBuilder.AddColumn<string>(
                name: "IMONumber",
                table: "Ships",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "Ships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OperationType",
                table: "ShipContainers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ShipContainers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalTime",
                table: "BerthAssignments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "BerthAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BerthAssignmentContainers",
                columns: table => new
                {
                    BerthAssignmentContainerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BerthAssignmentId = table.Column<int>(type: "integer", nullable: false),
                    ContainerId = table.Column<string>(type: "text", nullable: false),
                    OperationType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BerthAssignmentContainers", x => x.BerthAssignmentContainerId);
                    table.ForeignKey(
                        name: "FK_BerthAssignmentContainers_BerthAssignments_BerthAssignmentId",
                        column: x => x.BerthAssignmentId,
                        principalTable: "BerthAssignments",
                        principalColumn: "BerthAssignmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BerthAssignmentContainers_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ships_IMONumber",
                table: "Ships",
                column: "IMONumber");

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignments_ShipId_BerthId",
                table: "BerthAssignments",
                columns: new[] { "ShipId", "BerthId" });

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignmentContainers_BerthAssignmentId",
                table: "BerthAssignmentContainers",
                column: "BerthAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BerthAssignmentContainers_ContainerId",
                table: "BerthAssignmentContainers",
                column: "ContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BerthAssignments_Ships_ShipId",
                table: "BerthAssignments",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "ShipId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
