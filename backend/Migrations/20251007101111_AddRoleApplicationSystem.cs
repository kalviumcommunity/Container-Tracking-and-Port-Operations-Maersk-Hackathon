using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleApplicationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Containers_ContainerNumber_Unique",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "ContainerNumber",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Containers");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "ShipContainers",
                type: "character varying(11)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "Events",
                type: "character varying(11)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "Containers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CargoType",
                table: "Containers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "ContainerMovements",
                type: "character varying(11)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "BerthAssignments",
                type: "character varying(11)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RoleApplications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RequestedRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Justification = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedBy = table.Column<int>(type: "integer", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleApplications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_RoleApplications_Users_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RoleApplications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleApplications_RequestedAt",
                table: "RoleApplications",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RoleApplications_ReviewedBy",
                table: "RoleApplications",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RoleApplications_Status",
                table: "RoleApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RoleApplications_UserId",
                table: "RoleApplications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleApplications");

            migrationBuilder.DropColumn(
                name: "CargoType",
                table: "Containers");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "ShipContainers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(11)");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "Containers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11);

            migrationBuilder.AddColumn<string>(
                name: "ContainerNumber",
                table: "Containers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Containers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "ContainerMovements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(11)");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerId",
                table: "BerthAssignments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerNumber_Unique",
                table: "Containers",
                column: "ContainerNumber",
                filter: "\"ContainerNumber\" IS NOT NULL AND \"ContainerNumber\" != ''");
        }
    }
}
