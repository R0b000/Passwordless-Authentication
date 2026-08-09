namespace Auth.UI.Manager.Routes
{
    public static class RbacRoute
    {
        public const string GetAllUsers = "/api/Rbac/users";
        public const string GetAllRoles = "/api/Rbac/roles";
        public const string GetAllPermissions = "/api/Rbac/permissions";
        public const string AssignRole = "/api/Rbac/users/roles";
        public const string RemoveRole = "/api/Rbac/users/roles";
    }
}
