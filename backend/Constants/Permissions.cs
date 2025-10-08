namespace Backend.Constants
{
    /// <summary>
    /// Constants for application permissions
    /// </summary>
    public static class Permissions
    {
        // Global Permissions
        public const string GlobalPortAccess = "GlobalPortAccess";
        public const string ManageAllPorts = "ManageAllPorts";
        public const string ManageUsers = "ManageUsers";
        public const string ManageRoles = "ManageRoles";
        public const string SeedData = "SeedData";

        // Container Permissions
        public const string ViewContainers = "ViewContainers";
        public const string ManageContainers = "ManageContainers";
        public const string TrackContainers = "TrackContainers";

        // Ship Permissions
        public const string ViewShips = "ViewShips";
        public const string ManageShips = "ManageShips";
        public const string ScheduleShips = "ScheduleShips";

        // Port Permissions
        public const string ViewPortDetails = "ViewPortDetails";
        public const string ManagePortDetails = "ManagePortDetails";
        public const string ViewPortReports = "ViewPortReports";

        // Berth Permissions
        public const string ViewBerths = "ViewBerths";
        public const string ManageBerths = "ManageBerths";
        public const string AllocateBerths = "AllocateBerths";
        public const string ViewBerthAssignments = "ViewBerthAssignments";
        public const string ManageBerthAssignments = "ManageBerthAssignments";

        // Cargo Permissions
        public const string ViewCargo = "ViewCargo";
        public const string ManageCargo = "ManageCargo";

        // Equipment Permissions
        public const string ViewEquipment = "ViewEquipment";
        public const string ManageEquipment = "ManageEquipment";

        // Analytics Permissions
        public const string ViewDashboard = "ViewDashboard";
        public const string ViewReports = "ViewReports";
        public const string GenerateReports = "GenerateReports";
        public const string ExportData = "ExportData";

        // Event Permissions
        public const string ViewEvents = "ViewEvents";
        public const string ManageEvents = "ManageEvents";
    }
}
