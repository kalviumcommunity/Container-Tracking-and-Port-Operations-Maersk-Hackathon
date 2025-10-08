namespace Backend.Constants
{
    /// <summary>
    /// System role constants
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string PortManager = "PortManager";
        public const string Operator = "Operator";
        public const string Viewer = "Viewer";

        public static readonly string[] All = { Admin, PortManager, Operator, Viewer };
        public static readonly string[] Management = { Admin, PortManager };
        public static readonly string[] Operations = { Admin, PortManager, Operator };
    }
}


