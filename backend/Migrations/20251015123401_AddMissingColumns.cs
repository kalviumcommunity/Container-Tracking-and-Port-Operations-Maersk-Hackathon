using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing 'Severity' column to Events table (appears in your models)
            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Events",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Medium");

            // Add missing 'Category' column to Events table if not exists
            // (Check your Event model - this should already exist based on your migrations)

            // Add missing 'Source' column to Events table if not exists  
            // (Check your Event model - this should already exist based on your migrations)

            // Add missing performance tracking columns to Berths
            migrationBuilder.AddColumn<decimal>(
                name: "AverageOccupancyTime",
                table: "Berths",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalShipsServed",
                table: "Berths",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalContainersProcessed",
                table: "Berths",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Berths",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextMaintenanceDate",
                table: "Berths",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EfficiencyRating",
                table: "Berths",
                type: "decimal(3,2)",
                nullable: true);

            // Add missing performance columns to Ports
            migrationBuilder.AddColumn<decimal>(
                name: "AverageProcessingTime",
                table: "Ports",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OperationalEfficiencyRating",
                table: "Ports",
                type: "decimal(3,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeatherConditions",
                table: "Ports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Severity", table: "Events");
            migrationBuilder.DropColumn(name: "AverageOccupancyTime", table: "Berths");
            migrationBuilder.DropColumn(name: "TotalShipsServed", table: "Berths");
            migrationBuilder.DropColumn(name: "TotalContainersProcessed", table: "Berths");
            migrationBuilder.DropColumn(name: "LastMaintenanceDate", table: "Berths");
            migrationBuilder.DropColumn(name: "NextMaintenanceDate", table: "Berths");
            migrationBuilder.DropColumn(name: "EfficiencyRating", table: "Berths");
            migrationBuilder.DropColumn(name: "AverageProcessingTime", table: "Ports");
            migrationBuilder.DropColumn(name: "OperationalEfficiencyRating", table: "Ports");
            migrationBuilder.DropColumn(name: "WeatherConditions", table: "Ports");
        }
    }
}
